using System.Collections;
using System.Collections.Generic;
using DATA.ScriptData.Entity_Data;
using UnityEngine;

[CreateAssetMenu(menuName = "ScriptableObject/AiPursueData", fileName = "data")]
public class IaPursueData : AbstractEntityData
{
    [field: Header("Range Sight"), SerializeField]
    public int RangeSight { get; private set; }
    
    [field: Header("Move Speed"), SerializeField]
    public float MoveSpeed { get; private set; }
    
    [field: Header("Range Sight after Seeing Player"), SerializeField]
    public int RangeSightAsp { get; private set; }

    public override AbstractEntityDataInstance Instance()
    {
        return new IaPursueDataInstance(this);
    }
}

public class IaPursueDataInstance : AbstractEntityDataInstance
{
    public int rangeSight;
    public float moveSpeed;
    public int rangeSightAsp;
    
    public IaPursueDataInstance(IaPursueData data) : base(data)
    {
        rangeSight = data.RangeSight;
        moveSpeed = data.MoveSpeed;
        rangeSightAsp = data.RangeSightAsp;
    }
}