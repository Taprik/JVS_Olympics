using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;

namespace Basket
{
    public class Basket_TimerManager : MonoBehaviour
    {
        public static Basket_TimerManager i;
        private void Awake()
        {
            if (i != null)
            {
                Destroy(gameObject);
                return;
            }

            i = this;

            GameManager.OnGameStart += OnGameStart;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStart -= OnGameStart;
        }

        [SerializeField] TextMeshProUGUI _timerTextLeft;
        [SerializeField] TextMeshProUGUI _timerTextRight;
        public UnityEvent OnTimerEnd;
        public UnityEvent<int> OnUpdateTimer;
        int _timer;

        public void Start()
        {
            OnUpdateTimer.AddListener(DisplayTimer);
        }

        private void OnGameStart()
        {
            int time = 120;
            if (PlayerPrefs.HasKey(Basket_GeneralVariable.TimerKey))
                time = 60 + (PlayerPrefs.GetInt(Basket_GeneralVariable.TimerKey) * 30);
            StartCoroutine(LauchTimer(time));
        }

        public IEnumerator LauchTimer(int initialTimer)
        {
            _timer = initialTimer;

            for (int i = 0; i < initialTimer; i++)
            {
                yield return new WaitForSeconds(1f);
                _timer--;
                OnUpdateTimer?.Invoke(_timer);
            }

            OnTimerEnd?.Invoke();
        }

        private void DisplayTimer(int timer)
        {
            int minutes = timer / 60;
            int seconds = timer % 60;
            _timerTextLeft.text = $"{minutes.ToString("00")}:";
            _timerTextRight.text = $"{seconds.ToString("00")}";
        }
    }
}