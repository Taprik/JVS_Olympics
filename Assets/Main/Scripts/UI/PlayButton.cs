using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonEvent))]
public class PlayButton : MonoBehaviour
{
    ButtonEvent b;

    private void Start()
    {
        b = GetComponent<ButtonEvent>();
        b.Event.AddListener(OnClick);
    }

    void OnClick()
    {
        if (GameManager.CurrentGameSceneObject == null) return;
        GameManager.CurrentGameSceneObject.Play();
    }
}
