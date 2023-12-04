using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class MultipleTaches
{
    [SerializeField]
    ConditionOption _option;

    public Condition[] Condition => _condition;
    [SerializeField, Header("If :")]
    Condition[] _condition;

    public TacheType[] Taches => _taches;
    [SerializeField, Header("Do :")]
    TacheType[] _taches;

    [Serializable]
    public struct TacheType
    {
        public Tache tache;
        public UnityEvent unityEvent;
    }

    public Action<bool> CallBack;

    public async Task DoWork()
    {
        switch (_option)
        {
            case ConditionOption.Any:
                await DoWorkAny();
                break;

            case ConditionOption.All:
                await DoWorkAll();
                break;

            default:
                Debug.LogError("No Option Selected");
                break;
        }
    }

    public async Task DoWorkAll()
    {
        bool conditionOk = true;

        if (_condition != null)
        {
            Task<bool>[] tasks = new Task<bool>[_condition.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _condition[i].CheckCondition();
            }

            await Task.WhenAll(tasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                bool temp = tasks[i].Result;
                conditionOk = !temp ? temp : conditionOk;
            }
        }

        for (int i = 0; i < _taches.Length; i++)
        {
            if (_taches[i].tache != null)
            {
                Tache currentTache = _taches[i].tache;
                if (conditionOk)
                {
                    currentTache.CallBack += (callBack) =>
                    {
                        CallBack?.Invoke(callBack);
                    };
                    await currentTache.DoWork();
                }
                else currentTache.BadEnd();
            }
            
            if (_taches[i].unityEvent != null)
            {
                if (conditionOk)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _taches[i].unityEvent.Invoke());
                }
            }
        }
        
    }

    public async Task DoWorkAny()
    {
        bool conditionOk = false;

        if (_condition != null)
        {
            Task<bool>[] tasks = new Task<bool>[_condition.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _condition[i].CheckCondition();
            }

            await Task.WhenAny(tasks);

            for (int i = 0; i < tasks.Length; i++)
            {
                if (!tasks[i].IsCompleted) continue;

                bool temp = tasks[i].Result;
                conditionOk = !temp ? temp : conditionOk;
            }

            Debug.Log(conditionOk);
        }

        for (int i = 0; i < _taches.Length; i++)
        {
            if (_taches[i].tache != null)
            {
                Tache currentTache = _taches[i].tache;
                if (conditionOk)
                {
                    currentTache.CallBack += (callBack) =>
                    {
                        CallBack?.Invoke(callBack);
                    };
                    await currentTache.DoWork();
                }
                else currentTache.BadEnd();
            }

            if (_taches[i].unityEvent != null)
            {
                if (conditionOk)
                {
                    UnityMainThreadDispatcher.Instance().Enqueue(() => _taches[i].unityEvent?.Invoke());
                }
            }
        }
    }
}

public enum ConditionOption
{
    Any,
    All
}
