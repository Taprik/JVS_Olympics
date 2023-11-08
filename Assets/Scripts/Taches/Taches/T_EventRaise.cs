using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "T_EventRaise", menuName = "Game/Taches/EventRaise")]
public class T_EventRaise : Tache
{
    [SerializeField]
    EventSO _event;

    public override async Task DoTask()
    {
        await Task.Run(() =>
        {
            _event.Raise();
        });
    }
}
