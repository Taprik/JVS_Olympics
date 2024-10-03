using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueBool")]
public class ValuePresetBool : ValuePreset<bool>
{
    public override bool GetValue()
    {
        _value = PlayerPrefs.GetInt(Key) == 1;
        return _value;
    }

    public override void SaveValue()
    {
        PlayerPrefs.SetInt(Key, Value ? 1 : 0);
    }
}