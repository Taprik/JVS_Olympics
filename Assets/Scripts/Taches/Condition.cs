using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//[CreateAssetMenu(fileName = "Condition", menuName = "Game/Taches/Condition")]
public class Condition : ScriptableObject
{
    public virtual void Init() { }

    public virtual Task CheckCondition(out bool isOk)
    {
        isOk = true;
        return Task.CompletedTask;
    }
}
