using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//[CreateAssetMenu(fileName = "Tache", menuName = "Game/Taches/Tache")]
public class Tache : ScriptableObject
{
    public Action<bool> CallBack;

    public async Task DoWork()
    {
        Task tache = DoTask();
        await tache;

        if (tache.IsCompletedSuccessfully) GoodEnd();
        else BadEnd();
    }

    public void GoodEnd()
    {
        CallBack?.Invoke(true);
    }

    public void BadEnd()
    {
        CallBack?.Invoke(false);
    }

    public virtual async Task DoTask()
    {
        await Task.Run(() => { });
    }
}
