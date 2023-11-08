using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "C_WaitEventSo", menuName = "Game/Conditions/WaitEventSo")]
public class C_WaitEventSo : Condition
{
    [SerializeField]
    EventSO _event;

    bool _actionComplete;

    public override void Init()
    {
        _actionComplete = false;
        _event.Action += () =>
        {
            _actionComplete = true;
        };
    }

    public override Task CheckCondition(out bool isOk)
    {
        Task.Run(() => _actionComplete).Wait();
        isOk = true;
        return Task.CompletedTask;
    }
}
