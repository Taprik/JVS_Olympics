using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEvent : ButtonParent
{
    [SerializeField]
    UnityEvent _event;

    public override void DoWork()
    {
        _event?.Invoke();
    }
}
