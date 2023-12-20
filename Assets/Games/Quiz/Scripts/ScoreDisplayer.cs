using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI _text;

    [SerializeField]
    Image _image;

    public void Init(string text, Color outlineColor, TMP_FontAsset textFont = null)
    {
        _text.text = text;
        if(textFont != null) _text.font = textFont;
        _image.color = outlineColor;
    }
}
