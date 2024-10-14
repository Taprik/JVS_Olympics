using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[RequireComponent(typeof(TMP_Dropdown))]
public class DropDownPlayersPref : MonoBehaviour
{
    [SerializeField] string _key;
    TMP_Dropdown _dropdown;

    public void Start()
    {
        _dropdown = GetComponent<TMP_Dropdown>();

        if (PlayerPrefs.HasKey(_key))
            _dropdown.value = PlayerPrefs.GetInt(_key);

        GameManager.OnPlayerPrefs += () => {
            if (PlayerPrefs.HasKey(_key))
                _dropdown.value = PlayerPrefs.GetInt(_key);
        };
        _dropdown.onValueChanged.AddListener((value) => PlayerPrefs.SetInt(_key, value));
    }
}
