using System.Collections;
using System.Collections.Generic;
using Target;
using UnityEngine;

public class Target_Hitbox : MonoBehaviour, IReceivePoint
{
    Camera _cam => Target_GameManager.Instance.Cam;
    [SerializeField] Target_Animation _parent;

    public void ReceivePoint(float xPoint, float yPoint)
    {
        var hit = new Vector2(xPoint, yPoint);
        var pos = _cam.ScreenToWorldPoint(hit);
        //Debug.Log(pos);
        if(Tool.ToolBox.CheckPos(pos, this.transform))
        {
            Debug.Log("Hit");
            _parent.OnKill();
        }
    }
}
