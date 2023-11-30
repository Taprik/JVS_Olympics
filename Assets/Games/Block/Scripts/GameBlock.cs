using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game_Block", menuName = "Game/Block/Game_Block")]
public class GameBlock : GameSO
{
    public string ImagePath => _imagePath;
    const string _imagePath =
#if UNITY_EDITOR
        "C:\\Users\\smartJeux\\Documents\\Capteur\\Personnalisation\\Blocks";
#else
        "Personnalisation\\Blocks";
#endif

    public int[] NbDivision;
}
