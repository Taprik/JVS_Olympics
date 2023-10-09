using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameSceneObject : MonoBehaviour
{
    public void Awake()
    {
        GameManager.Instance.GameSceneManager.CurrentSceneObject = this.gameObject;
    }
}
