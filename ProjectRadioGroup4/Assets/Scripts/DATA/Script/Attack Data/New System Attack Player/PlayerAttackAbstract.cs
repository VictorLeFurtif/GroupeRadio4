using System;
using Controller;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/ AbstractAttack", fileName = "data")]
    public abstract class PlayerAttackAbstract : ScriptableObject
    {
        public enum AttackState
        {
            Default,
            Am,
            Fm,
        }
        
        [Serializable] public struct AttackClassic
        {
            public float damage;
            public float chanceOfOverload;
            public float damageMaxBonus;
            public string name;
            public AttackState attackState;
            public float indexFrequency;
            public float costBatteryExploration;
            public float costBatteryInFightMax;
            public float costBatteryInFight;
        }
        
        [field: Header("Attack Player"),SerializeField] public AttackClassic AttackP { get; private set; }
        
        [field: Header("Multiplicateur de vie prise"),SerializeField] public float MultiplicatorLifeTaken { get; private set; }
        
        public virtual PlayerAttackAbstractInstance Instance()
        {
            return new PlayerAttackAbstractInstance(this);
        }
    }
    
    
    [Serializable]
    public class PlayerAttackAbstractInstance
    {
        public PlayerAttackAbstract.AttackClassic attack;
        public float multiplicatorLifeTaken;
        
        public PlayerAttackAbstractInstance(PlayerAttackAbstract data)
        {
            attack = data.AttackP;
            multiplicatorLifeTaken = data.MultiplicatorLifeTaken;
        }
        
        public void TakeLifeFromPlayer()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.ManageLife(-attack.costBatteryInFightMax);
            }
        }
        
        public void TakeLifeFromPlayer(float damage)
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.ManageLife(-damage);
            }
        }

        
    }
}
