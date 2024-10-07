using Newtonsoft.Json.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/ValueInt")]
public class ValuePresetInt : ValuePreset<int>
{
    public override int GetValue(PresetEnum level)
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
                PlayerPrefs.SetInt(Key, _valueEasy);
                break;

            case PresetEnum.Medium:
                PlayerPrefs.SetInt(Key, _valueMedium);
                break;

            case PresetEnum.Hard:
                PlayerPrefs.SetInt(Key, _valueHard);
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
                _valueEasy = PlayerPrefs.GetInt(Key);
                break;

            case PresetEnum.Medium:
                _valueMedium = PlayerPrefs.GetInt(Key);
                break;

            case PresetEnum.Hard:
                _valueHard = PlayerPrefs.GetInt(Key);
                break;

            default:
                break;
        }
    }
}
