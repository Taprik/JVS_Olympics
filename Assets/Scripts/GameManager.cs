using OSC;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System;

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

    public GameSO[] GameScriptableObjects => _gameScriptableObjects;
    [SerializeField]
    GameSO[] _gameScriptableObjects;

    public GameSO CurrentGame { get; set; }

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

    public ExecutionQueueManager ExecutionQueueManager => _executionQueueManager;
    [SerializeField]
    ExecutionQueueManager _executionQueueManager;

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


    private void Start()
    {
        GameSceneManager.Init();
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            AddressablesManager.Init();
        }

        if(Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }

        if(Input.GetKeyDown(KeyCode.O))
        {
            ExecutionQueue loadAllQueue = ExecutionQueueManager.CreateTaskQueue("LoadAll");
            ToolBox.DoTaskInMainThread(ExecutionQueueManager.LoadScrenOnTask(AddressablesManager.LoadScreen, loadAllQueue.Run(() => WaitTask(1))));
            ToolBox.DoTaskInMainThread(ExecutionQueueManager.LoadScrenOnTask(AddressablesManager.LoadScreen, loadAllQueue.Run(() => WaitTask(2))));
        }
    }

    async Task WaitTask(int id)
    {
        for (int i = 0; i <= 5; i++)
        {
            await Task.Delay(200);
            Debug.Log(id + " : " + (i * 2) + "0%");
        }
    }
}
