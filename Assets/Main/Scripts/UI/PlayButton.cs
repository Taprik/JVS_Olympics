using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class PlayButton : MonoBehaviour
{
    Button b;

    private void Start()
    {
        b = GetComponent<Button>();
        b.onClick.AddListener(OnClick);
    }

    void OnClick()
    {
        if (GameManager.CurrentGameSceneObject == null) return;
        GameManager.CurrentGameSceneObject.Play();
    }
}
