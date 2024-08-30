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

        [SerializeField] Color _winnerColor;

        public async void OnReceiveName(string name)
        {
            PlayerData data = new PlayerData()
            {
                Name = name,
                Score = PlayerPrefs.GetFloat(Tir_SceneObject.WinnerScoreKey)
            };

            PlayerData defaultPlayer = new PlayerData()
            {
                Name = "Inconnu",
                Score = 0
            };

            _scoreBoardDisplayer.InitScoreBoard(await GameManager.Instance.ScoreBoardManager.UpdateScoreBoardDescendingOrder(data, GameScoreBoard.TirScoreBoard), Font, _winnerColor, defaultPlayer);
        }
    }
}