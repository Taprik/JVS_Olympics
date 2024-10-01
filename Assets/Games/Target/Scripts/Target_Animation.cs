using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Target_Animation : MonoBehaviour
{
    [SerializeField] private SpriteRenderer[] sprites;

    public int stepAnim;
    [HideInInspector] public GameObject shadow;

    // Use this for initialization
    void Start()
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = sprites[0].sortingOrder;
        }
        stepAnim = 1;
    }

    void goStep2()
    {
        stepAnim = 2;
    }

    void goStep3()
    {
        stepAnim = 3;
    }

    void goStep4()
    {
        stepAnim = 4;
    }

    int getStep()
    {
        return stepAnim;
    }

    public void SetActiveF()
    {
        gameObject.SetActive(false);
    }

    public void SetSorting(int number)
    {
        for (int i = 0; i < sprites.Length; i++)
        {
            sprites[i].sortingOrder = number;
        }
    }
}
