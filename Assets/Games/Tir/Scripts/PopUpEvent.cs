using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PopUpEvent : MonoBehaviour
{
    [SerializeField]
    UnityEvent _event;

    public void TriggerEvent()
    {
        _event?.Invoke();
    }
}
