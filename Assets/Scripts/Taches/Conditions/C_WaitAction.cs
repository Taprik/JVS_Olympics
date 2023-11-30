using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

[CreateAssetMenu(fileName = "C_WaitAction", menuName = "Game/Conditions/WaitAction")]
public class C_WaitAction : Condition
{
    [SerializeField]
    Action _action;

    bool _actionComplete;

    public override void Init()
    {
        _actionComplete = false;
        _action += () =>
        {
            _actionComplete = true;
        };
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
