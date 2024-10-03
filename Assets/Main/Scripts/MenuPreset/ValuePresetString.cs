using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueString")]
public class ValuePresetString : ValuePreset<string>
{
    public override string GetValue()
    {
        _value = PlayerPrefs.GetString(Key);
        return _value;
    }

    public override void SaveValue()
    {
        PlayerPrefs.SetString(Key, Value);
    }
}
