using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameSceneObject : MonoBehaviour
{
    [Header("Home")]
    [SerializeField] protected GameObject _homePage;
    public void SetHomePage(bool isActive) => _homePage.SetActive(isActive);

    [Header("Game")]
    [SerializeField] protected GameObject _gamePage;
    public void SetGamePage(bool isActive) => _gamePage.SetActive(isActive);

    [Header("Menu")]
    [SerializeField] protected GameObject _menuPage;
   public void SetMenuPage(bool isActive) { if(_menuPage != null) _menuPage.SetActive(isActive); }

    [Header("Score")]
    [SerializeField] protected GameObject _scorePage;
    public void SetScorePage(bool isActive) => _scorePage.SetActive(isActive);

    public virtual void Start()
    {
        GameManager.CurrentGameSceneObject = this;
        GameManager.Instance.GameSceneManager.CurrentSceneObject = this.gameObject;
        SetHomePage(true);
        SetGamePage(false);
        SetMenuPage(false);
        SetScorePage(false);
    }

    public virtual async Task InitScene()
    {
        GameManager.Instance.OSCManager.Ready();
    }

    public virtual void Play()
    {
        SetHomePage(false);
        SetGamePage(true);
        SetMenuPage(false);
        SetScorePage(false);
        GameManager.Instance.OSCManager.GameEnCours();
        GameManager.OnGameStart?.Invoke();
    }

    public virtual void PlayScore()
    {
        SetHomePage(false);
        SetGamePage(false);
        SetMenuPage(false);
        SetScorePage(true);
    }

    public virtual void OpenMenu()
    {
        SetHomePage(false);
        SetGamePage(false);
        SetMenuPage(true);
        SetScorePage(false);
    }

    public abstract Task Replay();
    public abstract void OnNameReceive(string name);
    public abstract void PageUp();
    public abstract void PageDown();
}
