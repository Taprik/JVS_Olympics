using OSC;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OSCTester : MonoBehaviour
{
    public void Update()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            float x = (float)Input.mousePosition.x / Screen.width;
            float y = (float)Input.mousePosition.y / Screen.height;
            OscMessage message = new OscMessage("/point");
            message.Add(x);
            message.Add(y);
            GameManager.Instance.OSCManager.onOSCPoint(message);
        }
    }
}
