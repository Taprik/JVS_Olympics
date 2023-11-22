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
    
    [SerializeField]
    TextMeshProUGUI _timerTextFront;
    [SerializeField]
    TextMeshProUGUI _timerTextBack;

    Action<int> TimerEnd;
    CancellationTokenSource _timerTokenSource;

    public List<BlockTeam> Teams;

    Texture2D[] Tex = new Texture2D[3]; 


    async Task GameAwake()
    {

    }

    async Task GameStart()
    {
        //foreach (var t in Teams)
        //{
        //    //t.SetAllText();
        //    t.Win += TeamWin;
        //}
        _currentTimer = _initialTimer;
        SetTimerText();
    }

    void GameUpdate()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            PlayGame();
        }
    }

    public async void PlayGame()
    {
        Task[] tasks = new Task[3];
        ExecutionQueue teamAQueue = GameManager.Instance.TasksManager.CreateComplexTaskQueue("TeamA");
        ExecutionQueue teamBQueue = GameManager.Instance.TasksManager.CreateComplexTaskQueue("TeamB");

        UnityMainThreadDispatcher.Instance().EnqueueAsync(async () =>
        {
            List<string> imagePath = ToolBox.GetFiles(GameBlockSo.ImagePath);
            imagePath.Shuffle();

            for (int i = 0; i < 3; i++)
            {
                Tex[i] = await ToolBox.CreateTextureFromPath(imagePath[i]);
            }


            foreach (var t in Teams)
            {
                for (int i = 0; i < Tex.Length; i++)
                {
                    t.DeActiveAllMark();
                    t.ImageCheckMarks[i].sprite = await ToolBox.CreateSpriteFromTexture(Tex[i]);
                }
            }

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

        await Task.Delay(2000);

        foreach (var t in Teams)
        {
            t.ActiveAllButton();
            t.ShufflePart(0.25f);
        }

        tasks[0] = teamAQueue.Completion;
        tasks[1] = teamBQueue.Completion;

        TimerEnd += FinishPlayGame;
        _timerTokenSource = new CancellationTokenSource();
        Task timer = Timer(_timerTokenSource.Token);
        tasks[2] = timer;

        await Task.WhenAny(tasks);
    }

    async Task PlayOneImage(Texture2D texture, int teamID, int id, bool notFirst = true)
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].DestroyAllParts());
        await UnityMainThreadDispatcher.Instance().EnqueueAsync(() => SplitImage(texture, id + 2, teamID));


        if (notFirst)
        {
            await Task.Delay(2000);
            UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].ActiveAllButton());
            UnityMainThreadDispatcher.Instance().Enqueue(() => Teams[teamID].ShufflePart(id * 0.25f + 0.25f));
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
            //Oups Egalit�

            Debug.Log("Egalit� !");
            return;
        }

        await DecreaseTimer();

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

    async Task DecreaseTimer()
    {
        int gain = Mathf.FloorToInt(_currentTimer);
        for (int i = 0; i < gain; i++)
        {
            _currentTimer -= 1f;
            SetTimerText();
            await Task.Delay(Mathf.RoundToInt(1f / gain * 1000));
        }
    }

    void SetTimerText()
    {
        int minute = Mathf.FloorToInt(_currentTimer / 60);
        minute = minute < 0 ? 0 : minute;
        int second = Mathf.FloorToInt(_currentTimer % 60);
        second = second < 0 ? 0 : second;
        if (second < 10) _timerTextFront.text = $"{minute}:0{second}";
        else _timerTextFront.text = $"{minute}:{second}";
        if (second < 10) _timerTextBack.text = $"{minute}:0{second}";
        else _timerTextBack.text = $"{minute}:{second}";
    }

    void ShuffleParts()
    {
        for (int i = 0; i < Teams[0].Parts.Count; i++)
        {
            int rot = GetRandomRotation();
            Teams[0].Parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
            Teams[1].Parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rot));
        }
    }

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

    #region SplitImage

    [SerializeField]
    GameObject _partPrefab;

    [SerializeField]
    Vector3 _scaleImage;

    public async Task SplitImage(string path, int nbDivision, int teamID)
    {
        GameObject parent = Teams[teamID].ImageHolder;
        Texture2D texture = await ToolBox.CreateTextureFromPath(path);

        for (int i = 0; i < nbDivision; i++)
        {
            for (int j = 0; j < nbDivision; j++)
            {
                float h = texture.height / nbDivision;
                float w = texture.width / nbDivision;
                Sprite newSprite = Sprite.Create(texture, new Rect(i * w, j * h, w, h), new Vector2(0.5f, 0.5f));
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
                Teams[teamID].AddPart(n);
            }
        }
    }

    public void SplitImage(Texture2D texture, int nbDivision, int teamID)
    {
        GameObject parent = Teams[teamID].ImageHolder;

        for (int i = 0; i < nbDivision; i++)
        {
            for (int j = 0; j < nbDivision; j++)
            {
                float h = texture.height / nbDivision;
                float w = texture.width / nbDivision;
                Sprite newSprite = Sprite.Create(texture, new Rect(i * w, j * h, w, h), new Vector2(0.5f, 0.5f));
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
                Teams[teamID].AddPart(n);
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
        base.Awake();
        await HomeAwake();
        await GameAwake();
        await ScoreAwake();
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
        public List<ButtonPartRotation> Parts;

        private bool _imageLoaded;
        public bool ImageLoaded { get
            {
                return _imageLoaded && Parts.Count > 0;
            }
            set 
            {
                _imageLoaded = value;
            }
        }
        public bool IsShuffle => !CheckAllPartsRotation();
        public Image[] ImageCheckMarks;
        public GameObject[] CheckMarks;
        //public TextMeshProUGUI NameTextFront;
        //public TextMeshProUGUI NameTextBack;
        //public TextMeshProUGUI ScoreTextFront;
        //public TextMeshProUGUI ScoreTextBack;

        public Action<int> Win;

        //public void SetAllText()
        //{
        //    NameTextFront.text = Name;
        //    NameTextBack.text = Name;
        //    ScoreTextFront.text = Score.ToString() +" pts";
        //    ScoreTextBack.text = Score.ToString() +" pts";
        //}

        public void AddPart(GameObject part)
        {
            part.TryGetComponent(out ButtonPartRotation buttonPart);
            if (buttonPart != null)
            {
                buttonPart.Rotate += PartRotate;
                buttonPart.IsActive = false;
                Parts.Add(buttonPart);
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

        public void ShufflePart(float pourcent = 1f)
        {
            int total = 0;
            List<int> rots = new();

            while(total != GetLimitThrow(pourcent))
            {
                rots = new();
                total = 0;
                for (int i = 0; i < Mathf.RoundToInt(Parts.Count * pourcent); i++)
                {
                    int rot = GetRandomRotation();
                    rots.Add(rot);
                    total += GetNbThrowWithRotation(rot);
                }
            }

            Parts.Shuffle();
            for (int i = 0; i < rots.Count; i++)
            {
                Parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, rots[i]));
            }

        }

        int GetLimitThrow(float pourcent)
        {
            switch (pourcent)
            {
                case 0.25f:
                    return 2;
                case 0.5f:
                    return 10;
                case 0.75f:
                    return 24;
                default:
                    return -1;
            }
        }

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

        public bool CheckAllPartsRotation()
        {
            foreach (var part in Parts)
            {
                if(part.transform.rotation != Quaternion.Euler(0, 0, 0))
                    return false;
            }
            return true;
        }

        public void DestroyAllParts()
        {
            foreach (var part in Parts)
                if(part != null)
                    Destroy(part.gameObject);
            Parts.Clear();
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