using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(ButtonEvent))]
public class PlayButton : MonoBehaviour
{
    ButtonEvent b;

    public void Awake()
    {
        if(gameObject.TryGetComponent(out Canvas canvas))
        {
            Destroy(canvas);
        }
    }

    private void FixedUpdate()
    {
        if (gameObject.TryGetComponent(out Canvas canvas))
        {
            Destroy(canvas);
        }
    }

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
