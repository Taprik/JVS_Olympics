using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tetrax
{
    public class Tetrax_ScoreManager : MonoBehaviour
    {
        public static Tetrax_ScoreManager Instance;

        public void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
        }

        [SerializeField] ScoreBoardDisplayer _scoreBoard;
        private TMP_FontAsset Font() => _font;
        [SerializeField] TMP_FontAsset _font;
        [SerializeField] Color _winnerColor;

        public async void OnReceiveName(string name)
        {
            PlayerData data = new PlayerData()
            {
                Name = name,
                Score = Tetrax_GameManager.WinnerScore
            };

            PlayerData defaultPlayer = new PlayerData()
            {
                Name = "Inconnu",
                Score = 0
            };

            _scoreBoard.InitScoreBoard(await GameManager.Instance.ScoreBoardManager.UpdateScoreBoardDescendingOrder(data, GameScoreBoard.TetraxScoreBoard), Font, _winnerColor, defaultPlayer);
        }

        public void PageDown()
        {
            _scoreBoard.PageDown();
        }

        public void PageUp()
        {
            _scoreBoard.PageUp();
        }
    }
}