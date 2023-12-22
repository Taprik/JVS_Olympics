using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameSceneObject : MonoBehaviour
{
    public virtual void Awake()
    {
        GameManager.Instance.CurrentGameSceneObject = this;
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

    public async virtual Task Replay() { }

    public virtual void OnNameReceive(string name) { }
    public virtual void PageUp() { }
    public virtual void PageDown() { }
}
