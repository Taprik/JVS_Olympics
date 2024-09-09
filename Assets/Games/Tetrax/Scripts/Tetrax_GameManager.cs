using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tetrax
{
    [System.Serializable]
    public class TetraxTeam
    {
        public string Name;
        public TeamColor Color;
        public Transform CanvasPart;
        public Transform SpawnCubeHolder;
    }

    [System.Serializable]
    public class CubeData
    {
        public TeamColor Color;
        public bool IsSpe = false;
        public RuntimeAnimatorController Animator;
    }

    public enum TeamColor
    {
        Red,
        Blue
    }

    public class Tetrax_GameManager : MonoBehaviour
    {
        public TetraxTeam[] Teams => _teams;
        [SerializeField] TetraxTeam[] _teams;

        [SerializeField] GameObject _cubePref;

        public CubeData[] CubesDatas => _cubesDatas;
        [SerializeField] CubeData[] _cubesDatas;

        public GameObject SpawnCube(CubeData data, Transform parent)
        {
            var cube = Instantiate(_cubePref, parent);
            var anim = cube.GetComponent<Animator>();
            anim.runtimeAnimatorController = data.Animator;
            return cube;
        }
    }
}