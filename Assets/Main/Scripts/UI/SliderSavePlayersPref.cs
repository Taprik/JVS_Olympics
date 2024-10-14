using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Slider))]
public class SliderSavePlayersPref : MonoBehaviour
{
    Slider _slider;
    [SerializeField]
    string _playerPrefKey;

    private void Start()
    {
        _slider = GetComponent<Slider>();
        if (PlayerPrefs.HasKey(_playerPrefKey))
            _slider.value = PlayerPrefs.GetFloat(_playerPrefKey);

        GameManager.OnPlayerPrefs += () => { if (PlayerPrefs.HasKey(_playerPrefKey)) _slider.value = PlayerPrefs.GetFloat(_playerPrefKey); };
        _slider.onValueChanged.AddListener((value) => PlayerPrefs.SetFloat(_playerPrefKey, value));
    }
}
