using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Target
{
    public class Target_GameManager : MonoBehaviour
    {
        public static Target_GameManager Instance;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public Camera Cam => _cam;
        [SerializeField] Camera _cam;

        [SerializeField] GameObject _targetPrefab;
        public bool IsGameOver { get; private set; } = false;

        public int Score;
        public List<Target_Animation> Targets = new();
        [SerializeField] Transform TargetHolder;
        [SerializeField] int _maxTarget;
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] TextMeshProUGUI _endText;
        [SerializeField] int _timer;
        private float delay = 4f;

        public ScoreBoardDisplayer ScoreBaord => _scoreboard;
        [SerializeField] ScoreBoardDisplayer _scoreboard;
        [SerializeField] TMP_FontAsset _font;
        public TMP_FontAsset Font() => _font;


        public void Start()
        {
            GameManager.OnGameStart += GameStart;
        }

        public void OnDestroy()
        {
            GameManager.OnGameStart -= GameStart;
        }

        private void GameStart()
        {
            for (int i = 0; i < Targets.Count; i++)
            {
                Destroy(Targets[i].gameObject);
            }
            Targets.Clear();
            Score = 0;
            IsGameOver = false;

            if (PlayerPrefs.HasKey("Target_Timer"))
                _timer = PlayerPrefs.GetInt("Target_Timer") * 30 + 60;

            if (PlayerPrefs.HasKey("Target_MaxSpawn"))
                _maxTarget = Mathf.RoundToInt(PlayerPrefs.GetFloat("Target_MaxSpawn"));

            if (PlayerPrefs.HasKey("Target_SpawnRate"))
                delay = Mathf.RoundToInt(PlayerPrefs.GetFloat("Target_SpawnRate"));

            StartCoroutine(GameLoop());
        }

        private IEnumerator GameLoop()
        {
            yield return null;

            bool endGameSoon = false;
            int nbTarget = 0;
            var time = DateTime.UtcNow;
            while (!IsGameOver)
            {
                while(_maxTarget > Targets.Count)
                {
                    nbTarget++;
                    SpawnTarget(nbTarget);
                    yield return new WaitForSeconds(delay);

                    if ((float)(DateTime.UtcNow - time).TotalSeconds > _timer - 10f)
                    {
                        if (!endGameSoon)
                        {
                            StartCoroutine(End());
                            endGameSoon = true;
                        }

                        if ((float)(DateTime.UtcNow - time).TotalSeconds > _timer)
                        {
                            break;
                        }
                    }
                }

                yield return null;

                if((float)(DateTime.UtcNow - time).TotalSeconds > _timer - 10f)
                {
                    if (!endGameSoon)
                    {
                        StartCoroutine(End());
                        endGameSoon = true;
                    }

                    if ((float)(DateTime.UtcNow - time).TotalSeconds > _timer)
                    {
                        break;
                    }
                }
            }
        }

        private IEnumerator End()
        {
            Debug.Log("End");
            _endText.gameObject.SetActive(true);
            for (int i = 0; i < 10; i++)
            {
                _endText.text = (10 - i).ToString();
                yield return new WaitForSeconds(1f);
            }
            _endText.gameObject.SetActive(false);

            Target_SoundManager.Instance.PlayEndSound();
            yield return null;
            while (Target_SoundManager.Instance.Source.isPlaying)
            {
                yield return null;
            }

            IsGameOver = true;
            GameManager.CurrentGameSceneObject.PlayScore();
            GameManager.Instance.OSCManager.NeedName();
        }

        private void SpawnTarget(int nb)
        {
            Vector2 pos = new Vector2(UnityEngine.Random.Range(-6f, 6f), UnityEngine.Random.Range(-3.5f, 3.5f));
            var t = Instantiate(_targetPrefab, pos, Quaternion.identity, TargetHolder);
            var a = t.GetComponent<Target_Animation>();
            a.AddOrderInLayer(nb * 2);
            Targets.Add(a);

        }

        public void AddPoint(int point)
        {
            Score += point;
            _scoreText.text = Score.ToString();
        }

        public async void OnReceiveName(string name)
        {
            PlayerData data = new PlayerData()
            {
                Name = name,
                Score = Score
            };

            PlayerData defaultPlayer = new PlayerData()
            {
                Name = "Inconnue",
                Score = 0
            };

            _scoreboard.InitScoreBoard(await GameManager.Instance.ScoreBoardManager.UpdateScoreBoardDescendingOrder(data, GameScoreBoard.TargetScoreBoard), Font, defaultPlayer);
        }
    }
}