using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Tir
{
    public class Tir_TimerManager : MonoBehaviour
    {
        [SerializeField]
        TextMeshProUGUI _timerText;

        [SerializeField]
        RectTransform _needleTransform;

        public int Timer => _timer;
        [SerializeField]
        int _timer;

        public Action OnTimerEnd;
        public TimeSpan CurrentTime => DateTime.UtcNow - _startingTime;

        DateTime _startingTime;
        bool _isPlaying;

        public void Start()
        {
            Tir_GameManager.Instance.OnGameStart += () => _startingTime = DateTime.UtcNow;
            Tir_GameManager.Instance.OnGameStart += () => _isPlaying = true;

            _needleTransform.transform.rotation = Quaternion.Euler(0, 0, 72);

            if (PlayerPrefs.HasKey(Tir_GeneralVariables.TimerKey))
                _timer = PlayerPrefs.GetInt(Tir_GeneralVariables.TimerKey);
        }

        public void Update()
        {
            if (!_isPlaying) return;
            TimeSpan dif = DateTime.UtcNow - _startingTime;

            float rot = 72 - (360 * ((float)dif.TotalSeconds / Timer));
            _needleTransform.transform.rotation = Quaternion.Euler(0, 0, rot);
            _timerText.text = $"{Mathf.FloorToInt((float)dif.TotalSeconds)}'{dif.ToString("ff")}''";

            if(dif.TotalSeconds >= Timer && Tir_GameManager.Instance.CurrentState == Tir_GameManager.GameState.Play)
            {
                //Debug.Log("Timer End");
                _isPlaying = false;
                _timerText.text = $"{Timer}'00''";
                OnTimerEnd?.Invoke();
            }
        }
    }
}