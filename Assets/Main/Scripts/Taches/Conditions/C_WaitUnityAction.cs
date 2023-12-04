using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "C_WaitUnityAction", menuName = "Game/Conditions/WaitUnityAction")]
public class C_WaitUnityAction : Condition
{
    [SerializeField]
    E_UnityEvent _event;

    bool _actionComplete;

    public override void Init()
    {
        _actionComplete = false;
        _event.Action.AddListener(() => _actionComplete = true);
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
