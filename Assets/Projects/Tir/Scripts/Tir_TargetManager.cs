using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tir
{
    public class Tir_TargetManager : MonoBehaviour
    {
        public GameObject TargetPrefab => _targetPrefab;
        [SerializeField]
        private GameObject _targetPrefab;

        [SerializeField]
        float _timeBetweenSpawn;

        [SerializeField]
        int _maxSpawnableTarget;

        public RectTransform ShadowHolder => _shadowHolder;
        [SerializeField]
        RectTransform _shadowHolder;

        private float _lastMultiplicator = 1f;

        public GameObject HitPointPref => _hitPointPref;
        [SerializeField] private GameObject _hitPointPref;

        public void Start()
        {
            Tir_GameManager.Instance.OnGameEnd += DisableAllTarget;
        }

        public void TryToSpawnTarget()
        {
            foreach (var team in Tir_GameManager.Instance.ScoreManager.Teams)
            {
                TimeSpan dif = DateTime.UtcNow - team.LastSpawn;
                if ((float)dif.TotalSeconds > Mathf.Clamp(_timeBetweenSpawn * _lastMultiplicator, 0.5f, float.MaxValue) && team.SpawnTargets.Count < _maxSpawnableTarget)
                {
                    //team.LastSpawn = DateTime.UtcNow;
                    SpawnTarget(team.TargetHolder, team);
                }
            }
        }

        private void SpawnTarget(RectTransform transform, TirTeam team)
        {
            float x = Random.Range(0, transform.rect.width) - (transform.rect.width / 2f);
            float y = Random.Range(0, transform.rect.height) - (transform.rect.height / 2f);
            TargetBehaviour targetBehaviour = Instantiate(_targetPrefab, transform.position + new Vector3(x, y), Quaternion.identity, transform).GetComponent<TargetBehaviour>();
            //Debug.Log(targetBehaviour.transform.position + " | " + targetBehaviour.transform.localPosition);
            _lastMultiplicator *= 0.99f - (PlayerPrefs.GetInt(Tir_GeneralVariables.DifficultyKey) * 0.03f);
            targetBehaviour.TimerMultiplicator = _lastMultiplicator;
            targetBehaviour.transform.localScale = Vector3.one * (1f - (PlayerPrefs.GetInt(Tir_GeneralVariables.DifficultyKey) / 4f));
            targetBehaviour.Team = team;
            targetBehaviour.OnDestroy += Tir_GameManager.Instance.ScoreManager.AddScore;
            team.SpawnTargets.Add(targetBehaviour);
            targetBehaviour.OnDestroy += (t, score, rect) => t.SpawnTargets.Remove(targetBehaviour);
        }

        private void DisableAllTarget()
        {
            Tir_GameManager.Instance.OnGameEnd -= DisableAllTarget;

            foreach (var team in Tir_GameManager.Instance.ScoreManager.Teams)
            {
                for (int i = 0; i < team.SpawnTargets.Count; i++)
                {
                    //team.SpawnTargets[i].transform.GetChild(0).GetComponentInChildren<Universal_Button>().IsActive = false;
                }
            }
        }
    }
}