using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace DATA.Script.Attack_Data
{
    
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack", fileName = "playerAttack")]
    public class PlayerAttack : ScriptableObject
    {
        public enum AttackState
        {
            Am,
            Fm,
        }
        
        [Serializable] public struct AttackClassic
        {
            public float damage;
            public float chanceOfOverload;
            public string name;
            //potentiel effet
            public float indexFrequency;
            public AttackState attackState;
        }
        
        [field:Header("DeadZone"),SerializeField] public bool DeadZone { get; private set; }
        [field: Header("Am Attack Parameters"),SerializeField] public AttackClassic AttackP { get; private set; }

        public PlayerAttackInstance Instance()
        {
            return new PlayerAttackInstance(this);
        }
    }
    
    [Serializable]
    public class PlayerAttackInstance
    {
        public PlayerAttack.AttackClassic attack;
        public bool deadZone;
        
        public PlayerAttackInstance(PlayerAttack data)
        {
            attack = data.AttackP;
            deadZone = data.DeadZone;
        }
    }
}
