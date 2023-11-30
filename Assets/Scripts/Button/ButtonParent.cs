using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tool;

public abstract class ButtonParent : MonoBehaviour, IReceivePoint
{
    public void ReceivePoint(float xPoint, float yPoint)
    {
        Vector2 hit = new Vector2(xPoint, yPoint);
        //Debug.Log(this.gameObject.name + " : " + ToolBox.CheckPos(hit, this.transform) + " | Hit : " + hit + " | Pos : " + this.transform.position);

        if (ToolBox.CheckPos(hit, this.transform))
        {
            DoWork();
        }
    }

    public abstract void DoWork();

    public void SetActive()
    {
        this.gameObject.SetActive(!this.gameObject.activeSelf);
    }
}
