using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Menu/Preset")]
public class Preset : ScriptableObject
{
    [SerializeField] List<ValuePreset> AllValues;
}
