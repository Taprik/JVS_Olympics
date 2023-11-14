using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoadScene : ButtonParent
{
    [SerializeField]
    SceneName scene;

    public override void DoWork()
    {
        Debug.Log("Load");
        GameManager.Instance.AddressablesManager.LoadScreen(GameManager.Instance.GameSceneManager.LoadScene(scene));
    }
}
