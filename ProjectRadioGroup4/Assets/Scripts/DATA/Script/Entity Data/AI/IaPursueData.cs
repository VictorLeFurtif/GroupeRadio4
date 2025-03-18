using System.Collections;
using System.Collections.Generic;
using AI;
using DATA.Script.Entity_Data.AI;
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

    public override AbstractEntityDataInstance Instance(GameObject entity)
    {
        return new IaPursueDataInstance(this,entity);
    }
}

public class IaPursueDataInstance : AbstractEntityDataInstance
{
    public int rangeSight;
    public float moveSpeed;
    public int rangeSightAsp;
    
    public IaPursueDataInstance(IaPursueData data, GameObject entity) : base(data,entity)
    {
        rangeSight = data.RangeSight;
        moveSpeed = data.MoveSpeed;
        rangeSightAsp = data.RangeSightAsp;
    }
}