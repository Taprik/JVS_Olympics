using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "C_WaitUnityAction", menuName = "Game/Taches/C_WaitUnityAction")]
public class C_WaitUnityAction : Condition
{
    [SerializeField]
    UnityAction _action;

    bool _actionComplete;

    public override void Init()
    {
        _actionComplete = false;
        _action += () =>
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
