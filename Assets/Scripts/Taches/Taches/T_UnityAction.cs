using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "T_UnityAction", menuName = "Game/Taches/T_UnityAction")]
public class T_UnityAction : Tache
{
    [SerializeField]
    UnityAction _action;

    public override async Task DoTask()
    {
        await Task.Run(() =>
        {
            _action?.Invoke();
        });
    }
}
