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
using System.Linq;
using System.IO;
using GlobalOutline;

public class BlockSceneObject : GameSceneObject
{
    public GameBlock GameBlockSo => _gameBlockSo;
    [SerializeField]
    GameBlock _gameBlockSo;

    #region HomePage

    public GameObject HomePage => _homePage;
    [SerializeField, Header("HomePage")]
    GameObject _homePage;

    async Task HomeAwake()
    {

    }

    async Task HomeStart()
    {

    }

    void HomeUpdate()
    {

    }

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

    Texture2D[] Tex = new Texture2D[3];

    List<int[,]> RotationsSave;


    async Task GameAwake()
    {
        RotationsSave = new()
        {
            null,
            null,
            null
        };
    }

    async Task GameInit()
    {
        string path = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\..\..\..\"));
        path = Path.GetFullPath(Path.Combine(path, GameBlockSo.ImagePath));
        List<string> imagePath = ToolBox.GetFiles(path, "*.jpg");
        imagePath.Shuffle();

        for (int i = 0; i < 3; i++)
        {
            Tex[i] = await ToolBox.CreateTextureFromPath(imagePath[i]);
        }

        await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
        {
            foreach (var t in Teams)
            {
                t.ImageSplitList?.Clear();
                t.ImageSplitList = new();

                for (int i = 0; i < Tex.Length; i++)
                {
                    t.ImageSplitList.Add(SplitImage(Tex[i], GameBlockSo.NbDivision[i], t.ID));
                    t.DeActiveAllMark();
                    t.ImageCheckMarks[i].sprite = ToolBox.CreateSpriteFromTexture(Tex[i]);

                }
            }
            Debug.Log("Finish Load Image");
        });
    }

    async Task GameStart()
    {
        //foreach (var t in Teams)
        //{
        //    //t.SetAllText();
        //    t.Win += TeamWin;
        //}
        _currentTimer = _initialTimer;
        SetTimerText(true);
    }

    void GameUpdate()
    {
        
    }

    public async void PlayGame()
    {
        Task[] tasks = new Task[3];
        ExecutionQueue teamAQueue = GameManager.Instance.TasksManager.CreateComplexTaskQueue("TeamA");
        ExecutionQueue teamBQueue = GameManager.Instance.TasksManager.CreateComplexTaskQueue("TeamB");

        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            teamAQueue.Run(() => PlayOneImage(Tex[0], 0, 0, false));
            teamAQueue.Run(() => PlayOneImage(Tex[1], 0, 1));
            teamAQueue.Run(() => PlayOneImage(Tex[2], 0, 2));
            teamAQueue.Complete();

            teamBQueue.Run(() => PlayOneImage(Tex[0], 1, 0, false));
            teamBQueue.Run(() => PlayOneImage(Tex[1], 1, 1));
            teamBQueue.Run(() => PlayOneImage(Tex[2], 1, 2));
            teamBQueue.Complete();
        });

        Task task = CheckShuffle();

        await GameManager.Instance.AddressablesManager.LoadScreen(task);

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

        //Go to Score
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
            await Task.Delay(50);
    }

    public async void FinishPlayGame(int teamID)
    {
        TimerEnd -= FinishPlayGame;
        _timerTokenSource.Dispose();
        foreach (var t in Teams)
            t.DeActiveAllButton();

        if (teamID < 0)
        {
            //Oups Egalité

            Debug.Log("Egalité !");
            return;
        }

        //await DecreaseTimer();

        await Task.Delay(2000);

        //Go to score

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
            SetTimerText();
            await Task.Delay(100);
        }
        TimerEnd?.Invoke(-1);
    }

    void SetTimerText(bool zero = false)
    {
        if (zero)
        {
            _timerTextFront.text = "0:000";
            _timerTextBack.text = "0:000";
            return;
        }

        TimeSpan timer = (DateTime.Now - _startTimer);

        int second = Mathf.FloorToInt((float)timer.TotalSeconds);
        second = second < 0 ? 0 : second;

        int millisecond = Mathf.FloorToInt(timer.Milliseconds);
        millisecond = millisecond < 0 ? 0 : millisecond;

        _timerTextFront.text = $"{second}:{millisecond}";
        _timerTextBack.text = $"{second}:{millisecond}";
    }

    //void ShuffleParts()
    //{
    //    for (int i = 0; i < Teams[0].Parts.Count; i++)
    //    {
    //        int rot = GetRandomRotation();
    //        Teams[0].Parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
    //        Teams[1].Parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
    //    }
    //}

    //int GetRandomRotation()
    //{
    //    int rot = Random.Range(0, 3);
    //    switch (rot)
    //    {
    //        case 0:
    //            return 90;
    //        case 1:
    //            return 180;
    //        case 2:
    //            return 270;
    //        default:
    //            return -1;
    //    }
    //}

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

    public Sprite[,] SplitImage(Texture2D texture, int nbDivision, int teamID)
    {
        GameObject parent = Teams[teamID].ImageHolder;
        Sprite[,] sprites = new Sprite[nbDivision, nbDivision];

        for (int i = 0; i < nbDivision; i++)
        {
            for (int j = 0; j < nbDivision; j++)
            {
                float h = texture.height / nbDivision;
                float w = texture.width / nbDivision;
                sprites[i, j] = Sprite.Create(texture, new Rect(i * w, j * h, w, h), new Vector2(0.5f, 0.5f));
            }
        }
        Teams[teamID].ImageLoaded = true;
        return sprites;
    }

    #endregion

    #endregion

    #region ScorePage

    public GameObject ScorePage => _scorePage;
    [SerializeField, Header("ScorePage")]
    GameObject _scorePage;

    async Task ScoreAwake()
    {

    }

    async Task ScoreStart()
    {

    }

    void ScoreUpdate()
    {

    }

    #endregion

    #region Unity Func

    public override async void Awake()
    {
        GameManager.Instance.CurrentGame = GameBlockSo;
        base.Awake();
        await HomeAwake();
        await GameAwake();
        await ScoreAwake();
        HomePage.SetActive(true);
        GamePage.SetActive(false);
        ScorePage.SetActive(false);
    }

    public async override Task InitScene()
    {
        await GameInit();
        await base.InitScene();
    }

    public override void Play()
    {
        PlayGame();
        base.Play();
    }

    private async void Start()
    {
        await HomeStart();
        await GameStart();
        await ScoreStart();
    }

    private void Update()
    {
        HomeUpdate();
        GameUpdate();
        ScoreUpdate();
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
                if(Parts == null)
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
                if(part.transform.rotation == Quaternion.Euler(0, 0, 0))
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

        int GetLimitThrow(float pourcent) =>  Mathf.RoundToInt(2 * Mathf.CeilToInt(Parts.Length * pourcent));

        int GetRandomRotation()
        {
            int rot = Random.Range(0, 3);
            switch (rot)
            {
                case 0:
                    return 90;
                case 1:
                    return 180;
                case 2:
                    return 270;
                default:
                    return -1;
            }
        }

        int GetNbThrowWithRotation(int rot)
        {
            switch (rot)
            {
                case 90:
                    return 1;
                case 180:
                    return 2;
                case 270:
                    return 3;
            }
            return -1;
        }

        int GetIDWithPourcent(float rot)
        {
            switch (rot)
            {
                case 0.25f:
                    return 0;
                case 0.5f:
                    return 1;
                case 0.75f:
                    return 2;
            }
            return -1;
        }

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
                if(part.transform.rotation.eulerAngles != new Vector3(0, 0, 0))
                    return false;
            }
            return true;
        }

        public void DestroyAllParts()
        {
            foreach (var part in PartsList)
                if(part != null)
                    Destroy(part.gameObject);
            Parts = new ButtonPartRotation[0,0];
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