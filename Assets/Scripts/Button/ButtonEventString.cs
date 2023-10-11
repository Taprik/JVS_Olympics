using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEventString : MonoBehaviour, IReceivePoint
{
    public string Value { get; set; }

    [SerializeField]
    UnityEvent<string> _event;

    public void ReceivePoint(float xPoint, float yPoint)
    {
        Vector2 hit = new Vector2(xPoint, yPoint);
        //Debug.Log(this.gameObject.name + " : " + ToolBox.CheckPos(hit, this.transform));

        if (ToolBox.CheckPos(hit, this.transform))
        {
            _event?.Invoke(Value);
        }
    }
}
