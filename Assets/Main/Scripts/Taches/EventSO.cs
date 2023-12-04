using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "EventSO", menuName = "Game/Event/EventSO")]
public class EventSO : ScriptableObject
{
    public List<Func<object>> _listeners = new();

    public virtual void Raise()
    {
        Debug.Log(_listeners.Count);
        for (int i = 0; i < _listeners.Count; i++)
        {
            _listeners[i]();
        }
        _listeners.Clear();
    }

    public void RegisterListener(Func<object> listener)
    {
        if (!_listeners.Contains(listener))
            _listeners.Add(listener);
    }

    public void UnregisterListener(Func<object> listener)
    {
        if (_listeners.Contains(listener))
            _listeners.Remove(listener);
    }
}
