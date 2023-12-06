using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Tool;
using System;
using Unity.VisualScripting;

public abstract class ButtonParent : MonoBehaviour, IReceivePoint
{
    public Action<bool> OnActiveChange;
    protected bool isActive = true;
    public bool IsActive
    {
        get
        {
            return isActive;
        }

        set
        {
            isActive = value;
            OnActiveChange?.Invoke(isActive);
        }
    }

    public void Awake()
    {
        Canvas canvas = gameObject.AddComponent<Canvas>();
        if (canvas != null) canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        if (canvas != null) canvas.sortingOrder = 16960;
    }

    public void ReceivePoint(float xPoint, float yPoint)
    {
        Vector2 hit = new Vector2(xPoint, yPoint);
        //Debug.Log(this.gameObject.name + " : " + ToolBox.CheckPos(hit, this.transform) + " | Hit : " + hit + " | Pos : " + this.transform.position);

        if (ToolBox.CheckPos(hit, this.transform) && isActive)
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
