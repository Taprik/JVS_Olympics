using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetActiveChildren : MonoBehaviour
{
    [SerializeField] GameObject Children;
    public void SetActiveChildrenTrue() => Children.SetActive(true);
    public void SetActiveChildrenFalse() => Children.SetActive(false);
}
