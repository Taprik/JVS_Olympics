using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEvent : ButtonParent
{
    public UnityEvent Event => _event;
    [SerializeField]
    UnityEvent _event;

    public override void DoWork()
    {
        _event?.Invoke();
    }
}
