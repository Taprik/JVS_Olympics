using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "T_ActionRaise", menuName = "Game/Taches/ActionRaise")]
public class T_ActionRaise : Tache
{
    [SerializeField]
    Action _action;

    public override async Task DoTask()
    {
        await Task.Run(() =>
        {
            _action?.Invoke();
        });
    }
}
