using Cinemachine;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using Tool;
using UnityEngine;
using UnityEngine.UI;

namespace Basket
{
    [System.Serializable]
    public class BasketTeam
    {
        public string Name;
        public int Score;
        public TextMeshProUGUI ScoreText;
        public NetSensor Net;
        public Cloth NetCloth;
        public ThrowSensor ThrowManager;
        public ShowScoreOnHoop ScoreDisplay;
        public Transform CamPoint;
        public Transform RotateCamPoint;
        public CinemachineVirtualCamera Cam;
        public GameObject Target;
        public Image Ball;
        public bool Next { get; set; } = false;
        public List<GameObject> AllBalls { get; private set; } = new List<GameObject>();
        public float Multiplicator { get; set; } = 1f;
    }

    public class Basket_GameManager : MonoBehaviour
    {
        public static Basket_GameManager i;
        private void Awake()
        {
            if (i != null)
            {
                Destroy(gameObject);
                return;
            }

            i = this;

            if(!PlayerPrefs.HasKey("Basket_Name1"))
                PlayerPrefs.SetString("Basket_Name1", "Joueur 1");

            if(!PlayerPrefs.HasKey("Basket_Name2"))
                PlayerPrefs.SetString("Basket_Name2", "Joueur 2");


            GameManager.OnGameStart += OnGameStart;
        }

        private void OnDestroy()
        {
            GameManager.OnGameStart -= OnGameStart;
        }

        public BasketTeam[] Teams => _teams;
        [SerializeField] BasketTeam[] _teams = new BasketTeam[2];

        [SerializeField] TextMeshProUGUI _winnerText;
        [SerializeField] AudioSource _source;

        public bool IsGameOver { get; private set; } = false;

        private void OnGameStart()
        {
            StartCoroutine(GameStart());
        }

        IEnumerator GameStart()
        {
            foreach (var team in Teams)
            {
                team.ThrowManager.CanShot = false;
                var pos = team.CamPoint.position;
                pos.y = team.Cam.transform.position.y;
                team.Cam.transform.position = pos;
                team.Score = 0;
                team.ScoreDisplay.DisplayScore(team.Score);
                team.ScoreText.text = "00tps";
            }

            yield return new WaitForSeconds(0.2f);

            IsGameOver = false;

            foreach (var team in Teams)
            {
                IEnumerator Transition()
                {
                    team.Target.SetActive(false);
                    team.Ball.DOColor(new Color(0.52f, 0.52f, 0.52f, 0.94f), 0.25f);
                    team.Ball.transform.DOScale(0.75f, 0.125f);
                    team.ThrowManager.CanShot = false;

                    int[] rnd =
                    {
                            -3,
                            -2,
                            -1,
                            0,
                            1,
                            2,
                            3
                        };

                    float[] rndup =
                    {
                            0.5f,
                            1,
                            1.5f,
                            2,
                            2.5f,
                            3
                        };

                    float[] rng =
                    {
                            6f,
                            6.5f,
                            7f,
                            7.5f,
                            8f
                        };

                    var multiplicator = rng[Random.Range(0, rng.Length)] * (1 + (.5f * PlayerPrefs.GetInt(Basket_GeneralVariable.DifficultyKey)));
                    team.Multiplicator = multiplicator / 6f;

                    team.RotateCamPoint.localRotation = Quaternion.Euler(0f, rnd[Random.Range(0, rnd.Length)] * 15f, 0f);
                    team.RotateCamPoint.GetChild(0).localPosition = new Vector3(0, Mathf.Abs(rndup.RandomElement()), multiplicator * -Mathf.Sign(team.RotateCamPoint.position.z));

                    yield return MoveCameraToPoint(team.Cam.transform, team.CamPoint.position);

                    team.Next = false;
                    team.ThrowManager.CanShot = true;

                    if (team.Score < 4)
                    {
                        team.Target.SetActive(true);
                        team.Target.transform.DOScale(1f / team.Multiplicator, 0f);
                    }

                    team.Ball.DOColor(Color.white, 0.25f);
                    team.Ball.transform.DOScale(1.25f, 0.125f);

                    //Debug.Log(team.Ball.color);
                }

                team.Net._event.AddListener(() =>
                {
                    StartCoroutine(Transition());
                });

                StartCoroutine(Transition());
            }

            Basket_TimerManager.i.OnTimerEnd.AddListener(() =>
            {
                IEnumerator Finish()
                {
                    IsGameOver = true;

                    foreach (var t in Teams)
                    {
                        t.Target.SetActive(false);
                        t.Ball.DOColor(new Color(0.52f, 0.52f, 0.52f, 0.94f), 0.5f);
                        t.Ball.transform.DOScale(0.75f, 0.25f);
                    }

                    if (Teams[0].Score != Teams[1].Score)
                        _winnerText.text = $"{PlayerPrefs.GetString(Teams[0].Score > Teams[1].Score ? "Basket_Name1" : "Basket_Name2")} a gagne !";
                    else
                        _winnerText.text = $"Egalite, personne n'a su se demarquer.";

                    _winnerText.transform.parent.gameObject.SetActive(true);

                    _source.clip = Basket_SoundManager.i._end[Random.Range(0, Basket_SoundManager.i._end.Length)];
                    _source.Play();

                    yield return new WaitForSeconds(2f);
                    yield return new WaitUntil(() => !_source.isPlaying);

                    PlayerPrefs.SetFloat(Basket_GeneralVariable.HighScoreKey, Mathf.Max(Teams[0].Score, Teams[1].Score));
                    //SceneManager.LoadScene(Basket_GeneralVariable.i.scoreScene);
                    _winnerText.transform.parent.gameObject.SetActive(false);
                    GameManager.Instance.OSCManager.NeedName();
                    (GameManager.CurrentGameSceneObject as BasketSceneObject).PlayScore();
                }

                StartCoroutine(Finish());
            });
        }

        public IEnumerator MoveCameraToPoint(Transform cam, Vector3 end)
        {
            //end.y = cam.position.y;
            yield return cam.DOMove(end, 0.5f).WaitForCompletion();
        }

        public IEnumerator LookAtBall(CinemachineVirtualCamera cam, Transform ball, bool team)
        {
            var t = Teams[team ? 1 : 0];
            //cam.LookAt = ball;
            //cam.gameObject.SetActive(true);
            yield return new WaitUntil(() => ball == null || t.Next);
            //cam.gameObject.SetActive(false);

            if (t.Next)
            {
                t.ThrowManager.CanShot = false;
                t.Net.IsActive = false;
                yield return new WaitForSeconds(0.5f);
                for (int i = 0; i < t.AllBalls.Count; i++)
                {
                    if (t.AllBalls[i] != null) Destroy(t.AllBalls[i]);
                }
                t.Net.IsActive = true;
            }
            else
            {
                if (t.Score < 4) t.Target.SetActive(true);
                t.Ball.DOColor(Color.white, 0.5f);
                t.Ball.transform.DOScale(1.25f, 0.25f);
            }
        }
    }
}