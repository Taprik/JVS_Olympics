using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "EventSO", menuName = "Game/Event/UnityEvent")]
public class E_UnityEvent : EventSO
{
    public UnityEvent Action;

    public override void Raise() => Action?.Invoke();
}
