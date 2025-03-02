using System.Collections;
using System.Collections.Generic;
using DATA.ScriptData.Entity_Data;
using UnityEngine;

public class IaPursueData : AbstractEntityData
{
    [field: Header("Range Sight"), SerializeField]
    public int RangeSight { get; private set; }
    
    [field: Header("Move Speed"), SerializeField]
    public float MoveSpeed { get; private set; }

    public override AbstractEntityDataInstance Instance()
    {
        return new IaPursueDataInstance(this);
    }
}

public class IaPursueDataInstance : AbstractEntityDataInstance
{
    public int rangeSight;
    public float moveSpeed;
    
    public IaPursueDataInstance(IaPursueData data) : base(data)
    {
        rangeSight = data.RangeSight;
        moveSpeed = data.MoveSpeed;
    }
}