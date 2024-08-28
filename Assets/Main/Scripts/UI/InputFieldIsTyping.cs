using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class InputFieldIsTyping : MonoBehaviour
{
    TMP_InputField _inputField;

    public void Start()
    {
        _inputField = GetComponent<TMP_InputField>();

        _inputField.onSelect.AddListener((value) => GameManager.IsTyping = true);
        _inputField.onDeselect.AddListener((value) => GameManager.IsTyping = false);
    }
}
