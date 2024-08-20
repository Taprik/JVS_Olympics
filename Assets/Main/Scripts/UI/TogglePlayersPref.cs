using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Toggle))]
public class TogglePlayersPref : MonoBehaviour
{
    [SerializeField] string _key;
    Toggle _toogle;

    public void Start()
    {
        _toogle = GetComponent<Toggle>();

        if (PlayerPrefs.HasKey(_key))
            _toogle.isOn = PlayerPrefs.GetInt(_key) == 1;

        _toogle.onValueChanged.AddListener((value) => PlayerPrefs.SetInt(_key, value ? 1 : 0));
    }
}
