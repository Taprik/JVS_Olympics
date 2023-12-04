using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class ButtonActiveDesactive : ButtonParent
{
    [SerializeField]
    GameObject[] activeObject;
    [SerializeField]
    GameObject[] desactiveObject;

    public override void DoWork()
    {
        foreach (var obj in desactiveObject)
        {
            obj.SetActive(false);
        }
        foreach (var obj in activeObject)
        {
            obj.SetActive(true);
        }
    }
}
