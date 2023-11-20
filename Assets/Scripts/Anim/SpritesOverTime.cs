using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpritesOverTime : MonoBehaviour
{
    [SerializeField]
    Image _image;

    [SerializeField]
    private Sprite[] _sprites;

    public IEnumerator Anim(float timer)
    {
        for (int i = 0; i < _sprites.Length; i++)
        {
            _image.sprite = _sprites[i];
            yield return new WaitForSeconds(timer / _sprites.Length);
        }
    }
}
