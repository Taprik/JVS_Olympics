using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public enum GameScoreBoard
{
    BlockScoreBoard,
    QuizScoreBoard
}

public class ScoreBoardManager : MonoBehaviour
{
    [SerializeField]
    GameScoreBoard[] _allScoreBoard;

    string GetPath(GameScoreBoard game) => Application.persistentDataPath + "/" + game.ToString() + ".json";

    public async void ResetAllScoreBoard()
    {
        foreach (GameScoreBoard scoreBoard in _allScoreBoard)
        {
            await CreateScoreBoard(scoreBoard);
        }
    }

    public async Task CreateScoreBoard(GameScoreBoard game)
    {
        if (File.Exists(GetPath(game)))
        {
            Debug.LogWarning("Old ScoreBoard File Delete");
            File.Delete(GetPath(game));
        }

        await using FileStream file = File.Create(GetPath(game));
        //string json = JsonConvert.SerializeObject(playerDatas);
        //File.WriteAllText(PATH, json);
    }

    public async Task<PlayerData[]> GetScoreBoard(GameScoreBoard game)
    {
        if (!File.Exists(GetPath(game)))
        {
            Debug.LogWarning("No ScoreBoard File found at " + GetPath(game));
            await CreateScoreBoard(game);
        }

        string json = File.ReadAllText(GetPath(game));

        if(json == string.Empty)
            return new PlayerData[0];

        PlayerData[] playerDatas = JsonHelper.FromJson<PlayerData>(json);

        playerDatas ??= new PlayerData[0];

        return playerDatas;
    }

    public async Task<PlayerData[]> UpdateScoreBoardAscendingOrder(PlayerData playerData, GameScoreBoard game)
    {
        PlayerData[] oldPlayerDatas = await GetScoreBoard(game);
        if (oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return null;
        }

        oldPlayerDatas = oldPlayerDatas.OrderBy(x => x.Score).ToArray();

        List<PlayerData> finalJson = new List<PlayerData>();
        for (int i = 0; i < 30; i++)
        {
            if (finalJson.Count >= 30)
                break;

            if (oldPlayerDatas.Length <= i)
            {
                if (!finalJson.Contains(playerData))
                {
                    playerData.Rank = finalJson.Count + 1;
                    playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                    finalJson.Add(playerData);
                }
                break;
            }

            if (oldPlayerDatas[i].Score > playerData.Score)
            {
                playerData.Rank = finalJson.Count + 1;
                playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                finalJson.Add(playerData);

                if (finalJson.Count >= 30)
                    break;
            }

            oldPlayerDatas[i].Rank = finalJson.Count + 1;
            finalJson.Add(oldPlayerDatas[i]);
        }

        string json = JsonHelper.ToJson(finalJson.ToArray(), true);
        File.WriteAllText(GetPath(game), json);
        return finalJson.ToArray();
    }

    public async Task<PlayerData[]> UpdateScoreBoardAscendingOrder(PlayerData[] playerDatas, GameScoreBoard game)
    {
        PlayerData[] oldPlayerDatas = await GetScoreBoard(game);
        if (oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return null;
        }

        oldPlayerDatas = oldPlayerDatas.OrderBy(x => x.Score).ToArray();

        Queue<PlayerData> newPlayerDatas = new Queue<PlayerData>(playerDatas.OrderBy(x => x.Score));
        List<PlayerData> finalJson = new List<PlayerData>();
        for (int i = 0; i < 30; i++)
        {
            if (finalJson.Count >= 30)
                break;

            if (oldPlayerDatas.Length <= i)
            {
                if (newPlayerDatas.Count <= 0)
                    break;

                PlayerData playerData = newPlayerDatas.Dequeue();
                playerData.Rank = finalJson.Count + 1;
                playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                finalJson.Add(playerData);
                continue;
            }

            if(newPlayerDatas.Count > 0)
            {
                if (oldPlayerDatas[i].Score > newPlayerDatas.Peek().Score)
                {
                    PlayerData playerData = newPlayerDatas.Dequeue();
                    playerData.Rank = finalJson.Count + 1;
                    playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                    finalJson.Add(playerData);

                    if (finalJson.Count >= 30)
                        break;
                }
            }

            oldPlayerDatas[i].Rank = finalJson.Count + 1;
            finalJson.Add(oldPlayerDatas[i]);
        }

        string json = JsonHelper.ToJson(finalJson.ToArray(), true);
        File.WriteAllText(GetPath(game), json);
        return finalJson.ToArray();
    }

    public async Task<PlayerData[]> UpdateScoreBoardDescendingOrder(PlayerData playerData, GameScoreBoard game)
    {
        PlayerData[] oldPlayerDatas = await GetScoreBoard(game);
        if (oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return null;
        }

        oldPlayerDatas = oldPlayerDatas.OrderByDescending(x => x.Score).ToArray();

        List<PlayerData> finalJson = new List<PlayerData>();
        for (int i = 0; i < 30; i++)
        {
            if (finalJson.Count >= 30)
                break;

            if (oldPlayerDatas.Length <= i)
            {
                if (!finalJson.Contains(playerData))
                {
                    playerData.Rank = finalJson.Count + 1;
                    playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                    finalJson.Add(playerData);
                }
                break;
            }

            if (oldPlayerDatas[i].Score < playerData.Score)
            {
                playerData.Rank = finalJson.Count + 1;
                playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                finalJson.Add(playerData);

                if (finalJson.Count >= 30)
                    break;
            }

            oldPlayerDatas[i].Rank = finalJson.Count + 1;
            finalJson.Add(oldPlayerDatas[i]);
        }

        string json = JsonHelper.ToJson(finalJson.ToArray(), true);
        File.WriteAllText(GetPath(game), json);
        return finalJson.ToArray();
    }

    public async Task<PlayerData[]> UpdateScoreBoardDescendingOrder(PlayerData[] playerDatas, GameScoreBoard game)
    {
        PlayerData[] oldPlayerDatas = await GetScoreBoard(game);
        if (oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return null;
        }

        oldPlayerDatas = oldPlayerDatas.OrderByDescending(x => x.Score).ToArray();

        Queue<PlayerData> newPlayerDatas = new Queue<PlayerData>(playerDatas.OrderByDescending(x => x.Score));
        List<PlayerData> finalJson = new List<PlayerData>();
        for (int i = 0; i < 30; i++)
        {
            if (finalJson.Count >= 30)
                break;

            if (oldPlayerDatas.Length <= i)
            {
                if (newPlayerDatas.Count <= 0)
                    break;

                PlayerData playerData = newPlayerDatas.Dequeue();
                playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                playerData.Rank = finalJson.Count + 1;
                finalJson.Add(playerData);
                continue;
            }

            if(newPlayerDatas.Count > 0)
            {
                if (oldPlayerDatas[i].Score < newPlayerDatas.Peek().Score)
                {
                    PlayerData playerData = newPlayerDatas.Dequeue();
                    playerData.Date = DateTime.UtcNow.ToString(@"dd\:HH\:mm\:ss\:ff");
                    playerData.Rank = finalJson.Count + 1;
                    finalJson.Add(playerData);

                    if (finalJson.Count >= 30)
                        break;
                }
            }

            oldPlayerDatas[i].Rank = finalJson.Count + 1;
            finalJson.Add(oldPlayerDatas[i]);
        }

        string json = JsonHelper.ToJson(finalJson.ToArray(), true);
        File.WriteAllText(GetPath(game), json);
        return finalJson.ToArray();
    }
}

[System.Serializable]
public struct PlayerData
{
    public string Name;
    public string Date;
    public int Rank;
    public float Score;
}

public static class JsonHelper
{
    public static T[] FromJson<T>(string json)
    {
        Wrapper<T> wrapper = JsonUtility.FromJson<Wrapper<T>>(json);
        return wrapper.Items;
    }

    public static string ToJson<T>(T[] array)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper);
    }

    public static string ToJson<T>(T[] array, bool prettyPrint)
    {
        Wrapper<T> wrapper = new Wrapper<T>();
        wrapper.Items = array;
        return JsonUtility.ToJson(wrapper, prettyPrint);
    }

    [Serializable]
    private class Wrapper<T>
    {
        public T[] Items;
    }
}
