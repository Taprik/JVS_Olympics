using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class ScoreBoardManager : MonoBehaviour
{
    private string PATH { get; set; }

    private void Awake()
    {
        PATH = Application.persistentDataPath + "/ScoreBoard.json";
    }

    public async Task CreateScoreBoard()
    {
        if (File.Exists(PATH))
        {
            Debug.LogWarning("Old ScoreBoard File Delete");
            File.Delete(PATH);
        }

        await using FileStream file = File.Create(PATH);
        //string json = JsonConvert.SerializeObject(playerDatas);
        //File.WriteAllText(PATH, json);
    }

    public PlayerData[] GetScoreBoard()
    {
        if (!File.Exists(PATH))
        {
            Debug.LogError("No ScoreBoard File found at " + PATH);
            return null;
        }

        string json = File.ReadAllText(PATH);
        PlayerData[] playerDatas = JsonUtility.FromJson<PlayerData[]>(json);

        return playerDatas;
    }

    public PlayerData[] UpdateScoreBoard(PlayerData playerData)
    {
        PlayerData[] oldPlayerDatas = GetScoreBoard();
        if(oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return null;
        }

        List<PlayerData> finalJson = new List<PlayerData>();
        for (int i = 0; i < 30; i++)
        {
            if (finalJson.Count >= 30)
                break;

            if (oldPlayerDatas.Length <= i)
            {
                if (!finalJson.Contains(playerData))
                {
                    finalJson.Add(playerData);
                    playerData.Rank = finalJson.Count;
                }
                break;
            }

            if (oldPlayerDatas[i].Score < playerData.Score)
            {
                finalJson.Add(playerData);
                playerData.Rank = finalJson.Count;

                if (finalJson.Count >= 30)
                    break;
            }

            finalJson.Add(oldPlayerDatas[i]);
            oldPlayerDatas[i].Rank = finalJson.Count;
        }

        string json = JsonConvert.SerializeObject(finalJson);
        File.WriteAllText(PATH, json);
        return finalJson.ToArray();
    }

    public PlayerData[] UpdateScoreBoard(PlayerData[] playerDatas)
    {
        PlayerData[] oldPlayerDatas = GetScoreBoard();
        if (oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return null;
        }

        Queue<PlayerData> newPlayerDatas = new Queue<PlayerData>(playerDatas.OrderBy(x => x.Score));
        List<PlayerData> finalJson = new List<PlayerData>();
        for (int i = 0; i < 30; i++)
        {
            if (finalJson.Count >= 30)
                break;

            if (oldPlayerDatas.Length <= i)
            {
                PlayerData playerData = newPlayerDatas.Dequeue();
                finalJson.Add(playerData);
                playerData.Rank = finalJson.Count;
                continue;
            }

            if (oldPlayerDatas[i].Score < newPlayerDatas.Peek().Score)
            {
                PlayerData playerData = newPlayerDatas.Dequeue();
                finalJson.Add(playerData);
                playerData.Rank = finalJson.Count;

                if (finalJson.Count >= 30)
                    break;
            }

            finalJson.Add(oldPlayerDatas[i]);
            oldPlayerDatas[i].Rank = finalJson.Count;
        }

        string json = JsonConvert.SerializeObject(finalJson);
        File.WriteAllText(PATH, json);
        return finalJson.ToArray();
    }
}

public struct PlayerData
{
    public string Name;
    public string Date;
    public int Rank;
    public float Score;
}
