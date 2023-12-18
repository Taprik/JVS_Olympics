using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreBoardManager", menuName = "Manager/ScoreBoardManager")]
public class ScoreBoardManager : ScriptableObject
{
    readonly string PATH = Application.persistentDataPath + "/ScoreBoard.json";

    public async Task CreateScoreBoard(PlayerData[] playerDatas)
    {
        if (File.Exists(PATH))
        {
            Debug.LogWarning("Old ScoreBoard File Delete");
            File.Delete(PATH);
        }

        await using FileStream file = File.Create(PATH);
        string json = JsonConvert.SerializeObject(playerDatas);
        File.WriteAllText(PATH, json);
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

    public int UpdateScoreBoard(PlayerData playerData)
    {
        int position = -1;
        PlayerData[] oldPlayerDatas = GetScoreBoard();
        if(oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return position;
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
                    position = finalJson.Count;
                }
                break;
            }

            if (oldPlayerDatas[i].Score < playerData.Score)
            {
                finalJson.Add(playerData);
                position = finalJson.Count;

                if (finalJson.Count >= 30)
                    break;

                finalJson.Add(oldPlayerDatas[i]);
            }
            else
                finalJson.Add(oldPlayerDatas[i]);

        }

        string json = JsonConvert.SerializeObject(finalJson);
        File.WriteAllText(PATH, json);
        return position;
    }

    public Dictionary<PlayerData, int> UpdateScoreBoard(PlayerData[] playerDatas)
    {
        Dictionary<PlayerData, int> position = new Dictionary<PlayerData, int>();
        PlayerData[] oldPlayerDatas = GetScoreBoard();
        if (oldPlayerDatas == null)
        {
            Debug.LogError("ScoreBoard is null");
            return position;
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
                position.Add(playerData, finalJson.Count);
                continue;
            }

            if (oldPlayerDatas[i].Score < newPlayerDatas.Peek().Score)
            {
                PlayerData playerData = newPlayerDatas.Dequeue();
                finalJson.Add(playerData);
                position.Add(playerData, finalJson.Count);

                if (finalJson.Count >= 30)
                    break;

                finalJson.Add(oldPlayerDatas[i]);
            }
        }

        string json = JsonConvert.SerializeObject(finalJson);
        File.WriteAllText(PATH, json);
        return position;
    }
}

public struct PlayerData
{
    public string Name;
    public string Date;
    public float Score;
}
