using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public abstract class GameSO : ScriptableObject
{
    public abstract Task GameInit();
}
