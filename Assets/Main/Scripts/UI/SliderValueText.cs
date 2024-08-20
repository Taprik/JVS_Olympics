using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(TextMeshProUGUI))]
public class SliderValueText : MonoBehaviour
{
    TextMeshProUGUI _text;
    [SerializeField] Slider _slider;
    [SerializeField] string _unit;

    private void Start()
    {
        _text = GetComponent<TextMeshProUGUI>();
        _text.text = _slider.value.ToString("n1") + _unit;
        _slider.onValueChanged.AddListener((value) => _text.text = value.ToString("n1") + _unit);
    }
}
