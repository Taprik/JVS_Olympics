using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using Object = UnityEngine.Object;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.SceneManagement;
using System.Linq;

[CreateAssetMenu(fileName = "AddressablesManager", menuName = "Manager/AddressablesManager")]
public class AddressablesManager : ScriptableObject
{
    public AssetLabelReference LoadAtStart => _loadAtStart;
    [SerializeField]
    AssetLabelReference _loadAtStart;

    [SerializeField]
    List<GameSO> _allGameSo;

    public async void Init()
    {
        await LoadAssets<Object>(_loadAtStart, false);

        Task[] _gameInitTasks = new Task[_allGameSo.Count];

        for (int i = 0; i < _gameInitTasks.Length; i++)
            _gameInitTasks[i] = _allGameSo[i].GameInit();

        await LoadScreen(_gameInitTasks);
    }

    public async Task LoadAssets<T>(AssetReference[] references, bool closeAtEnd = true) where T : Object
    {
        foreach(AssetReference reference in references)
        {
            AsyncOperationHandle<T> handle = reference.LoadAssetAsync<T>();
            await LoadScreen(handle, closeAtEnd);
        }
    }

    public async Task LoadAssets<T>(AssetLabelReference label, bool closeAtEnd = true) where T : Object
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, callback => 
        {
            Debug.Log(callback.ToString() + " Succeeded to Load");
        });
        await LoadScreen(handle, closeAtEnd);
    }

    public async Task InstantiatesAsset(AssetReference[] references, bool closeAtEnd = true)
    {
        for (int i = 0; i < references.Length; i++)
        {
            AsyncOperationHandle<GameObject> handle = references[i].InstantiateAsync();
            await LoadScreen(handle, closeAtEnd);
        }
    }

    public async Task InstantiatesAsset(AssetLabelReference label, bool closeAtEnd = true)
    {
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(label, obj =>
        {
            Instantiate(obj);
        });
        await LoadScreen(handle, closeAtEnd);
    }

    public async Task LoadScreen(AsyncOperationHandle handle, bool closeAtEnd = true)
    {
        GameManager.Instance.SetLoadBar(0, 0);
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        handle.Completed += async (handle) =>
        {
            GameManager.Instance.SetLoadBar(1, 0.1f);
            await Task.Delay(500);
            if(closeAtEnd) GameManager.Instance.LoadScreen.SetActive(false);
        };

        while(!handle.IsDone)
        {
            Tween tween = GameManager.Instance.LoadScreenBar.DOValue(handle.PercentComplete, 0.1f);

            GameManager.Instance.LoadScreenText.text = Mathf.RoundToInt(handle.PercentComplete * 100f).ToString() + "%";

            await tween.AsyncWaitForCompletion();
        }
    }

    public async Task LoadScreen(Task task, bool closeAtEnd = true)
    {
        GameManager.Instance.SetLoadBar(0, 0);
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        while (!task.IsCompleted)
        {
            await Task.Delay(50);
        }

        GameManager.Instance.SetLoadBar(1, 0.1f);
        await Task.Delay(500);
        if(closeAtEnd) GameManager.Instance.LoadScreen.SetActive(false);
    }

    public async Task LoadScreen(Task[] tasks, bool closeAtEnd = true)
    {
        GameManager.Instance.SetLoadBar(0, 0);
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        while (!tasks.ToList().TrueForAll(x => x.IsCompleted == true))
        {
            float value = tasks.ToList().FindAll(x => x.IsCompleted == true).Count / (float)tasks.Length;
            GameManager.Instance.SetLoadBar(value, 0.1f);

            await Task.Delay(50);
        }

        GameManager.Instance.SetLoadBar(1, 0.1f);
        await Task.Delay(500);
        if(closeAtEnd) GameManager.Instance.LoadScreen.SetActive(false);
    }
}
