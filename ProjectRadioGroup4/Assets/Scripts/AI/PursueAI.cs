using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PursueAI : AbstractAI
{
    private IaPursueDataInstance _iaPursueDataInstance;
    
    protected override void AiShift()
    {
        throw new System.NotImplementedException();
    }

    protected override void Init()
    {
        base.Init();
        _iaPursueDataInstance = (IaPursueDataInstance)_abstractEntityDataInstance;
    }
}
