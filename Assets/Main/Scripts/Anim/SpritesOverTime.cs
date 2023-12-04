using System.Collections;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SpritesOverTime : MonoBehaviour
{
    [SerializeField]
    Image _image;

    [SerializeField]
    private Sprite[] _sprites;

    public async Task Anim(float timer, CancellationToken token)
    {
        for (int i = 0; i < _sprites.Length; i++)
        {
            if(token.IsCancellationRequested) return;

            _image.sprite = _sprites[i];
            await Task.Delay(Mathf.RoundToInt(timer / _sprites.Length * 1000));
        }
    }

    public async Task Anim(float timer)
    {
        for (int i = 0; i < _sprites.Length; i++)
        {
            _image.sprite = _sprites[i];
            await Task.Delay(Mathf.RoundToInt(timer / _sprites.Length * 1000));
        }
    }
}
