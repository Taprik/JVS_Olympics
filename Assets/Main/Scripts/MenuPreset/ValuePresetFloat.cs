using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueFloat")]
public class ValuePresetFloat : ValuePreset<float>
{
    public override float GetValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                _valueEasy = PlayerPrefs.GetInt(Key);
                return _valueEasy;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetInt(Key);
                return _valueMedium;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetInt(Key);
                return _valueHard;

            default:
                break;
        }
        return -1;
    }

    public override void SaveValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                PlayerPrefs.SetFloat(Key, _valueEasy);
                break;

            case PresetEnum.Medium:
                PlayerPrefs.SetFloat(Key, _valueMedium);
                break;

            case PresetEnum.Hard:
                PlayerPrefs.SetFloat(Key, _valueHard);
                break;

            default:
                break;
        }
    }

    public override void SetValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                _valueEasy = PlayerPrefs.GetFloat(Key);
                break;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetFloat(Key);
                break;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetFloat(Key);
                break;

            default:
                break;
        }
    }
}
