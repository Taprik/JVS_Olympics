using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Tool;
using UnityEngine;
using UnityEngine.UI;
using static Tir.Tir_GameManager;

namespace Target
{
    public class Target_Background : MonoBehaviour
    {
        [SerializeField] Transform WaveHolder;
        [SerializeField] Sprite WaveFillSprite;
        [SerializeField] Transform BubbleHolder;
        [SerializeField] Sprite[] BubbleSprite;
        [SerializeField] Image BubblePrefab;

        private void Start()
        {
            GameManager.OnGameStart += GameStart;
        }

        private void GameStart()
        {
            StartCoroutine(StartBackgroundAnim());
            StartCoroutine(StartBubbleAnim());
        }

        private void OnDestroy()
        {
            GameManager.OnGameStart -= GameStart;
        }

        private IEnumerator StartBackgroundAnim()
        {
            int timer = PlayerPrefs.GetInt("Target_Timer");
            timer = 120;
            for (int i = 0; i < WaveHolder.childCount; i++)
            {
                yield return new WaitForSeconds((float)timer / (float)WaveHolder.childCount);
                //Debug.Log("Wave");
                Image wave = WaveHolder.GetChild(WaveHolder.childCount - i - 1).GetComponent<Image>();
                StartCoroutine(WaveAnim(wave));
            }
        }

        private IEnumerator WaveAnim(Image wave)
        {
            wave.sprite = WaveFillSprite;
            (wave.transform as RectTransform).sizeDelta = new Vector2((wave.transform as RectTransform).sizeDelta.x, 50f);
            while (!Target_GameManager.Instance.IsGameOver)
            {
                yield return wave.transform.DOLocalMoveX(100f, 4f).SetEase(Ease.Linear).WaitForCompletion();
                if (Target_GameManager.Instance.IsGameOver) break;
                yield return wave.transform.DOLocalMoveX(-100f, 4f).SetEase(Ease.Linear).WaitForCompletion();
            }
        }

        private IEnumerator StartBubbleAnim()
        {
            while (!Target_GameManager.Instance.IsGameOver)
            {
                yield return new WaitForSeconds(UnityEngine.Random.Range(0.2f, 0.5f));
                var b = Instantiate(BubblePrefab, BubbleHolder);
                b.transform.localPosition = new Vector3(UnityEngine.Random.Range(-900, 900), -500);
                b.sprite = BubbleSprite.RandomElement();
                int size = UnityEngine.Random.Range(5, 10) * 5;
                (b.transform as RectTransform).sizeDelta = new Vector2(size, size);
                StartCoroutine(BubbleAnim(b, size));
            }
        }

        private IEnumerator BubbleAnim(Image bubble, int size)
        {
            float x = bubble.transform.localPosition.x;
            bubble.transform.DOLocalMoveY(600, 1000f / size).SetEase(Ease.Linear);

            Ease ease = Ease.Linear;
            //Debug.Log((size / 50f));
            while (!Target_GameManager.Instance.IsGameOver)
            {
                yield return bubble.transform.DOLocalMoveX(x + 50 * (size / 50f), 1f / (size / 50f / 0.5f) * 2f).SetEase(ease).WaitForCompletion();
                yield return bubble.transform.DOLocalMoveX(x - 50 * (size / 50f), 1f / (size / 50f / 0.5f) * 2f).SetEase(ease).WaitForCompletion();
            }
        }
    }
}