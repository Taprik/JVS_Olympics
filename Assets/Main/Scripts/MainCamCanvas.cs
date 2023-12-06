using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
[DisallowMultipleComponent]
public class MainCamCanvas : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
