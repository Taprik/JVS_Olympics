using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoadScene : ButtonParent
{
    [SerializeField]
    SceneName scene;

    public override void DoWork()
    {
        GameManager.Instance.GameSceneManager.LoadScene(scene);
    }
}
