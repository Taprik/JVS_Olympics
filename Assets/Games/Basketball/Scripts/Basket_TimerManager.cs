using System.Collections;
using System.Collections.Generic;
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
        }

        public UnityEvent OnTimerEnd;
        public UnityEvent<int> OnUpdateTimer;
        int _timer;

        public void Start()
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
    }
}