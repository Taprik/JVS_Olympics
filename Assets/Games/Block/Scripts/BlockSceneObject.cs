using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using Tool;
using System.Threading.Tasks;
using UnityEngine.UI;
using System;
using TMPro;
using System.Threading;
using DG.Tweening;
using GlobalOutline;

namespace Blocks
{
    public class BlockSceneObject : GameSceneObject
    {
        public GameBlock GameBlockSo => _gameBlockSo;
        [SerializeField]
        GameBlock _gameBlockSo;

        #region HomePage

        public GameObject HomePage => _homePage;
        [SerializeField, Header("HomePage")]
        GameObject _homePage;

        #endregion

        #region GamePage

        public GameObject GamePage => _gamePage;
        [SerializeField, Header("GamePage")]
        GameObject _gamePage;

        [SerializeField]
        float _initialTimer;
        float _currentTimer;
        DateTime _startTimer;

        [SerializeField]
        TextMeshProUGUI _timerTextFront;
        [SerializeField]
        TextMeshProUGUI _timerTextBack;

        Action<int> TimerEnd;
        CancellationTokenSource _timerTokenSource;

        public List<BlockTeam> Teams;
        List<ImageData> _imagesDatas = new();

        List<int[,]> RotationsSave = new()
        {
            null,
            null,
            null
        };

        Task GameInit()
        {
            _imagesDatas.Clear();
            var list = GameBlockSo.ImageDatas;
            list.Shuffle();
            for (int i = 0; i < 3; i++)
            {
                if (i >= list.Count) break;
                ImageData data = list[i];
                _imagesDatas.Add(data);
            }

            foreach (var t in Teams)
            {
                t.ImageSplitList?.Clear();
                t.ImageSplitList = new();
                t.DeActiveAllMark();

                for (int i = 0; i < _imagesDatas.Count; i++)
                {
                    Sprite[,] array = _imagesDatas[i].ImageSplit[i];
                    t.ImageSplitList.Add(array);
                    t.ImageCheckMarks[i].sprite = ToolBox.CreateSpriteFromTexture(_imagesDatas[i].Texture);

                }
                Teams[t.ID].ImageLoaded = true;
            }
            Debug.Log("Finish Load Image");
            return Task.CompletedTask;
        }

        void GameStart()
        {
            RotationsSave = new()
            {
                null,
                null,
                null
            };

            _currentTimer = _initialTimer;
            SetTimerText(true);
        }

        public async void PlayGame()
        {
            GameStart();

            GamePage.SetActive(true);
            HomePage.SetActive(false);

            Task[] tasks = new Task[3];
            ExecutionQueue teamAQueue = GameManager.Instance.TasksManager.CreateComplexTaskQueue("TeamA");
            ExecutionQueue teamBQueue = GameManager.Instance.TasksManager.CreateComplexTaskQueue("TeamB");

            UnityMainThreadDispatcher.Instance().Enqueue(() =>
            {
                teamAQueue.Run(() => PlayOneImage(_imagesDatas[0].Texture, 0, 0, false));
                teamAQueue.Run(() => PlayOneImage(_imagesDatas[1].Texture, 0, 1));
                teamAQueue.Run(() => PlayOneImage(_imagesDatas[2].Texture, 0, 2));
                teamAQueue.Complete();

                teamBQueue.Run(() => PlayOneImage(_imagesDatas[0].Texture, 1, 0, false));
                teamBQueue.Run(() => PlayOneImage(_imagesDatas[1].Texture, 1, 1));
                teamBQueue.Run(() => PlayOneImage(_imagesDatas[2].Texture, 1, 2));
                teamBQueue.Complete();
            });

            Task task = CheckShuffle();

            while (!task.IsCompleted)
            {
                await Task.Delay(5);
            }

            Task[] CountDowns = new Task[2];
            CountDowns[0] = Teams[0].CountDown();
            CountDowns[1] = Teams[1].CountDown();
            await Task.WhenAll(CountDowns);

            foreach (var t in Teams)
            {
                t.ActiveAllButton();
                await Task.Delay(1);
                t.ShufflePart(ref RotationsSave, 0.25f);
            }

            tasks[0] = teamAQueue.Completion;
            tasks[1] = teamBQueue.Completion;

            TimerEnd += FinishPlayGame;
            _timerTokenSource = new CancellationTokenSource();
            Task timer = Timer(_timerTokenSource.Token);
            tasks[2] = timer;

            await Task.WhenAny(tasks);

            AudioSource.PlayClipAtPoint(GameBlockSo.AudioWin, Vector3.zero);
        }

        async Task PlayOneImage(Texture2D texture, int teamID, int id, bool notFirst = true)
        {
            UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].DestroyAllParts());
            await UnityMainThreadDispatcher.Instance().EnqueueAsync(() => LoadImage(texture, Teams[teamID].ImageSplitList[id], GameBlockSo.NbDivision[id], teamID));


            if (notFirst)
            {
                await Teams[teamID].CountDown();
                UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].ActiveAllButton());
                UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].ShufflePart(ref RotationsSave, (id * 0.25f + 0.25f)));
                if (id == 2)
                    Teams[teamID].Win += TeamWin;
            }

            bool imageFinish = false;

            Teams[teamID].Win += (n) => { imageFinish = true; };

            await Task.Run(async () =>
            {
                while (!imageFinish)
                    await Task.Delay(50);
            });

            UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].ActiveMark(id));
        }

        async Task CheckShuffle()
        {
            while ((Teams[0].ImageLoaded && Teams[1].ImageLoaded) == false)
                await Task.Delay(5);
        }

        public async void FinishPlayGame(int teamID)
        {
            TimerEnd -= FinishPlayGame;
            _timerTokenSource.Dispose();
            foreach (var t in Teams)
                t.DeActiveAllButton();

            if (teamID < 0)
            {
                Debug.LogError("Egalité ?!");
                return;
            }

            finalTimer = (DateTime.Now - _startTimer);
            winningTeam = Teams[teamID];

            await Task.Delay(2000);

            PlayScore();

            Debug.Log(Teams[teamID].Name + " Win !");
        }

        void TeamWin(int teamID)
        {
            foreach (var t in Teams)
            {
                t.Win -= TeamWin;
            }
            _timerTokenSource.Cancel();
            TimerEnd?.Invoke(teamID);
        }

        async Task Timer(CancellationToken token)
        {
            _startTimer = DateTime.Now;
            while (_currentTimer > 0)
            {
                if (token.IsCancellationRequested)
                    return;

                _currentTimer -= 0.1f;
                UnityMainThreadDispatcher.Instance().Enqueue(() => SetTimerText());
                await Task.Delay(100);
            }
            TimerEnd?.Invoke(-1);
        }

        void SetTimerText(bool zero = false)
        {
            if (zero)
            {
                _timerTextFront.text = "00:00";
                _timerTextBack.text = "00:00";
                return;
            }

            TimeSpan timer = (DateTime.Now - _startTimer);

            int second = Mathf.FloorToInt((float)timer.TotalSeconds);
            second = second < 0 ? 0 : second;

            string format = timer.ToString(@"ff");

            _timerTextFront.text = $"{second}:{format}";
            _timerTextBack.text = $"{second}:{format}";
        }

        #region SplitImage

        [SerializeField]
        GameObject _partPrefab;

        [SerializeField]
        Vector3 _scaleImage;

        //public async Task LoadImage(string path, int nbDivision, int teamID)
        //{
        //    GameObject parent = Teams[teamID].ImageHolder;
        //    Texture2D texture = await ToolBox.CreateTextureFromPath(path);
        //    Teams[teamID].Parts = new ButtonPartRotation[nbDivision, nbDivision];

        //    for (int i = 0; i < nbDivision; i++)
        //    {
        //        for (int j = 0; j < nbDivision; j++)
        //        {
        //            float h = texture.height / nbDivision;
        //            float w = texture.width / nbDivision;
        //            Sprite newSprite = Sprite.Create(texture, new Rect(i * w, j * h, w, h), new Vector2(0.5f, 0.5f));
        //            GameObject n = Instantiate(_partPrefab, parent.transform);
        //            Image sr = n.GetComponent<Image>();
        //            RectTransform rt = n.GetComponent<RectTransform>();
        //            rt.sizeDelta = new Vector2((parent.transform as RectTransform).rect.width / nbDivision, (parent.transform as RectTransform).rect.height / nbDivision);
        //            sr.sprite = newSprite;
        //            float imageWidth = sr.rectTransform.sizeDelta.x;
        //            float imageHeight = sr.rectTransform.sizeDelta.y;
        //            (n.transform as RectTransform).localPosition = new Vector3(
        //                i * imageWidth/* * _scaleImage.x */+ imageWidth/* * _scaleImage.x *// 2 - (parent.transform as RectTransform).rect.width / 2, 
        //                j * imageHeight/* * _scaleImage.y */+ imageHeight/* * _scaleImage.y *// 2 - (parent.transform as RectTransform).rect.height / 2,
        //                0);
        //            sr.transform.localScale = _scaleImage;
        //            Teams[teamID].AddPart(n, i, j);
        //        }
        //    }
        //}

        public void LoadImage(Texture2D texture, Sprite[,] sprites, int nbDivision, int teamID)
        {
            GameObject parent = Teams[teamID].ImageHolder;
            Teams[teamID].Parts = new ButtonPartRotation[nbDivision, nbDivision];

            for (int i = 0; i < nbDivision; i++)
            {
                for (int j = 0; j < nbDivision; j++)
                {
                    float h = texture.height / nbDivision;
                    float w = texture.width / nbDivision;
                    Sprite newSprite = sprites[i, j];
                    GameObject n = Instantiate(_partPrefab, parent.transform);
                    Image sr = n.GetComponent<Image>();
                    RectTransform rt = n.GetComponent<RectTransform>();
                    rt.sizeDelta = new Vector2((parent.transform as RectTransform).rect.width / nbDivision, (parent.transform as RectTransform).rect.height / nbDivision);
                    sr.sprite = newSprite;
                    float imageWidth = sr.rectTransform.sizeDelta.x;
                    float imageHeight = sr.rectTransform.sizeDelta.y;
                    (n.transform as RectTransform).localPosition = new Vector3(
                        i * imageWidth/* * _scaleImage.x */+ imageWidth/* * _scaleImage.x *// 2 - (parent.transform as RectTransform).rect.width / 2,
                        j * imageHeight/* * _scaleImage.y */+ imageHeight/* * _scaleImage.y *// 2 - (parent.transform as RectTransform).rect.height / 2,
                        0);
                    sr.transform.localScale = _scaleImage;
                    //childRt.transform.localScale = _scaleImage;
                    Teams[teamID].AddPart(n, i, j);
                }
            }
            Teams[teamID].ImageLoaded = true;
        }

        #endregion

        #endregion

        #region ScorePage

        public GameObject ScorePage => _scorePage;
        [SerializeField, Header("ScorePage")]
        GameObject _scorePage;

        [SerializeField]
        ScoreBoardDisplayer _scoreBoardDisplayer;

        [SerializeField]
        GameObject _finalScoreObject;

        [SerializeField]
        TextMeshProUGUI _finalScoreFrontText;

        [SerializeField]
        TextMeshProUGUI _finalScoreBackText;


        BlockTeam winningTeam;
        TimeSpan finalTimer;

        private void PlayScore()
        {
            GamePage.SetActive(false);
            ScorePage.SetActive(true);
            _scoreBoardDisplayer.gameObject.SetActive(false);
            _finalScoreObject.SetActive(false);

            string text = "Vous avez fini en " + Mathf.Clamp(Mathf.FloorToInt((float)finalTimer.TotalSeconds), 0, float.PositiveInfinity) + "," + finalTimer.ToString(@"ff") + " secondes";
            _finalScoreFrontText.text = text;
            _finalScoreBackText.text = text;

            _finalScoreObject.SetActive(true);
            GameManager.Instance.OSCManager.NeedName();
        }

        public async override void OnNameReceive(string name)
        {
            PlayerData playerData = new()
            {
                Name = name,
                Score = Mathf.RoundToInt((float)finalTimer.TotalSeconds * 100) / 100f
            };

            _scoreBoardDisplayer.InitScoreBoard(await GameManager.Instance.ScoreBoardManager.UpdateScoreBoardAscendingOrder(playerData, GameScoreBoard.BlockScoreBoard), () => GameBlockSo.GetWinFont(winningTeam.ID));
        }

        public override void PageUp()
        {
            base.PageUp();
            _scoreBoardDisplayer.PageUp();
        }

        public override void PageDown()
        {
            base.PageDown();
            _scoreBoardDisplayer.PageDown();
        }


        #endregion

        #region Unity Func

        public override void Awake()
        {
            GameManager.Instance.CurrentGame = GameBlockSo;
            base.Awake();
            HomePage.SetActive(true);
            GamePage.SetActive(false);
            ScorePage.SetActive(false);
        }

        public async override Task InitScene()
        {
            await GameInit();
            await base.InitScene();
        }

        public async override void Play()
        {
            if (ScorePage.activeSelf)
                await Replay();

            base.Play();
            PlayGame();
        }

        public async override Task Replay()
        {
            ScorePage.SetActive(false);
            await GameManager.Instance.AddressablesManager.LoadScreen(GameInit());
        }

        #endregion

        [System.Serializable]
        public class BlockTeam
        {
            public string Name;
            public int ID;
            public int Score;
            public Color Color;

            public GameObject ImageHolder;
            public List<Sprite[,]> ImageSplitList;
            public ButtonPartRotation[,] Parts;
            public List<ButtonPartRotation> PartsList { get
                {
                    if (Parts == null)
                        return new List<ButtonPartRotation>();
                    List<ButtonPartRotation> list = new List<ButtonPartRotation>();
                    for (int x = 0; x < Parts.GetLength(0); x++)
                    {
                        for (int y = 0; y < Parts.GetLength(1); y++)
                        {
                            list.Add(Parts[x, y]);
                        }
                    }
                    return list;
                }
            }

            private bool _imageLoaded;
            public bool ImageLoaded { get
                {
                    return _imageLoaded && Parts.Length > 0;
                }
                set
                {
                    _imageLoaded = value;
                }
            }
            public bool IsShuffle => !CheckAllPartsRotation();
            public Image[] ImageCheckMarks;
            public GameObject[] CheckMarks;
            public Image FadeCountDown;
            public TextMeshProUGUI CountDownTextFront;
            public TextMeshProUGUI CountDownTextBack;

            public Action<int> Win;

            public async Task CountDown()
            {
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FadeCountDown.color = new Color(0, 0, 0, 0);
                    CountDownTextFront.text = string.Empty;
                    CountDownTextBack.text = string.Empty;
                    FadeCountDown.DOFade(0.5f, 0.5f);
                });
                await Task.Delay(500);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FadeCountDown.DOFade(0.45f, 1f);
                    CountDownTextFront.text = "3";
                    CountDownTextBack.text = "3";
                });
                await Task.Delay(1000);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FadeCountDown.DOFade(0.4f, 1f);
                    CountDownTextFront.text = "2";
                    CountDownTextBack.text = "2";
                });
                await Task.Delay(1000);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FadeCountDown.DOFade(0.35f, 1f);
                    CountDownTextFront.text = "1";
                    CountDownTextBack.text = "1";
                });
                await Task.Delay(1000);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    FadeCountDown.DOFade(0f, 0.5f);
                    CountDownTextFront.text = "GO";
                    CountDownTextBack.text = "GO";
                });
                await Task.Delay(500);
                UnityMainThreadDispatcher.Instance().Enqueue(() =>
                {
                    CountDownTextFront.text = string.Empty;
                    CountDownTextBack.text = string.Empty;
                    FadeCountDown.color = new Color(0, 0, 0, 0);
                });
            }

            public void AddPart(GameObject part, int x, int y)
            {
                part.TryGetComponent(out ButtonPartRotation buttonPart);
                GameManager.Instance.OutlineManager.AddGameObject(part);
                OutlineEffect outline = part.GetComponent<OutlineEffect>();
                if (buttonPart != null && outline != null)
                {
                    buttonPart.Rotate += PartRotate;
                    buttonPart.OnActiveChange += (isActive) => outline.enabled = isActive;
                    buttonPart.IsActive = false;
                    Parts[x, y] = buttonPart;
                }
                else
                    Debug.LogError("L'image n'a pas de ButtonPartRotation.");
            }

            public void ActiveAllButton()
            {
                foreach (var b in Parts)
                    b.IsActive = true;
            }

            public void DeActiveAllButton()
            {
                foreach (var b in Parts)
                    b.IsActive = false;
            }

            void PartRotate()
            {
                //Debug.Log(CheckAllPartsRotation());
                if (CheckAllPartsRotation())
                {
                    Win?.Invoke(ID);
                }
            }

            public void ShufflePart(ref List<int[,]> rotationsSave, float pourcent = 1f)
            {
                int id = GetIDWithPourcent(pourcent);
                Debug.Log(id + " | " + pourcent);
                if (rotationsSave[id] != null)
                {
                    int l = (GameManager.Instance.CurrentGame as GameBlock).NbDivision[id];
                    for (int x = 0; x < l; x++)
                    {
                        for (int y = 0; y < l; y++)
                        {
                            Parts[x, y].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rotationsSave[id][x, y]));
                            //Debug.Log(x + ";" + y);
                        }
                    }
                }
                else
                {
                    rotationsSave[id] = SufflePartRandomly(id, pourcent);
                }

                foreach (var part in PartsList)
                    if (part.transform.rotation == Quaternion.Euler(0, 0, 0))
                        part.IsActive = false;
            }

            int[,] SufflePartRandomly(int id, float pourcent = 1f)
            {
                int total = 0;
                List<int> rots = new();

                while (total != GetLimitThrow(pourcent))
                {
                    rots = new();
                    total = 0;
                    for (int i = 0; i < Mathf.CeilToInt(Parts.Length * pourcent); i++)
                    {
                        int rot = GetRandomRotation();
                        rots.Add(rot);
                        total += GetNbThrowWithRotation(rot);
                    }
                }

                List<ButtonPartRotation> parts = PartsList;
                parts.Shuffle();
                for (int i = 0; i < rots.Count; i++)
                {
                    parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rots[i]));
                }
                return ConvertPartToTable(Parts, (GameManager.Instance.CurrentGame as GameBlock).NbDivision[id]);
            }

            int GetLimitThrow(float pourcent) => Mathf.RoundToInt(2 * Mathf.CeilToInt(Parts.Length * pourcent));

            int GetRandomRotation() => Random.Range(1, 4) * 90;

            int GetNbThrowWithRotation(int rot) => rot / 90;

            int GetIDWithPourcent(float pourcent) => Mathf.RoundToInt((pourcent - 0.25f) * 4);

            int[,] ConvertPartToTable(ButtonPartRotation[,] parts, int l)
            {
                int[,] table = new int[l, l];
                for (int x = 0; x < l; x++)
                {
                    for (int y = 0; y < l; y++)
                    {
                        table[x, y] = Mathf.RoundToInt(parts[x, y].gameObject.transform.rotation.eulerAngles.z);
                        //Debug.Log(x + ";" + y);
                    }
                }
                return table;
            }

            public bool CheckAllPartsRotation()
            {
                foreach (var part in Parts)
                {
                    if (part.transform.rotation.eulerAngles != new Vector3(0, 0, 0))
                        return false;
                }
                return true;
            }

            public void DestroyAllParts()
            {
                foreach (var part in PartsList)
                    if (part != null)
                        Destroy(part.gameObject);
                Parts = new ButtonPartRotation[0, 0];
                ImageLoaded = false;
            }

            public void ActiveMark(int id)
            {
                CheckMarks[id].SetActive(true);
            }

            public void DeActiveAllMark()
            {
                foreach (var c in CheckMarks)
                {
                    c.SetActive(false);
                }
            }
        }
    }
}