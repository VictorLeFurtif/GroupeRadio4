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

        [field: Header("Animation Take Damage for Player"), SerializeField]
        public Animation TakeDamageAnimationPlayer{ get; private set; }
        
        [field: Header("Animation Fm"), SerializeField]
        public Animation FmAnimation{ get; private set; }
        
        [field: Header("Animation Am"), SerializeField]
        public Animation AmAnimation{ get; private set; }
        
        [field: Header("Boolean pour activer désactiver le gros bouclier"),SerializeField]
        public bool GrosBouclier { get; private set; }
        
        [field: Header("Boolean pour activer désactiver le Classic Echo"),SerializeField]
        public bool ClassicEcho { get; private set; }
        
        [field: Header("Cherie Bomb"),SerializeField] public bool CherieBomb { get; private set; }

        public override AbstractEntityDataInstance Instance(GameObject entity)
        {
            return new PlayerDataInstance(this,entity);
        }
    }

    [Serializable]
    public class PlayerDataInstance : AbstractEntityDataInstance
    {
        public int moveSpeed;
        public readonly Animation takeDamageAnimation;
        public readonly Animation fmAnimation;
        public readonly Animation amAnimation;
        public bool grosBouclier;
        public bool classicEcho;
        public bool cherieBomb;

        public PlayerDataInstance(PlayerData data,GameObject entity) : base(data,entity)
        {
            moveSpeed = data.MoveSpeed;
            takeDamageAnimation = data.TakeDamageAnimationPlayer;
            fmAnimation = data.FmAnimation;
            amAnimation = data.AmAnimation;
            grosBouclier = data.GrosBouclier;
            classicEcho = data.ClassicEcho;
            cherieBomb = data.CherieBomb;
        }
    
    }
}