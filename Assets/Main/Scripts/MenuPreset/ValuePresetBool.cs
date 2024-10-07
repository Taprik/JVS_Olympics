using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueBool")]
public class ValuePresetBool : ValuePreset<bool>
{
    public override bool GetValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                _valueEasy = PlayerPrefs.GetInt(Key) == 1;
                return _valueEasy;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetInt(Key) == 1;
                return _valueMedium;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetInt(Key) == 1;
                return _valueHard;

            default:
                break;
        }
        return false;
    }

    public override void SaveValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                PlayerPrefs.SetInt(Key, _valueEasy ? 1 : 0);
                break;

            case PresetEnum.Medium:
                PlayerPrefs.SetInt(Key, _valueMedium ? 1 : 0);
                break;

            case PresetEnum.Hard:
                PlayerPrefs.SetInt(Key, _valueHard ? 1 : 0);
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
                _valueEasy = PlayerPrefs.GetInt(Key) == 1;
                break;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetInt(Key) == 1;
                break;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetInt(Key) == 1;
                break;

            default:
                break;
        }
    }
}