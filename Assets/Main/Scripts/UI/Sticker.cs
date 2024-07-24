using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class Sticker : MonoBehaviour
{
    [SerializeField] private bool _bintro;
    private string _path;
    private Image _img;
    [SerializeField] private GameObject[] _gos;
    [SerializeField] string _playerPrefPath;

    void Start()
    {
        _img = GetComponent<Image>();
        _img.enabled = false;
        string appPath;
        string newPath;
        appPath = Application.dataPath;
        newPath = Path.GetFullPath(Path.Combine(appPath, @"../../../../"));
        _path = newPath + "Personnalisation\\sticker";

#if UNITY_EDITOR
        _path = $"{Path.GetFullPath(Path.Combine(appPath, @"../../../../"))}Documents\\Capteur\\Personnalisation\\sticker";
#endif

        Debug.Log(_path + ".png");

        if (PlayerPrefs.HasKey(_playerPrefPath) && _playerPrefPath != string.Empty)
        {
            if (PlayerPrefs.GetInt(_playerPrefPath) == 1)
                StartCoroutine(LoadImage());
        }
        else
        {
            StartCoroutine(LoadImage());
        }
    }

    IEnumerator LoadImage()
    {
        string imagesPath;

        if (!_bintro)
        {
            imagesPath = _path + @"\sticker.png";
        }
        else
        {
            imagesPath = _path + @"\stickerAccueilTous.png";
        }

        Texture2D tex;
        tex = new Texture2D(2, 2);
        WWW www = new WWW(imagesPath);

        //Debug.Log(imagesPath);
        while (!www.isDone)
            yield return null;
        if (www.error != null)
        {
            Debug.LogWarning("Image WWW ERROR: " + www.error);

            yield return LoadImageJGP();
        }
        else
        {

            www.LoadImageIntoTexture(tex);

            //_img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            _img.sprite = Tool.ToolBox.CreateSpriteFromTexture(tex);
            _img.enabled = true;
        }
    }

    IEnumerator LoadImageJGP()
    {
        string imagesPath;

        if (!_bintro)
        {
            imagesPath = _path + @"\sticker.jpg";
        }
        else
        {
            imagesPath = _path + @"\stickerAccueilTous.jpg";
        }

        Texture2D tex;
        tex = new Texture2D(2, 2);
        WWW www = new WWW(imagesPath);

        //Debug.Log(imagesPath);
        while (!www.isDone)
            yield return null;
        if (www.error != null)
        {
            Debug.LogError("Image WWW ERROR: " + www.error);

            foreach (GameObject go in _gos)
            {
                go.SetActive(false);
            }

        }
        else
        {

            www.LoadImageIntoTexture(tex);

            //_img.sprite = Sprite.Create(tex, new Rect(0, 0, tex.width, tex.height), new Vector2(0.5f, 0.5f));
            _img.sprite = Tool.ToolBox.CreateSpriteFromTexture(tex);
            _img.enabled = true;
        }
    }
}
