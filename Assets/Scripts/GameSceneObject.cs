using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneObject : MonoBehaviour
{
    public virtual void Awake()
    {
        GameManager.Instance.CurrentGameSceneObject = this;
        GameManager.Instance.GameSceneManager.CurrentSceneObject = this.gameObject;
    }
}
