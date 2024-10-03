using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueInt")]
public class ValuePresetInt : ValuePreset<int>
{
    public override int GetValue()
    {
        _value = PlayerPrefs.GetInt(Key);
        return _value;
    }

    public override void SaveValue()
    {
        PlayerPrefs.SetInt(Key, Value);
    }
}
