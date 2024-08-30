using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Unity.VisualScripting;
using UnityEngine;

public enum GameScoreBoard
{
    BlockScoreBoard,
    QuizScoreBoard,
    BasketballBoard,
    TirScoreBoard,
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

        List<PlayerData> finalJson = new List<PlayerData>();

        foreach (PlayerData d in oldPlayerDatas)
            d.WinNow = false;

        playerData.WinNow = true;

        List<PlayerData> allPlayerData = new List<PlayerData>(oldPlayerDatas)
        {
            playerData
        };
        allPlayerData = allPlayerData.OrderBy(x => x.Score).ToList();

        for (int i = 0; i < allPlayerData.Count; i++)
        {
            allPlayerData[i].Rank = i + 1;
            finalJson.Add(allPlayerData[i]);
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

        List<PlayerData> finalJson = new List<PlayerData>();

        foreach (PlayerData d in oldPlayerDatas)
            d.WinNow = false;

        foreach (PlayerData d in playerDatas)
            d.WinNow = true;

        List<PlayerData> allPlayerData = new List<PlayerData>(oldPlayerDatas);
        allPlayerData.AddRange(playerDatas);
        allPlayerData = allPlayerData.OrderBy(x => x.Score).ToList();

        for (int i = 0; i < allPlayerData.Count; i++)
        {
            allPlayerData[i].Rank = i + 1;
            finalJson.Add(allPlayerData[i]);
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

        List<PlayerData> finalJson = new List<PlayerData>();

        foreach (PlayerData d in oldPlayerDatas)
            d.WinNow = false;

        playerData.WinNow = true;

        List<PlayerData> allPlayerData = new List<PlayerData>(oldPlayerDatas)
        {
            playerData
        };
        allPlayerData = allPlayerData.OrderByDescending(x => x.Score).ToList();

        for (int i = 0; i < allPlayerData.Count; i++)
        {
            allPlayerData[i].Rank = i + 1;
            finalJson.Add(allPlayerData[i]);
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

        List<PlayerData> finalJson = new List<PlayerData>();

        foreach (PlayerData d in oldPlayerDatas)
            d.WinNow = false;

        foreach (PlayerData d in playerDatas)
            d.WinNow = true;

        List<PlayerData> allPlayerData = new List<PlayerData>(oldPlayerDatas);
        allPlayerData.AddRange(playerDatas);
        allPlayerData = allPlayerData.OrderByDescending(x => x.Score).ToList();

        for (int i = 0; i < allPlayerData.Count; i++)
        {
            allPlayerData[i].Rank = i + 1;
            finalJson.Add(allPlayerData[i]);
        }

        string json = JsonHelper.ToJson(finalJson.ToArray(), true);
        File.WriteAllText(GetPath(game), json);
        return finalJson.ToArray();
    }
}

[System.Serializable]
public class PlayerData
{
    public string Name;
    public int Rank;
    public bool WinNow;
    public float Score;
    public string Value;
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
