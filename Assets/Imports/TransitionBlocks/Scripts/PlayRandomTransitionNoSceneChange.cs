using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Transitioner)), RequireComponent(typeof(RandomTransitionSetter))]
public class PlayRandomTransitionNoSceneChange : MonoBehaviour
{
    private RandomTransitionSetter _randomTransitionSetter;
    private Transitioner _transitioner;
    [SerializeField] private float _timeModifier;
    public void Awake()
    {
        _randomTransitionSetter = GetComponent<RandomTransitionSetter>();
        _transitioner = GetComponent<Transitioner>();
    }

    public void Launch()
    {
        StartCoroutine(SceneIn());
    }
    //public IEnumerator SceneInOut()
    //{
    //    _transitioner.TransitionOutWithoutChangingScene();
    //    yield return new WaitForSeconds(_randomTransitionSetter.GetTransition().TransitionTime * _timeModifier);
    //    if (_transition != null)
    //    {
    //        _transition.Raise();
    //    }
    //    _transitioner.TransitionInWithoutChangingScene();
    //    yield return new WaitForSeconds(_randomTransitionSetter.GetTransition().TransitionTime * _timeModifier);
    //    if (_finished != null)
    //    {
    //        _finished.Raise();
    //    }
    //    _randomTransitionSetter.ChooseTransitionRandom();
    //}
    public IEnumerator SceneIn()
    {
        _transitioner.TransitionInWithoutChangingScene();

        yield return new WaitForSeconds(_randomTransitionSetter.GetTransition().TransitionTime * _timeModifier);

        _randomTransitionSetter.ChooseTransitionRandom();
    }
}
