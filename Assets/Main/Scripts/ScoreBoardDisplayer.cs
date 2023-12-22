using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

public class ScoreBoardDisplayer : MonoBehaviour
{
    [SerializeField]
    private string _unit = "pts";

    [SerializeField]
    GameObject _scoreBoardObject;

    [SerializeField]
    GameObject _scoreDisplay;

    [SerializeField]
    Transform _collum;

    List<GameObject> _instantiateScoreDisplay = new();

    PlayerData[] _datas;
    PlayerData _currentWinner;
    Func<TMP_FontAsset> _fontFunc;
    int _currentRank;

    public void InitScoreBoard(PlayerData[] datas, Func<TMP_FontAsset> fontFunc)
    {
        _datas = datas;
        _currentWinner = _datas.ToList().Find(x => x.WinNow);
        _currentRank = _currentWinner.Rank;
        _fontFunc = fontFunc;

        Init(FindPlayerData(_currentRank));

        DisplayScoreBoard();
    }

    public void PageUp()
    {
        if(!_scoreBoardObject.activeSelf) return;

        _currentRank = Mathf.Clamp(_currentRank - 8, 0, _datas.Length);
        Init(FindPlayerData(_currentRank));
    }

    public void PageDown()
    {
        if (!_scoreBoardObject.activeSelf) return;

        _currentRank = Mathf.Clamp(_currentRank + 8, 0, _datas.Length);
        Init(FindPlayerData(_currentRank));
    }

    private PlayerData[] FindPlayerData(int rank)
    {
        int sec = (rank - 1) / 8;
        List<PlayerData> playerFind = new();
        for (int i = 0; i < 8; i++)
        {
            if(((sec * 8) + i) >= _datas.Length)
                continue;

            playerFind.Add(_datas[(sec * 8) + i]);
        }
        return playerFind.ToArray();
    }

    private void Init(PlayerData[] playerDatas)
    {
        ResetDisplay();

        for (int i = 0; i < playerDatas.Length; i++)
        {
            GameObject go = Instantiate(_scoreDisplay, _collum);
            _instantiateScoreDisplay.Add(go);

            string text = playerDatas[i].Rank + ". " + playerDatas[i].Name + " : " + playerDatas[i].Score + _unit;
            TMP_FontAsset font = playerDatas[i].WinNow ? _fontFunc() : null;

            go.GetComponent<ScoreDisplayer>().Init(text, font);
        }
    }

    private void ResetDisplay()
    {
        foreach (GameObject go in _instantiateScoreDisplay)
            Destroy(go);
        _instantiateScoreDisplay.Clear();
    }
    public void DisplayScoreBoard() => _scoreBoardObject.SetActive(true);
    public void HideScoreBoard() => _scoreBoardObject.SetActive(false);
}
