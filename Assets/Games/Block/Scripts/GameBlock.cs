using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using TMPro;
using Tool;
using UnityEngine;

namespace Blocks
{
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

        public AudioClip AudioWin;
        public AudioClip AudioClic;

        [SerializeField]
        TMP_FontAsset[] WinFont;
        public TMP_FontAsset GetWinFont(int id) => id < WinFont.Length && id >= 0 ? WinFont[id] : null;

        public List<ImageData> ImageDatas => _imageDatas;
        List<ImageData> _imageDatas = new();

        public override async Task GameInit()
        {
            try
            {
                _imageDatas?.Clear();
                string path = Path.GetFullPath(Path.Combine(Application.dataPath, @"..\..\..\..\"));
                path = Path.GetFullPath(Path.Combine(path, ImagePath));
                Debug.Log("Blocks Path : " + path);
                List<string> imagePath = ToolBox.GetFiles(path, new string[2] { "*.jpg", "*.png" });

                for (int i = 0; i < imagePath.Count; i++)
                {
                    ImageData data = new();
                    data.Texture = ToolBox.CreateTextureFromPath(imagePath[i]);
                    await UnityMainThreadDispatcher.Instance().EnqueueAsync(() =>
                    {
                        data.ImageSplit?.Clear();
                        data.ImageSplit = new();

                        for (int i = 0; i < NbDivision.Length; i++)
                        {
                            data.ImageSplit.Add(SplitImage(data.Texture, NbDivision[i]));
                        }
                        data.FullImage = ToolBox.CreateSpriteFromTexture(data.Texture);
                    });
                    _imageDatas.Add(data);
                }

                while (_imageDatas.Count < imagePath.Count)
                    await Task.Delay(50);
                Debug.LogWarning("End Load All : " + _imageDatas.Count);
            }
            catch (System.Exception ex)
            {
                Debug.LogError(ex.Message);
            }
        }

        Sprite[,] SplitImage(Texture2D texture, int nbDivision)
        {
            Sprite[,] sprites = new Sprite[nbDivision, nbDivision];

            for (int i = 0; i < nbDivision; i++)
            {
                for (int j = 0; j < nbDivision; j++)
                {
                    float h = texture.height / nbDivision;
                    float w = texture.width / nbDivision;
                    sprites[i, j] = Sprite.Create(texture, new Rect(i * w, j * h, w, h), new Vector2(0.5f, 0.5f));
                }
            }
            return sprites;
        }

    }

    [System.Serializable]
    public struct ImageData
    {
        public Texture2D Texture;
        public Sprite FullImage;
        public List<Sprite[,]> ImageSplit;
    }
}