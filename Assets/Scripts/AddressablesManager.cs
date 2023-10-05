using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

[CreateAssetMenu(fileName = "AddressablesManager", menuName = "Manager/AddressablesManager")]
public class AddressablesManager : ScriptableObject
{
    public AssetLabelReference LoadAtStart => _loadAtStart;
    [SerializeField]
    AssetLabelReference _loadAtStart;

    public void Init()
    {
         InstantiateAsset(_loadAtStart);
    }

    public async void LoadAsset<T>(AssetReference[] references) where T : Object
    {
        foreach(AssetReference reference in references)
        {
            AsyncOperationHandle<T> handle = reference.LoadAssetAsync<T>();
            await handle.Task;
        }
    }

    public async void LoadAsset<T>(AssetLabelReference label) where T : Object
    {
        AsyncOperationHandle<T> handle = Addressables.LoadAssetAsync<T>(label);
        await handle.Task;
    }

    public async void InstantiateAsset(AssetReference[] references)
    {
        for (int i = 0; i < references.Length; i++)
        {
            AsyncOperationHandle<GameObject> handle = references[i].InstantiateAsync();
            await handle.Task;
        }
    }

    public async void InstantiateAsset(AssetLabelReference label)
    {
        AsyncOperationHandle<GameObject> handle = Addressables.InstantiateAsync(label);
        await handle.Task;
    }
}
