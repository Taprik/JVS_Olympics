using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Target
{
    public class Target_GameManager : MonoBehaviour
    {
        public static Target_GameManager Instance;

        public void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }

            Instance = this;
        }

        public Camera Cam => _cam;
        [SerializeField] Camera _cam;
    }
}