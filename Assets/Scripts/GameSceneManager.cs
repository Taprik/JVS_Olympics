using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.SceneManagement;

[CreateAssetMenu(fileName = "GameSceneManager", menuName = "Manager/GameSceneManager")]
public class GameSceneManager : ScriptableObject
{
    public SceneName Scene => _scene;
    [SerializeField]
    SceneName _scene;

    public void LoadScene(SceneName scene)
    {
        SceneManager.LoadSceneAsync(scene.ToString(), LoadSceneMode.Additive);
    }

}

public enum SceneName
{
    MainScene,
    Game
}
