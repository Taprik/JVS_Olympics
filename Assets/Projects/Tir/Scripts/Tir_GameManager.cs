using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Tir
{
    [RequireComponent(typeof(Tir_SceneManager))]
    [RequireComponent(typeof(Tir_ScoreManager))]
    [RequireComponent(typeof(Tir_TargetManager))]
    [RequireComponent(typeof(Tir_TimerManager))]
    [RequireComponent(typeof(Tir_SoundManager))]
    public class Tir_GameManager : MonoBehaviour
    {
        public static Tir_GameManager Instance => _instance;
        private static Tir_GameManager _instance;

        public void Awake()
        {
            if (Instance == null)
                _instance = this;
            else
                Destroy(this.gameObject);
        }

        public enum GameState
        {
            None,
            Play,
            Stop
        }
        public GameState CurrentState => _currentState;
        private GameState _currentState;

        public Tir_SceneManager SceneManager => _sceneManager;
        private Tir_SceneManager _sceneManager;

        public Tir_ScoreManager ScoreManager => _scoreManager;
        private Tir_ScoreManager _scoreManager;

        public Tir_TargetManager TargetManager => _targetManager;
        private Tir_TargetManager _targetManager;

        public Tir_TimerManager TimerManager => _timerManager;
        private Tir_TimerManager _timerManager;

        public Tir_SoundManager SoundManager => _soundManager;
        private Tir_SoundManager _soundManager;

        public IEnumerator Start()
        {
            _sceneManager = GetComponent<Tir_SceneManager>();
            _scoreManager = GetComponent<Tir_ScoreManager>();
            _targetManager = GetComponent<Tir_TargetManager>();
            _timerManager = GetComponent<Tir_TimerManager>();
            _soundManager = GetComponent<Tir_SoundManager>();

            _timerManager.OnTimerEnd += GameEnd;
            OnGameEnd += () => StartCoroutine(WaitBeforeGoToScore());
            Sticker();

            yield return null;

            GameStart();
        }

        public Action OnGameStart;
        public void GameStart()
        {
            OnGameStart?.Invoke();
            _currentState = GameState.Play;
        }

        public Action OnGameEnd;
        public void GameEnd()
        {
            _currentState = GameState.Stop;
            TimerManager.OnTimerEnd -= GameEnd;
            OnGameEnd?.Invoke();
        }

        public float _timeBeforeGoingToScore;
        IEnumerator WaitBeforeGoToScore()
        {
            yield return new WaitForSeconds(_timeBeforeGoingToScore);
            yield return new WaitUntil(() => ScoreManager.Teams.ToList().TrueForAll(x => x.EndAnimation == true));
            SceneManager.LoadScene(Tir_Scene.Score_Tir);
        }

        public void Update()
        {
            switch (_currentState)
            {
                case GameState.Play:
                    GameStartUpdate();
                    break;

                case GameState.Stop:
                    break;

                case GameState.None:
                    break;
            }
        }

        void GameStartUpdate()
        {
            TargetManager.TryToSpawnTarget();
        }

        [SerializeField] GameObject _sticker;
        void Sticker()
        {
            _sticker.SetActive(PlayerPrefs.GetInt(Tir_GeneralVariables.StickerKey) == 1);
        }
    }
}