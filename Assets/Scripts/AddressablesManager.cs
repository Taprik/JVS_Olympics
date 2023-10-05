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
         InstantiatesAsset(_loadAtStart);
    }

    public async void LoadAssets<T>(AssetReference[] references) where T : Object
    {
        foreach(AssetReference reference in references)
        {
            AsyncOperationHandle<T> handle = reference.LoadAssetAsync<T>();
            await handle.Task;
        }
    }

    public async void LoadAssets<T>(AssetLabelReference label) where T : Object
    {
        AsyncOperationHandle<IList<T>> handle = Addressables.LoadAssetsAsync<T>(label, callback => 
        {
            Debug.Log(callback.ToString() + " Succeeded to Load");
        });
        await handle.Task;
    }

    public async void InstantiatesAsset(AssetReference[] references)
    {
        for (int i = 0; i < references.Length; i++)
        {
            AsyncOperationHandle<GameObject> handle = references[i].InstantiateAsync();
            await handle.Task;
        }
    }

    public async void InstantiatesAsset(AssetLabelReference label)
    {
        AsyncOperationHandle<IList<GameObject>> handle = Addressables.LoadAssetsAsync<GameObject>(label, obj =>
        {
            Instantiate(obj);
        });
        await handle.Task;
    }
}
