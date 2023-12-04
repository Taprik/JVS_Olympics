using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Canvas))]
public class MainCamCanvas : MonoBehaviour
{
    private void Start()
    {
        Canvas canvas = this.GetComponent<Canvas>();
        canvas.worldCamera = Camera.main;
    }
}
