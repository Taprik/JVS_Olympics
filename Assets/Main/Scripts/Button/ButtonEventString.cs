using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonEventString : ButtonParent
{
    public string Value { get; set; }

    [SerializeField]
    UnityEvent<string> _event;

    public override void DoWork()
    {
        _event?.Invoke(Value);
    }
}
