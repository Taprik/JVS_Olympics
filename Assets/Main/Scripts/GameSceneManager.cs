using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using DG.Tweening;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceProviders;

[CreateAssetMenu(fileName = "GameSceneManager", menuName = "Manager/GameSceneManager")]
public class GameSceneManager : ScriptableObject
{
    public SceneName CurrentScene => _currentScene;
    [SerializeField]
    SceneName _currentScene;

    public GameObject CurrentSceneObject { get; set; }

    public bool IsRunning { get; private set; }

    public void Init()
    {
        _currentScene = SceneName.MainScene;
        IsRunning = false;
    }


    public async void LoadScene(SceneName scene)
    {
        if(IsRunning) return;

        IsRunning = true;
        if (CurrentScene != SceneName.MainScene)
            await UnloadScene(CurrentScene, scene != SceneName.MainScene);


        if(scene != SceneName.MainScene)
            await LoadScene(scene, LoadSceneMode.Additive);


        Debug.Log("Load : " + scene.ToString());
        _currentScene = scene;
        IsRunning = false;
    }

    public async Task LoadScene(SceneName scene, LoadSceneMode mode)
    {
        GameManager.Instance.LoadScreenText.text = "0%";
        GameManager.Instance.LoadScreenBar.value = 0;
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        AsyncOperationHandle<SceneInstance> handle = Addressables.LoadSceneAsync(scene.ToString(), mode);
        handle.Completed += async (handle) =>
        {
            GameManager.Instance.LoadScreenText.text = "50%";
            GameManager.Instance.LoadScreenBar.DOValue(0.5f, 0.1f);
            await Task.Delay(500);
        };

        while (!handle.IsDone)
        {
            Tween tween = GameManager.Instance.LoadScreenBar.DOValue(handle.PercentComplete, 0.1f);

            GameManager.Instance.LoadScreenText.text = Mathf.RoundToInt(handle.PercentComplete * 50f).ToString() + "%";

            await tween.AsyncWaitForCompletion();
        }

        if (GameManager.Instance.CurrentGameSceneObject != null)
            await GameManager.Instance.CurrentGameSceneObject.InitScene();

        GameManager.Instance.LoadScreenText.text = "100%";
        GameManager.Instance.LoadScreenBar.DOValue(1f, 0.1f);
        await Task.Delay(500);
        GameManager.Instance.LoadScreen.SetActive(false);
        GameManager.Instance.MainSceneObject.SetActive(false);
    }

    public async Task UnloadScene(SceneName scene, bool isGoingToLoad = false)
    {
        GameManager.Instance.LoadScreenText.text = "0%";
        GameManager.Instance.LoadScreenBar.value = 0;
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        AsyncOperation operation = SceneManager.UnloadSceneAsync(scene.ToString());
        operation.completed += async (operation) =>
        {
            if(!isGoingToLoad)
            {
                GameManager.Instance.LoadScreenText.text = "100%";
                GameManager.Instance.LoadScreenBar.DOValue(1, 0.1f);
                await Task.Delay(500);
            }
            else if(CurrentSceneObject != null)
            {
                CurrentSceneObject.SetActive(false);
                CurrentSceneObject = null;
            }
            GameManager.Instance.LoadScreen.SetActive(false);
            GameManager.Instance.MainSceneObject.SetActive(true);
        };

        while (!operation.isDone)
        {
            Tween tween = GameManager.Instance.LoadScreenBar.DOValue(operation.progress, 0.1f);

            GameManager.Instance.LoadScreenText.text = Mathf.RoundToInt(Mathf.Clamp01(operation.progress / 0.9f) * 100f).ToString() + "%";

            await tween.AsyncWaitForCompletion();
        }

    }

}

public enum SceneName
{
    MainScene = 0,
    Block = 1,
    Quiz = 2,
}
