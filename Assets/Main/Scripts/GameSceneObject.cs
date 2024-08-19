using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameSceneObject : MonoBehaviour
{
    public virtual void Start()
    {
        GameManager.CurrentGameSceneObject = this;
        GameManager.Instance.GameSceneManager.CurrentSceneObject = this.gameObject;
    }

    public virtual async Task InitScene()
    {
        GameManager.Instance.OSCManager.Ready();
    }

    public virtual void Play()
    {
        GameManager.Instance.OSCManager.GameEnCours();
    }

    public abstract Task Replay();

    public abstract void OnNameReceive(string name);
    public abstract void PageUp();
    public abstract void PageDown();
}
