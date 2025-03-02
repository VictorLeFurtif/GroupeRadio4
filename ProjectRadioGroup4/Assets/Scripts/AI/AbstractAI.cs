using System;
using System.Collections;
using System.Collections.Generic;
using DATA.ScriptData.Entity_Data;
using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public abstract class AbstractAI : MonoBehaviour
{
    [SerializeField]
    private AbstractEntityData _abstractEntityData;
    protected AbstractEntityDataInstance _abstractEntityDataInstance;
    protected AiFightState _aiFightState;
    protected enum AiFightState
    {
        InFight,
        OutFight
    }
    
    protected abstract void AiShift();

    private void Start()
    {
        Init();
    }

    protected virtual void Init()
    {
        _abstractEntityDataInstance = _abstractEntityData.Instance();
        _aiFightState = AiFightState.OutFight;
    }
}
