using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ValuePreset : ScriptableObject
{
    public enum PresetEnum
    {
        Easy,
        Medium,
        Hard
    }

    public abstract void SaveValue(PresetEnum level);
    public abstract void SetValue(PresetEnum level);
}

public abstract class ValuePreset<T> : ValuePreset
{
    public string Key => _key;
    [SerializeField] protected string _key;

    public T ValueEasy => _valueEasy;
    [SerializeField] protected T _valueEasy;
    public T ValueMedium => _valueMedium;
    [SerializeField] protected T _valueMedium;
    public T ValueHard => _valueHard;
    [SerializeField] protected T _valueHard;

    public abstract T GetValue(PresetEnum level);
}