using UnityEngine;

namespace DATA.ScriptData.Entity_Data
{
    [CreateAssetMenu(menuName = "SciptableObject/PlayerData", fileName = "PlayerData")]
    public class PlayerData : AbstractEntityData
    {
        [field: Header("Move Speed"), SerializeField]
        public int MoveSpeed { get; private set; }

        public override AbstractEntityDataInstance Instance()
        {
            return new PlayerDataInstance(this);
        }
    }

    public class PlayerDataInstance : AbstractEntityDataInstance
    {
        public int moveSpeed;

        public PlayerDataInstance(PlayerData data) : base(data)
        {
            moveSpeed = data.MoveSpeed;
        }
    
    }
}