using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PresetButton : MonoBehaviour
{
    [SerializeField] Preset preset;
    [SerializeField] ValuePreset.PresetEnum type;

    public void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    public void OnClick()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            preset.SavePreset(type);
        }
        else
        {
            preset.ActivePreset(type);
        }
    }
}
