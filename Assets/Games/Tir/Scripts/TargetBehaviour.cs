using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Tir
{
    public class TargetBehaviour : MonoBehaviour, IReceivePoint
    {
        [SerializeField]
        RectTransform _body;

        [SerializeField]
        Image _backImage;

        [SerializeField]
        Image _frontImage;

        [SerializeField]
        Image _shadowImage;

        [SerializeField]
        TargetSprite[] _targetSprites;

        [SerializeField]
        bool _OnStart;

        public TirTeam Team { get; set; }
        public Action<TirTeam, int, RectTransform> OnDestroy;
        private float TimeBetweenPhase;
        public float TimerMultiplicator { get; set; }
        public float MaxTimeToDestroy;

        public int CurrentID => _currentID;

        int _currentID;
        IEnumerator _coroutine;
        bool _isDestroy = false;

        private void Start()
        {
            if (PlayerPrefs.HasKey(Tir_SceneObject.TimeBetweenPhaseKey))
            {
                TimeBetweenPhase = PlayerPrefs.GetFloat(Tir_SceneObject.TimeBetweenPhaseKey);
            }

            _isDestroy = false;
            OnDestroy += (team, score, rect) => team.LastSpawn = DateTime.UtcNow;
            Tir_GameManager.Instance.OnGameEnd += () => { 
                if (!_isDestroy && this.gameObject.activeSelf)
                {
                    StopCoroutine(_coroutine); 

                }
            };

            if (_OnStart)
                StartAnim();

            AjustShadowHeight();
        }

        private void OnDisable()
        {
            Destroy(this.gameObject);
        }

        public void StartAnim()
        {
            if (_coroutine != null)
                StopCoroutine(_coroutine);
            _coroutine = Anim();
            StartCoroutine(_coroutine);
        }

        private IEnumerator Anim()
        {
            void NextImage(int id)
            {
                if (_targetSprites.Length <= id) return;

                _currentID = id;
                _backImage.transform.localScale = new Vector3(_targetSprites[id].backRatio, _targetSprites[id].backRatio);
                _shadowImage.transform.localScale = new Vector3(_targetSprites[id].backRatio, _targetSprites[id].backRatio);
                _frontImage.sprite = _targetSprites[id].sprites[0];
                AjustShadowHeight();
            }

            void ChangeAlpha(float alpha)
            {
                _backImage.color = new Color(_backImage.color.r, _backImage.color.g, _backImage.color.b, alpha);
                _frontImage.color = new Color(_frontImage.color.r, _frontImage.color.g, _frontImage.color.b, alpha);
                _shadowImage.color = new Color(_shadowImage.color.r, _shadowImage.color.g, _shadowImage.color.b, alpha);
            }

            ChangeAlpha(0f);
            yield return new WaitForSeconds(Mathf.Clamp(TimerMultiplicator != 0f ? TimeBetweenPhase * TimerMultiplicator : TimeBetweenPhase, 0.1f, float.MaxValue));
            ChangeAlpha(1f);

            for (int i = 0; i < _targetSprites.Length; i++)
            {
                NextImage(i);
                yield return new WaitForSeconds(Mathf.Clamp(TimerMultiplicator != 0f ? TimeBetweenPhase * TimerMultiplicator : TimeBetweenPhase, 0.1f, float.MaxValue));
            }

            yield return null;
        }

        public void GetDestroy()
        {
            if (_isDestroy || Tir_GameManager.Instance.CurrentState == Tir_GameManager.GameState.Stop) return;
            _isDestroy = true;
            StopCoroutine(_coroutine);
            StartCoroutine(DestroyAnim());
            OnDestroy?.Invoke(Team, _currentID, this.transform as RectTransform);
            OnDestroy -= Tir_GameManager.Instance.ScoreManager.AddScore;
        }

        private IEnumerator DestroyAnim()
        {
            int id = _currentID;

            foreach (var sprite in _targetSprites[id].sprites)
            {
                _frontImage.sprite = sprite;
                yield return new WaitForSeconds(MaxTimeToDestroy / 3f * (((id / 2f) + 5f) / 10f));
            }

            Destroy(this.gameObject);
            yield return null;
        }

        public void OnImpact()
        {
            Tir_GameManager.Instance.SoundManager.PlayImpactSound(Team);
        }

        private void AjustShadowHeight()
        {
            RectTransform rectTransform = this.transform as RectTransform;
            RectTransform rectTargetHolder = Team.TargetHolder;
            RectTransform rectShadowHolder = Tir_GameManager.Instance.TargetManager.ShadowHolder;

            float oldY = rectTransform.position.y - (rectTargetHolder.position.y - (rectTargetHolder.rect.size.y / 2));
            float pourcent = oldY / rectTargetHolder.rect.size.y;
            float newY = rectShadowHolder.position.y - (rectShadowHolder.rect.size.y / 2) + (rectShadowHolder.rect.size.y * pourcent);
            float scalePourcent = 1 - (pourcent * 0.5f);

            //Debug.Log(scalePourcent);
            _shadowImage.transform.localScale = new Vector3(_shadowImage.transform.localScale.x * scalePourcent, _shadowImage.transform.localScale.y * scalePourcent);

            _shadowImage.transform.position = new Vector3(_shadowImage.transform.position.x, 125f);
        }

        public void ReceivePoint(float xPoint, float yPoint)
        {
            var point = new Vector3(xPoint * Screen.width, yPoint * Screen.height);
            var pos = Camera.main.ScreenToWorldPoint(point);
            pos.z = 0;

            if(Tool.ToolBox.CheckPos(pos, this.transform as RectTransform))
            {
                Destroy(Instantiate(Tir_GameManager.Instance.TargetManager.HitPointPref, point, Quaternion.identity, this.transform.parent.parent), 1f);
            }
        }
    }

    [System.Serializable]
    public struct TargetSprite
    {
        public Sprite[] sprites;
        public float backRatio;
    }
}