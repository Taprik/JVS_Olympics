using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEventInt : ButtonParent
{
    public int Value { get; set; }

    [SerializeField]
    UnityEvent<int> _event;

    public override void DoWork()
    {
        _event?.Invoke(Value);
    }
}
