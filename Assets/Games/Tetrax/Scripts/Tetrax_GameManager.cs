using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetrax
{
    [System.Serializable]
    public class TetraxTeam
    {
        public string Name;
        public Transform CanvasPart;
        public Transform SpawnCubeHolder;
    }

    public class Tetrax_GameManager : MonoBehaviour
    {
        public TetraxTeam[] Teams => _teams;
        [SerializeField] TetraxTeam[] _teams;

        [SerializeField] GameObject _cubePref;
    }
}