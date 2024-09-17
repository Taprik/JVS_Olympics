using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tool;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Tetrax
{
    [System.Serializable]
    public class TetraxTeam
    {
        public string Name;
        public TeamColor Color;
        public int Score;
        public Transform CanvasPart;
        public Transform SpawnCubeHolder;
        public TextMeshProUGUI ScoreText;
        public TextMeshProUGUI EndText;
        public AudioSource Source;
        [HideInInspector] public List<GameObject> CubesList;
        [HideInInspector] public bool IsGameOver;
    }

    [System.Serializable]
    public class CubeData
    {
        public TeamColor Color;
        public bool IsSpe = false;
        public int Score;
        public RuntimeAnimatorController Animator;
    }

    public enum TeamColor
    {
        Red,
        Blue
    }

    public class Tetrax_GameManager : MonoBehaviour
    {
        public static Tetrax_GameManager Instance;

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

            GameManager.OnGameStart += OnGameStart;
        }

        public static int WinnerScore;
        public bool IsGameOver { get; private set; } = false;

        public TetraxTeam[] Teams => _teams;
        [SerializeField] TetraxTeam[] _teams;

        [SerializeField] GameObject _cubePref;

        public CubeData[] CubesDatas => _cubesDatas;
        [SerializeField] CubeData[] _cubesDatas;

        [SerializeField] private float _tick = 5f;
        [SerializeField] private int _acceleration = 1;
        //[SerializeField] private float _nbCube = 5f;
        [SerializeField] private int _maxCube = 5;
        [SerializeField] private int _minCube = 2;
        //[SerializeField] private int _chanceToSpawn = 60;
        [SerializeField] private int _chanceToSpawnSpeCube = 5;

        [SerializeField] private AudioClip _victory;
        [SerializeField] private AudioClip _failure;

        public void OnDestroy()
        {
            GameManager.OnGameStart -= OnGameStart;
        }

        public void OnGameStart()
        {
            Debug.Log("Start");
            foreach (var team in Teams)
            {
                team.Score = 0;
                team.ScoreText.text = team.Score.ToString("00");
                team.IsGameOver = false;
                for (int i = 0; i < team.CubesList.Count; i++)
                {
                    if (team.CubesList[i] == null) continue;
                    Destroy(team.CubesList[i]);
                }
                team.CubesList.Clear();
                team.EndText.transform.parent.gameObject.SetActive(false);
            }
            WinnerScore = 0;
            IsGameOver = false;

            if (PlayerPrefs.HasKey("Tetrax_Tick")) _tick = PlayerPrefs.GetFloat("Tetrax_Tick");
            else PlayerPrefs.SetFloat("Tetrax_Tick", _tick);
            
            if (PlayerPrefs.HasKey("Tetrax_Acceleration")) _acceleration = Mathf.RoundToInt(PlayerPrefs.GetFloat("Tetrax_Acceleration"));
            else PlayerPrefs.SetFloat("Tetrax_Acceleration", _acceleration);

            if (PlayerPrefs.HasKey("Tetrax_Max")) _maxCube = Mathf.RoundToInt(PlayerPrefs.GetFloat("Tetrax_Max"));
            else PlayerPrefs.SetFloat("Tetrax_Max", _maxCube);

            if (PlayerPrefs.HasKey("Tetrax_Min")) _minCube = Mathf.RoundToInt(PlayerPrefs.GetFloat("Tetrax_Min"));
            else PlayerPrefs.SetFloat("Tetrax_Min", _minCube);

            StartCoroutine(GameLoop());
        }

        private CubeData GetCubeData(TeamColor color, bool isSpe)
        {
            return CubesDatas.ToList().Find(x => x.IsSpe == isSpe && x.Color == color);
        }

        private GameObject SpawnCube(CubeData data, Transform parent, Vector2 pos)
        {
            var cube = Instantiate(_cubePref, parent);
            cube.transform.localPosition = pos;
            var anim = cube.GetComponent<Animator>();
            anim.runtimeAnimatorController = data.Animator;
            var behaviour = cube.GetComponent<CubeBehaviour>();
            behaviour.Data = data;
            return cube;
        }

        IEnumerator GameLoop()
        {
            yield return null;

            int speed = 0;
            while (!IsGameOver)
            {
                foreach (var t in Teams)
                {
                    if (t.IsGameOver) continue;
                    foreach (var cubes in t.CubesList)
                    {
                        if(cubes != null)
                        {
                            cubes.transform.DOLocalMoveY(cubes.transform.localPosition.y - 135, 0.15f);
                            //cubes.transform.localPosition += new Vector3(0, -135);
                        }
                    }
                }

                //for (int i = 0; i < _nbCube; i++)
                //{
                //    if(Random.Range(0, 100) < _chanceToSpawn)
                //    {
                //        var pos = new Vector2(-410 + (135 * i), -70);
                //        bool isSpe = Random.Range(0, 100) < _chanceToSpawnSpeCube;
                //        foreach (var t in Teams)
                //        {
                //            if (t.IsGameOver) continue;
                //            var cube = SpawnCube(GetCubeData(t.Color, isSpe), t.SpawnCubeHolder, pos);
                //            t.CubesList.Add(cube);
                //        }
                //    }
                //}

                var rnd = Random.Range(_minCube, _maxCube + 1);
                var rng = new List<int>() { 0, 1, 2, 3, 4 };
                for (int i = 0; i < rnd; i++)
                {
                    var n = rng.RandomElement();
                    rng.Remove(n);
                    var pos = new Vector2(-410 + (135 * n), -70);
                    bool isSpe = Random.Range(0, 100) < _chanceToSpawnSpeCube;
                    foreach (var t in Teams)
                    {
                        if (t.IsGameOver) continue;
                        var cube = SpawnCube(GetCubeData(t.Color, isSpe), t.SpawnCubeHolder, pos);
                        t.CubesList.Add(cube);
                    }
                }

                foreach (var t in Teams)
                {
                    if (t.CubesList.Any(x => x.transform.localPosition.y <= -1015))
                    {
                        if(Teams.ToList().Any(t => t.IsGameOver) && !Teams.ToList().TrueForAll(t => t.IsGameOver) && !t.IsGameOver)
                        {
                            t.Source.clip = _victory;
                            t.Source.Play();
                        }
                    }
                }

                bool anyLoose = Teams.ToList().Any(t => t.IsGameOver);

                foreach (var t in Teams)
                {
                    if (t.CubesList.Any(x => x.transform.localPosition.y <= -1015) && !t.IsGameOver)
                    {
                        Debug.Log("Loose");
                        t.IsGameOver = true;
                        continue;
                    }
                }

                if (!anyLoose && !Teams.ToList().TrueForAll(t => t.IsGameOver) && Teams.ToList().Any(t => t.IsGameOver))
                {
                    var t = Teams.ToList().Find(x => x.IsGameOver);
                    t.Source.clip = _failure;
                    t.Source.Play();
                }
                else if (Teams.ToList().TrueForAll(t => t.IsGameOver) && !anyLoose)
                {
                    foreach (var team in Teams)
                    {
                        bool win = Teams.ToList().Max(x => x.Score) == team.Score;
                        team.Source.clip = win ? _victory : null;
                        team.Source.Play();
                    }
                }

                if (Teams.ToList().TrueForAll(t => t.IsGameOver))
                {
                    IsGameOver = true;
                    WinnerScore = Teams.ToList().Max(x => x.Score);
                    foreach (var team in Teams)
                    {
                        bool win = Teams.ToList().Max(x => x.Score) == team.Score;
                        team.EndText.text = win ? "Vous avez gagne" : "Vous avez perdu";
                        team.EndText.transform.parent.gameObject.SetActive(true);
                    }

                    yield return new WaitForSeconds(5);

                    GameManager.Instance.OSCManager.NeedName();
                    GameManager.CurrentGameSceneObject.PlayScore();
                    break;
                }

                yield return new WaitForSeconds(Mathf.Lerp(_tick, 0.5f, Mathf.Clamp01(speed / 100f)));
                speed += _acceleration;
            }
        }
    }
}