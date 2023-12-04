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
        _event.RegisterListener(() =>
        {
            Debug.Log("Success");
            _actionComplete = true;
            return null;
        });
    }

    public async override Task<bool> CheckCondition()
    {
        await Task.Run(async () =>
        {
            while (!_actionComplete)
            {
                await Task.Delay(1);
            }
        });
        return true;
    }
}
