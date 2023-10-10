using OSC;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OSCTester : MonoBehaviour, IReceivePoint
{
    [SerializeField]
    SceneName scene;

    public void ReceivePoint(float xPoint, float yPoint)
    {
        Vector2 hit = new Vector2(xPoint, yPoint);
        //Debug.Log(this.gameObject.name + " : " + ToolBox.CheckPos(hit, this.transform));

        if(ToolBox.CheckPos(hit, this.transform))
        {
            GameManager.Instance.GameSceneManager.LoadScene(scene);
        }
    }

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
