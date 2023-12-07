using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "ScoreBoardManager", menuName = "Manager/ScoreBoardManager")]
public class ScoreBoardManager : ScriptableObject
{
    const string PATH = "" + @"\ScoreBoard.json";

    public async Task CreateScoreBoard(PlayerData[] playerDatas)
    {
        if (!File.Exists(PATH))
        {
            await using FileStream file = File.Create(PATH);
            string json = JsonConvert.SerializeObject(playerDatas);
            File.WriteAllText(PATH, json);
        }
        else
        {
            Debug.LogError("File Already exist");
        }
    }

    public async Task UpdateScoreBoard(PlayerData playerData)
    {
        string json = JsonConvert.SerializeObject(playerData);
        File.WriteAllText(PATH, json);
    }

    public async Task UpdateScoreBoard(PlayerData[] playerDatas)
    {
        string json = JsonConvert.SerializeObject(playerDatas);
        File.WriteAllText(PATH, json);
    }
}

public struct PlayerData
{
    public string Name;
    public string Date;
    public object Score;
}
