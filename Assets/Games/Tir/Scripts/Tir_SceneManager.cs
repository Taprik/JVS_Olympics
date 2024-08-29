using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Tir
{
    public enum Tir_Scene
    {
        Acceuil_Tir,
        GameScene_Tir,
        Menu_Tir,
        Score_Tir
    }

    public class Tir_SceneManager : MonoBehaviour
    {

        public void LoadScene(Tir_Scene name)
        {
            SceneManager.LoadScene(name.ToString());
        }

        public void LoadScene(string name)
        {
            SceneManager.LoadScene(name);
        }
    }
}