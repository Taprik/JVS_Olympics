using OSC;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class OSCTester : MonoBehaviour
{
    [SerializeField]
    KeyCode _startMessage;

    [SerializeField]
    KeyCode _acceuilMessage;

    [SerializeField]
    KeyCode _launchMessage;
    [SerializeField]
    string _gameToLaunch;

    public void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            float x = (float)Input.mousePosition.x / Screen.width;
            float y = (float)Input.mousePosition.y / Screen.height;
            OscMessage message = new OscMessage("/point");
            message.Add(x);
            message.Add(y);
            GameManager.Instance.OSCManager.onOSCPoint(message);
        }

        if(Input.GetKeyDown(_startMessage))
        {
            OscMessage msg = new OscMessage("/remote/Start");
            GameManager.Instance.OSCManager.onOSCStart(msg);
        }

        if(Input.GetKeyDown(_acceuilMessage))
        {
            OscMessage msg = new OscMessage("/remote/Accueil");
            GameManager.Instance.OSCManager.onOSCAccueil(msg);
        }

        if (Input.GetKeyDown(_launchMessage))
        {
            OscMessage msg = new OscMessage("/remote/Accueil");
            msg.Add(_gameToLaunch);
            GameManager.Instance.OSCManager.onOSCAccueil(msg);
        }
    }
}
