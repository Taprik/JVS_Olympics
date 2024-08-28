using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tir
{
    public class Tir_ScoreSceneManager : MonoBehaviour
    {
        public static Tir_ScoreSceneManager Instance => _instance;
        private static Tir_ScoreSceneManager _instance;

        public void Awake()
        {
            if (Instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);
        }

        [SerializeField]
        ScoreBoardDisplayer _scoreBoardDisplayer;

        private TMP_FontAsset Font() => _winnerFont;
        [SerializeField]
        TMP_FontAsset _winnerFont;

        public async void OnReceiveName(string name)
        {
            PlayerData data = new PlayerData()
            {
                Name = name,
                Score = PlayerPrefs.GetFloat(Tir_GeneralVariables.WinnerScoreKey)
            };

            PlayerData defaultPlayer = new PlayerData()
            {
                Name = "Inconnu",
                Score = 0
            };

            //_scoreBoardDisplayer.InitScoreBoard(await ScoreBoardManager.UpdateScoreBoardDescendingOrder(data, GameScoreBoard.TirScoreBoard), Font, defaultPlayer);
        }
    }
}