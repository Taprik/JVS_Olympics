using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using Target;
using UnityEngine;

public class Target_Animation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] _main;
    [SerializeField] private Collider[] _mainCollider;
    [SerializeField] private SpriteRenderer[] _shadow;

    public IEnumerator Start()
    {
        for (int i = 0; i < _main.Length; i++)
        {
            _mainCollider[i].enabled = false;
            _main[i].material.DOFloat(360f, Shader.PropertyToID("_Arc1"), 0f);
            _shadow[i].material.DOFloat(360f, Shader.PropertyToID("_Arc1"), 0f);
        }

        //Debug.Log("Start");
        for (int i = 0; i < _main.Length; i++)
        {
            //Debug.Log(i);
            _mainCollider[i].enabled = true;
            _main[i].material.DOFloat(0f, Shader.PropertyToID("_Arc1"), 1f);
            _shadow[i].material.DOFloat(0f, Shader.PropertyToID("_Arc1"), 1f);
            yield return new WaitForSeconds(1.5f);
        }
    }

    public void OnKill()
    {
        Target_GameManager.Instance.Targets.Remove(this);
        Target_GameManager.Instance.AddPoint();
        Destroy(gameObject);
    }

    public void AddOrderInLayer(int nb)
    {
        for (int i = 0; i < _main.Length; i++)
        {
            _main[i].sortingOrder += nb;
            _shadow[i].sortingOrder += nb;
        }
    }
}
