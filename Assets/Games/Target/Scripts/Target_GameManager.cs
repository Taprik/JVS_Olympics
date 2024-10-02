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
        public List<Target_Animation> Targets;
        [SerializeField] Transform TargetHolder;
        [SerializeField] int _maxTarget;
        [SerializeField] TextMeshProUGUI _scoreText;
        [SerializeField] int _timer;

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
                    yield return new WaitForSeconds(4f);

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

            for (int i = 0; i < 10; i++)
            {

                yield return new WaitForSeconds(1f);

            }

            IsGameOver = true;
        }

        private void SpawnTarget(int nb)
        {
            Vector2 pos = new Vector2(UnityEngine.Random.Range(-6f, 6f), UnityEngine.Random.Range(-3.5f, 3.5f));
            var t = Instantiate(_targetPrefab, pos, Quaternion.identity, TargetHolder);
            var a = t.GetComponent<Target_Animation>();
            a.AddOrderInLayer(nb * 2);
            Targets.Add(a);

        }

        public void AddPoint()
        {
            Score++;
            _scoreText.text = Score.ToString();
        }
    }
}