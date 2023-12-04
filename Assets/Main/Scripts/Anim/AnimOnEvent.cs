using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class AnimOnEvent : MonoBehaviour
{
    [SerializeField]
    TriggerType _trigger;

    [SerializeField]
    Animator _animator;

    [SerializeField]
    AnimVariableType _variableType;

    [SerializeField]
    string _parameterName;

    [SerializeField, Header("Value Parameter")]
    int _intValue;

    [SerializeField]
    float _floatValue;

    [SerializeField]
    bool _boolValue;

    public void OnDestroy()
    {
        if (_trigger == TriggerType.OnDestroy)
            Action();
    }

    public void Action()
    {
        switch (_variableType)
        {
            case AnimVariableType.Int:
                _animator.SetInteger(_parameterName, _intValue);
                break;

            case AnimVariableType.Float:
                _animator.SetFloat(_parameterName, _floatValue);
                break;

            case AnimVariableType.Bool:
                _animator.SetBool(_parameterName, _boolValue);
                break;

            case AnimVariableType.Trigger:
                _animator.SetTrigger(_parameterName);
                break;

            default:
                break;
        }
    }
}

public enum AnimVariableType
{
    Int,
    Float,
    Bool,
    Trigger
}

public enum TriggerType
{
    OnDestroy
}
