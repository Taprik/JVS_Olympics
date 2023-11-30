using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class RandomTransitionSetter : MonoBehaviour
{
    [SerializeField] private List<TransitionParams> _transitions;
    private TransitionParams _randomTransition;
    public TransitionParams GetTransition() 
    {
        return _randomTransition;
    }
    // Start is called before the first frame update
    void Awake()
    {
        ChooseTransitionRandom();
    }

    public void ChooseTransitionRandom() 
    {
        _randomTransition = _transitions[Random.Range(0, _transitions.Count)];
        Transitioner.Instance._widthOfTransitionInBlocks = _randomTransition.BlockWidth;
        Transitioner.Instance._transitionBlockPrefab = _randomTransition.BlockPrefab;
        Transitioner.Instance._transitionBlockSprite = _randomTransition.BlockSprite;
        Transitioner.Instance._transitionBlockColor = _randomTransition.BlockColor;
        Transitioner.Instance._transitionBlockAnimationTime = _randomTransition.BlockAnimationTime;
        Transitioner.Instance._transitionOrderPrefab = _randomTransition.TransitionOrderPrefab;
        Transitioner.Instance._transitionTime = _randomTransition.TransitionTime;
    }

}

[Serializable]
public struct TransitionParams
{
    public int BlockWidth;
    public GameObject BlockPrefab;
    public Sprite BlockSprite;
    public Color BlockColor;
    public float BlockAnimationTime;
    public GameObject TransitionOrderPrefab;
    public float TransitionTime;

}
