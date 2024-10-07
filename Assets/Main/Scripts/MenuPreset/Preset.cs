using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/Preset")]
public class Preset : ScriptableObject
{
    [SerializeField] List<ValuePreset> AllValues;

    public void SavePreset(ValuePreset.PresetEnum type)
    {
        foreach (var v in AllValues)
        {
            v.SetValue(type);
        }
    }

    public void ActivePreset(ValuePreset.PresetEnum type)
    {
        foreach (var v in AllValues)
        {
            v.SaveValue(type);
        }
    }
}
