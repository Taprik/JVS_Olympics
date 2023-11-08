using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSO", menuName = "Game/Event/EventSO")]
public class EventSO : ScriptableObject
{
    public Action Action { get; set; }

    public virtual void Raise() => Action?.Invoke();
}
