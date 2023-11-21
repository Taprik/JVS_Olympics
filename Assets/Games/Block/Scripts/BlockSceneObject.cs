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
    public GameBlock GameBlock => _gameBlock;
    [SerializeField]
    GameBlock _gameBlock;

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
    TextMeshProUGUI _timerText;
    Action<int> TimerEnd;
    CancellationTokenSource _timerTokenSource;

    public List<BlockTeam> Teams;

    const string ImagePath = "C:\\Users\\smartJeux\\Documents\\Capteur\\Personnalisation\\Blocks\\choisi";

    async Task GameAwake()
    {

    }

    async Task GameStart()
    {
        foreach (var t in Teams)
        {
            t.SetAllText();
            t.Win += TeamWin;
        }
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
        Task[] splitImages = new Task[2];
        splitImages[0] = SplitImage(ToolBox.GetFiles(ImagePath)[0], _nbDivision, 0);
        splitImages[1] = SplitImage(ToolBox.GetFiles(ImagePath)[0], _nbDivision, 1);
        await Task.WhenAll(splitImages);

        await Task.Delay(2000);

        foreach (var t in Teams)
        {
            t.ShufflePart();
            t.ActiveAllButton();
        }

        TimerEnd += FinishPlayGame;
        _timerTokenSource = new CancellationTokenSource();
        Task timer = Timer(_timerTokenSource.Token);
    }

    public async void FinishPlayGame(int teamID)
    {
        TimerEnd -= FinishPlayGame;
        _timerTokenSource.Dispose();
        foreach (var t in Teams)
            t.DeActiveAllButton();

        if (teamID < 0)
        {

            return;
        }

        Task[] tasks = new Task[2];

        tasks[0] = Teams[teamID].AddScore(Mathf.FloorToInt(_currentTimer), 1f);
        tasks[1] = DecreaseTimer();

        await Task.WhenAll(tasks);

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
        int second = Mathf.FloorToInt(_currentTimer % 60);
        if (second < 10) _timerText.text = $"{minute}:0{second}";
        else _timerText.text = $"{minute}:{second}";
    }

    #region SplitImage

    [SerializeField]
    GameObject _partPrefab;

    [SerializeField, Tooltip("That is the RootSquare of the numbre enter :")]
    int _nbDivision;

    [SerializeField]
    Vector3 _scaleImage;

    public async Task SplitImage(string path,int nbDivision, int teamID)
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
        public TextMeshProUGUI NameText;
        public TextMeshProUGUI ScoreText;

        public Action<int> Win;

        public void SetAllText()
        {
            NameText.text = Name;
            ScoreText.text = Score.ToString() +" pts";
        }

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
            if (CheckAllPartsRotation())
            {
                Win?.Invoke(ID);
            }
        }

        public void ShufflePart()
        {
            for (int i = 0; i < Parts.Count; i++)
            {
                Parts[i].transform.rotation = Quaternion.Euler(new Vector3(0, 0, GetRandomRotation()));
            }
        }

        public bool CheckAllPartsRotation()
        {
            foreach (var part in Parts)
            {
                if(part.transform.rotation != Quaternion.identity)
                    return false;
            }
            return true;
        }

        int GetRandomRotation()
        {
            int rot = Random.Range(0, 4);
            switch (rot)
            {
                case 0:
                    return 90;
                case 1:
                    return 180;
                case 2:
                    return 270;
                case 3:
                    return 0;
                default:
                    return -1;
            }
        }

        void DestroyAllParts()
        {
            foreach (var part in Parts)
                Destroy(part.gameObject);
            Parts.Clear();
        }

        public async Task AddScore(int gain, float time)
        {
            for (int i = 0; i < gain; i++)
            {
                ScoreText.text = (Score + i).ToString() + " pts";
                await Task.Delay(Mathf.RoundToInt(time/gain * 1000));
            }
            Score += gain;
        }
    }
}