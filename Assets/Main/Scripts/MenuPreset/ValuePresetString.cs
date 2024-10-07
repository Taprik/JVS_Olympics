using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueString")]
public class ValuePresetString : ValuePreset<string>
{
    public override string GetValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                _valueEasy = PlayerPrefs.GetString(Key);
                return _valueEasy;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetString(Key);
                return _valueMedium;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetString(Key);
                return _valueHard;

            default:
                break;
        }
        return string.Empty;
    }

    public override void SaveValue(PresetEnum level)
    {
        switch (level)
        {
            case PresetEnum.Easy:
                PlayerPrefs.SetString(Key, _valueEasy);
                break;

            case PresetEnum.Medium:
                PlayerPrefs.SetString(Key, _valueMedium);
                break;

            case PresetEnum.Hard:
                PlayerPrefs.SetString(Key, _valueHard);
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
                _valueEasy = PlayerPrefs.GetString(Key);
                break;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetString(Key);
                break;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetString(Key);
                break;

            default:
                break;
        }
    }
}
