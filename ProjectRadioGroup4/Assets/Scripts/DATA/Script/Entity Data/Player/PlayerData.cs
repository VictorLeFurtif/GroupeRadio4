using System;
using DATA.Script.Entity_Data.AI;
using UnityEngine;

namespace DATA.Script.Entity_Data.Player
{
    [CreateAssetMenu(menuName = "SciptableObject/PlayerData", fileName = "PlayerData")]
    public class PlayerData : AbstractEntityData
    {
        [field: Header("Move Speed"), SerializeField]
        public int MoveSpeed { get; private set; }
        
        public override AbstractEntityDataInstance Instance(GameObject entity)
        {
            return new PlayerDataInstance(this,entity);
        }
    }

    [Serializable]
    public class PlayerDataInstance : AbstractEntityDataInstance
    {
        public int moveSpeed;

        public PlayerDataInstance(PlayerData data,GameObject entity) : base(data,entity)
        {
            moveSpeed = data.MoveSpeed;
        }
    
    }

    
}