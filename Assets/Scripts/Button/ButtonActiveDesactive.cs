using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActiveDesactive : MonoBehaviour, IReceivePoint
{
    [SerializeField]
    GameObject activeObject;
    [SerializeField]
    GameObject desactiveObject;

    public void ReceivePoint(float xPoint, float yPoint)
    {
        Vector2 hit = new Vector2(xPoint, yPoint);
        //Debug.Log(this.gameObject.name + " : " + ToolBox.CheckPos(hit, this.transform));

        if (ToolBox.CheckPos(hit, this.transform))
        {
            desactiveObject.SetActive(false);
            activeObject.SetActive(true);
        }
    }
}
