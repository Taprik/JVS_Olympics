using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[System.Serializable]
public class MultipleTaches
{
    [SerializeField]
    ConditionOption _option;

    public Condition[] Condition => _condition;
    [SerializeField, Header("If :")]
    protected Condition[] _condition;

    public Tache[] Taches => _taches;
    [SerializeField, Header("Do :")]
    Tache[] _taches;

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
            Task[] tasks = new Task[_condition.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _condition[i].CheckCondition(out bool temp);
                conditionOk = !temp ? temp : conditionOk;
            }

            await Task.WhenAll(tasks);
        }

        for (int i = 0; i < _taches.Length; i++)
        {
            if (conditionOk)
            {
                _taches[i].CallBack += (callBack) =>
                {
                    CallBack?.Invoke(callBack);
                };
                await _taches[i].DoWork();
            }
            else _taches[i].BadEnd();
        }
        
    }

    public async Task DoWorkAny()
    {
        bool conditionOk = false;

        if (_condition != null)
        {
            Task[] tasks = new Task[_condition.Length];

            for (int i = 0; i < tasks.Length; i++)
            {
                tasks[i] = _condition[i].CheckCondition(out bool temp);
                conditionOk = temp ? temp : conditionOk;
            }

            await Task.WhenAny(tasks);
        }

        for (int i = 0; i < _taches.Length; i++)
        {
            if (conditionOk)
            {
                _taches[i].CallBack += (callBack) =>
                {
                    CallBack?.Invoke(callBack);
                };
                await _taches[i].DoWork();
            }
            else _taches[i].BadEnd();
        }
    }
}

public enum ConditionOption
{
    Any,
    All
}
