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

[CreateAssetMenu(fileName = "AddressablesManager", menuName = "Manager/AddressablesManager")]
public class AddressablesManager : ScriptableObject
{
    public AssetLabelReference LoadAtStart => _loadAtStart;
    [SerializeField]
    AssetLabelReference _loadAtStart;

    public async void Init()
    {
        await LoadAssets<Object>(_loadAtStart);
    }

    public async Task LoadAssets<T>(AssetReference[] references) where T : Object
    {
        foreach(AssetReference reference in references)
        {
            AsyncOperationHandle<T> handle = reference.LoadAssetAsync<T>();
            await LoadScreen(handle);
        }
    }

    public async Task LoadAssets<T>(AssetLabelReference label) where T : Object
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, callback => 
        {
            Debug.Log(callback.ToString() + " Succeeded to Load");
        });
        await LoadScreen(handle);
    }

    public async Task InstantiatesAsset(AssetReference[] references)
    {
        for (int i = 0; i < references.Length; i++)
        {
            AsyncOperationHandle<GameObject> handle = references[i].InstantiateAsync();
            await LoadScreen(handle);
        }
    }

    public async Task InstantiatesAsset(AssetLabelReference label)
    {
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(label, obj =>
        {
            Instantiate(obj);
        });
        await LoadScreen(handle);
    }

    public async Task LoadScreen(AsyncOperationHandle handle)
    {
        GameManager.Instance.LoadScreenText.text = "0%";
        GameManager.Instance.LoadScreenBar.value = 0;
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        handle.Completed += async (handle) =>
        {
            GameManager.Instance.LoadScreenText.text = "100%";
            GameManager.Instance.LoadScreenBar.DOValue(1, 0.1f);
            await Task.Delay(500);
            GameManager.Instance.LoadScreen.SetActive(false);
        };

        while(!handle.IsDone)
        {
            Tween tween = GameManager.Instance.LoadScreenBar.DOValue(handle.PercentComplete, 0.1f);

            GameManager.Instance.LoadScreenText.text = Mathf.RoundToInt(handle.PercentComplete * 100f).ToString() + "%";

            await tween.AsyncWaitForCompletion();
        }
    }

    public async Task LoadScreen(Task task)
    {

        GameManager.Instance.LoadScreenText.text = "0%";
        GameManager.Instance.LoadScreenBar.value = 0;
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        while (!task.IsCompleted)
        {
            await Task.Delay(50);
        }

        GameManager.Instance.LoadScreenText.text = "100%";
        GameManager.Instance.LoadScreenBar.DOValue(1, 0.1f);
        await Task.Delay(500);
        GameManager.Instance.LoadScreen.SetActive(false);
    }

    public async Task LoadScreen(Task[] tasks)
    {

        GameManager.Instance.LoadScreenText.text = "0%";
        GameManager.Instance.LoadScreenBar.value = 0;
        GameManager.Instance.LoadScreen.SetActive(true);
        await Task.Delay(50);

        await Task.WhenAll(tasks);

        GameManager.Instance.LoadScreenText.text = "100%";
        GameManager.Instance.LoadScreenBar.DOValue(1, 0.1f);
        await Task.Delay(500);
        GameManager.Instance.LoadScreen.SetActive(false);
    }
}
