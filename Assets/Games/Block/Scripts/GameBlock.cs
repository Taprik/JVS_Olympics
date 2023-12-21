using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

[CreateAssetMenu(fileName = "Game_Block", menuName = "Game/Block/Game_Block")]
public class GameBlock : GameSO
{
    public string ImagePath => _imagePath;
    const string _imagePath =
#if UNITY_EDITOR
        "C:\\Users\\smartJeux\\Documents\\Capteur\\Personnalisation\\Blocks";
#else
        "Documents\\Capteur\\Personnalisation\\Blocks";
#endif

    public int[] NbDivision;

    public AudioClip AudioWin;
    public AudioClip AudioClic;

    [SerializeField]
    TMP_FontAsset[] WinFont;
    public TMP_FontAsset GetWinFont(int id) => id < WinFont.Length && id >= 0 ? WinFont[id] : null;
}
