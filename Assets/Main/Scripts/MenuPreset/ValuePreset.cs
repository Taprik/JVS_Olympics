using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValuePreset : ScriptableObject
{

}

public abstract class ValuePreset<T> : ValuePreset
{
    public string Key => _key;
    [SerializeField] protected string _key;

    public T Value => _value;
    [SerializeField] protected T _value;

    public abstract void SaveValue();
    public abstract T GetValue();
}