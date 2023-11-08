using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonMultipleTache : ButtonParent
{
    [SerializeField]
    MultipleTaches _multipleTaches;

    public override void DoWork()
    {
        _multipleTaches.DoWork();
    }
}
