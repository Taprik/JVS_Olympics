using DG.Tweening;
using GlobalOutline;
using OSC;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            return;
        }
        
        Debug.LogWarning("Il y a plus d'une Instance de " + this.GetType() + " dans la scene !");
        Destroy(gameObject);
    }

    public GameSO CurrentGame { get; set; }
    public GameSceneObject CurrentGameSceneObject { get; set; }

    #region Manager

    public AddressablesManager AddressablesManager => _addressablesManager;
    [SerializeField, Header("Managers")]
    AddressablesManager _addressablesManager;

    public OSC_Manager OSCManager => _oscManager;
    [SerializeField]
    OSC_Manager _oscManager;

    public GameSceneManager GameSceneManager => _gameSceneManager;
    [SerializeField]
    GameSceneManager _gameSceneManager;

    public TasksManager TasksManager => _tasksManager;
    [SerializeField]
    TasksManager _tasksManager;

    public OutlineManager OutlineManager => _outlineManager;
    [SerializeField]
    OutlineManager _outlineManager;

    public ScoreBoardManager ScoreBoardManager => _scoreBoardManager;
    [SerializeField]
    ScoreBoardManager _scoreBoardManager;

    #endregion

    #region Loading

    public GameObject LoadScreen => _loadScreen;
    [SerializeField, Header("Loading Screen")]
    GameObject _loadScreen;

    public Slider LoadScreenBar => _loadScreenBar;
    [SerializeField]
    Slider _loadScreenBar;

    public TextMeshProUGUI LoadScreenText => _loadScreenText;
    [SerializeField]
    TextMeshProUGUI _loadScreenText;

    public GameObject MainSceneObject => _mainSceneObject;
    [SerializeField]
    GameObject _mainSceneObject;


    #endregion

    #region Shortcut Key

    [SerializeField, Header("Shortcut Key")]
    KeyCode _backMainMenu;

    [SerializeField]
    KeyCode _loadAllAsset;

    [SerializeField]
    KeyCode _quitApp;

    [SerializeField]
    KeyCode _resetScoreBoard;

    #endregion

    #region Message

    public KeyMessage CurrentMessage;

    public GameObject _messageObject;

    public TextMeshProUGUI _messageText;

    public TextMeshProUGUI _optionAText;

    public TextMeshProUGUI _optionBText;

    #endregion

    private void Start()
    {
        GameSceneManager.Init();
    }

    public void Update()
    {
        if(Input.GetKeyDown(_loadAllAsset))
            Message(ref CurrentMessage, "Voulez-vous charger tous les assets ?", () => AddressablesManager.Init(), _loadAllAsset);

        if(Input.GetKeyDown(_quitApp))
            Message(ref CurrentMessage, "Voulez-vous vraiment quitter ?", () => OSCManager.messageOutQuit(), _quitApp);

        if(Input.GetKeyDown(_backMainMenu))
            Message(ref CurrentMessage, "Voulez-vous retourner à l'acceuil ?" , () => GameSceneManager.LoadScene(SceneName.MainScene), _backMainMenu);

        if(Input.GetKeyDown(_resetScoreBoard))
            Message(ref CurrentMessage, "Voulez-vous reset tous les ScoreBoard ?", () => ScoreBoardManager.ResetAllScoreBoard(), _resetScoreBoard);
    }

    public void Message(ref KeyMessage msg, string message, Action action, KeyCode key, string optionA = "Oui", string optionB = "Non", float timer = 5f)
    {
        if (msg != null)
        {
            if (msg.IsRunning && msg.KeyCode == key)
                msg.OptionA();
            else
                msg = null;
        }

        msg ??= new KeyMessage(message, action, key, optionA, optionB, timer);
    }

    public void SetLoadBar(float value, float duration)
    {
        LoadScreenText.text = Mathf.RoundToInt(value * 100f) + "%";
        LoadScreenBar.DOValue(value, duration);
    }
}

public class KeyMessage
{
    private GameObject MessageObject => GameManager.Instance._messageObject;
    private TextMeshProUGUI MessageText => GameManager.Instance._messageText;
    private TextMeshProUGUI OptionAText => GameManager.Instance._optionAText;
    private TextMeshProUGUI OptionBText => GameManager.Instance._optionBText;

    private readonly string _message;
    private readonly string _optionA;
    private readonly string _optionB;
    private readonly Action _action;
    private float _timer;
    private CancellationTokenSource _cancellationToken;

    public bool IsRunning { get; private set; }
    public KeyCode KeyCode { get; private set; }

    public KeyMessage(string message, Action action, KeyCode key, string optionA = "Oui", string optionB = "Non", float timer = 5f)
    {
        _message = message;
        _optionA = optionA;
        _optionB = optionB;
        _action = action;
        _timer = timer;
        KeyCode = key;
        IsRunning = true;

        SetUpMessage();
    }

    ~KeyMessage()
    {
        _cancellationToken?.Cancel();
        _cancellationToken?.Dispose();
        IsRunning = false;
    }

    private void DisplayMessage() => MessageObject.SetActive(true);
    private void HideMessage() => MessageObject.SetActive(false);
    private async Task Timer(CancellationToken token)
    {
        while (_timer > 0 && !token.IsCancellationRequested)
        {
            await Task.Delay(50);
            _timer -= 0.05f;
        }
    }

    public void SetUpMessage()
    {
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            MessageText.text = _message;
            OptionAText.text = _optionA;
            OptionBText.text = _optionB;
            DisplayMessage();
        });

        WaitTImer();
    }

    private async void WaitTImer()
    {
        _cancellationToken = new CancellationTokenSource();
        await Timer(_cancellationToken.Token);
        _cancellationToken.Dispose();
        _cancellationToken = null;
        HideMessage();
        IsRunning = false;
    }

    public void OptionA()
    {
        _cancellationToken?.Cancel();
        UnityMainThreadDispatcher.Instance().Enqueue(() =>
        {
            _action?.Invoke();
        });
    }
}
