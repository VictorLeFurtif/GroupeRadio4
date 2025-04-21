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
        
        [field: Header("Normal Attack"),SerializeField] public AttackData NormalAttack { get; private set; }
        [field: Header("Heavy Attack"),SerializeField] public AttackData HeavyAttack { get; private set; }
        [field: Header("Steal Batteries"),SerializeField] public AttackData StealBatteries { get; private set; }
        [field: Header("Steal a lot of Batteries"),SerializeField] public AttackData StealALotBatteries { get; private set; }
        
        
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
            normalAttack = data.NormalAttack;
            heavyAttack = data.HeavyAttack;
            stealBatteries = data.StealBatteries;
            stealALotBatteries = data.StealALotBatteries;
            maxHp = hp;
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
}