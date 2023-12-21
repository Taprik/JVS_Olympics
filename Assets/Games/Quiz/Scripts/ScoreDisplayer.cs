using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplayer : MonoBehaviour
{
    [SerializeField]
    TextMeshProUGUI[] _texts;

    public void Init(string text, TMP_FontAsset textFont = null)
    {
        foreach (var t in _texts)
        {
            t.text = text;
            if (textFont != null) t.font = textFont;
        }
    }
}
