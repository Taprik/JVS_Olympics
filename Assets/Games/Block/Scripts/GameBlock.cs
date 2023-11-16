using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Game_Block", menuName = "Game/Block/Game_Block")]
public class GameBlock : GameSO
{
    public List<Texture2D> _texture2D = new();
}
