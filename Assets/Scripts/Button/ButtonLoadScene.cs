using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonLoadScene : MonoBehaviour, IReceivePoint
{
    [SerializeField]
    SceneName scene;

    public void ReceivePoint(float xPoint, float yPoint)
    {
        Vector2 hit = new Vector2(xPoint, yPoint);
        //Debug.Log(this.gameObject.name + " : " + ToolBox.CheckPos(hit, this.transform));

        if (ToolBox.CheckPos(hit, this.transform))
        {
            GameManager.Instance.GameSceneManager.LoadScene(scene);
        }
    }
}
