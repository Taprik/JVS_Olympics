using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public void Awake()
    {
        if (Instance == null)
        {
            Instance = this; 
            return;
        }
        
        Debug.Log("Il y a plus d'une Instance de " + this.GetType() + " dans la scene !");
        Destroy(gameObject);
    }

    public GameSO[] GameScriptableObjects => _gameScriptableObjects;
    [SerializeField]
    GameSO[] _gameScriptableObjects;

    public GameSO CurrentGame => _currentGame;
    [SerializeField]
    GameSO _currentGame;

    #region Manager

    public AddressablesManager AddressablesManager => _addressablesManager;
    [SerializeField]
    AddressablesManager _addressablesManager;

    #endregion

    


    private void Start()
    {
        
    }

    public void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            _addressablesManager.Init();
        }
    }


}
