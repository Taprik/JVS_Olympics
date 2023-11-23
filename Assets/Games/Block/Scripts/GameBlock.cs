using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game_Block", menuName = "Game/Block/Game_Block")]
public class GameBlock : GameSO
{
    public string ImagePath => _imagePath;
    const string _imagePath = "C:\\Users\\psuchet\\Documents\\JVS_Olympics\\Personnalisation\\Blocks";

    public int[] NbDivision;
}
