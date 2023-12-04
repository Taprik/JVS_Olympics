using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

//[CreateAssetMenu(fileName = "Condition", menuName = "Game/Taches/Condition")]
public class Condition : ScriptableObject
{
    public virtual void Init() { }

    public async virtual Task<bool> CheckCondition()
    {
        return true;
    }
}
