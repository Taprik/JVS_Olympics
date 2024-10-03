using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueFloat")]
public class ValuePresetFloat : ValuePreset<float>
{
    public override float GetValue()
    {
        _value = PlayerPrefs.GetFloat(Key);
        return _value;
    }

    public override void SaveValue()
    {
        PlayerPrefs.SetFloat(Key, Value);
    }
}
