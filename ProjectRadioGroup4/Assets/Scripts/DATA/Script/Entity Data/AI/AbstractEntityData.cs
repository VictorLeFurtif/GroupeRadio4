using System;
using AI;
using MANAGER;
using UnityEngine;
using UnityEngine.Serialization;

namespace DATA.Script.Entity_Data.AI
{
    [CreateAssetMenu(menuName = "ScriptableObject/AbstractIAData", fileName = "data")]
    
    public abstract class AbstractEntityData : ScriptableObject
    {
        [field: Header("Life"), SerializeField]
        public float Hp { get; private set; }
    
        [field: Header("Hidden Speed"),SerializeField]
        public int Speed { get; private set; }

        [field: Header("Turn State"), SerializeField]
        public FightManager.TurnState TurnState;
    
        [field: Header("Wave amplitude"),SerializeField,Tooltip("Max O.4f please")]
        public float WaveAmplitudeEnemy { get; private set; }
        
        [field: Header("Wave frequency"),SerializeField, Tooltip("Max 15f please")]
        public float WaveFrequencyEnemy { get; private set; }
        
        [field: Header("Seen by Radio"),SerializeField]
        public bool SeenByRadio { get; private set; }

        [field: Header("Hidden"),SerializeField]
        public bool NotHidden { get; private set; }
        
        [field: Header("Battery "),SerializeField,Tooltip("Only for the AI, start at 0")]
        public float Battery { get; private set; }
        
        [Serializable] public struct AttackData
        {
            [Tooltip("Damage dealt (negative value)")]
            public float damage;
    
            [Tooltip("Battery gain (positive value)")]
            public float batteryGain;
        }
        
        [field: Header("ANIMATION "),SerializeField]
        public EntityAnimation everyAnimation { get; private set; }
        
        public virtual AbstractEntityDataInstance Instance(GameObject entity)
        {
            return new AbstractEntityDataInstance(this, entity);
        }
    }

    [Serializable]
    public class AbstractEntityDataInstance
    {
        public float hp;
        public float maxHp { get; private set; }
        public int speed;
        public FightManager.TurnState turnState;
        public GameObject entity;
        public float waveAmplitudeEnemy;
        public float waveFrequency;
        public bool seenByRadio;
        [FormerlySerializedAs("notHidden")] public bool reveal;
        public float battery;
        public AbstractEntityData.AttackData normalAttack;
        public AbstractEntityData.AttackData heavyAttack;
        public AbstractEntityData.AttackData stealBatteries;
        public AbstractEntityData.AttackData stealALotBatteries;
        public bool flashed;

        public EntityAnimation entityAnimation;
 
        
        public AbstractEntityDataInstance(AbstractEntityData data, GameObject entity)
        {
            hp = data.Hp;
            speed = data.Speed;
            turnState = data.TurnState;
            this.entity = entity;
            waveAmplitudeEnemy = data.WaveAmplitudeEnemy;
            waveFrequency = data.WaveFrequencyEnemy;
            seenByRadio = data.SeenByRadio;
            reveal = data.NotHidden;
            battery = data.Battery;
            maxHp = hp;
            flashed = false;

            entityAnimation = data.everyAnimation;
        }

        public bool IsDead()
        {
            return hp <= 0;
        }

        public bool IsBatteryMoreThanHundred()
        {
            return battery > 100;
        }

        public bool IsBatteryEqualOrMoreThanFifty()
        {
            return battery >= 50;
        }
    }
    
    
    [Serializable]
    public class EntityAnimation
    {
        public AnimationClip attackAnimation;
        public AnimationClip takeDamageAnimation;
    }
}