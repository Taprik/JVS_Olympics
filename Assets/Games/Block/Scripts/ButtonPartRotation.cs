using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonPartRotation : ButtonParent
{
    public Action Rotate;
    public bool IsActive { get; set; }
    public override void DoWork()
    {
        if (!IsActive) return;

        this.gameObject.transform.rotation = Quaternion.Euler(new Vector3(0, 0, this.gameObject.transform.rotation.eulerAngles.z - 90));
        Rotate?.Invoke();
    }
}
