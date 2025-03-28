using System;
using AI;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Entity_Data.AI
{
    [CreateAssetMenu(menuName = "ScriptableObject/AbstractIAData", fileName = "data")]
    public abstract class AbstractEntityData : ScriptableObject
    {
        [field: Header("Life"), SerializeField]
        public int Hp { get; private set; }
    
    
        [field: Header("Hidden Speed"),SerializeField]
        public int Speed { get; private set; }

        [field: Header("Turn State"), SerializeField]
        public FightManager.TurnState TurnState;
    
        [field: Header("Wave amplitude"),SerializeField,Tooltip("Max O.4f please")]
        public float WaveAmplitudeEnemy { get; private set; }
        
        [field: Header("Wave frequency"),SerializeField, Tooltip("Max 15f please")]
        public float WaveFrequencyEnemy { get; private set; }

        public virtual AbstractEntityDataInstance Instance(GameObject entity)
        {
            return new AbstractEntityDataInstance(this, entity);
        }
    }

    [Serializable]
    public class AbstractEntityDataInstance
    {
        public int hp;
        public int speed;
        public FightManager.TurnState turnState;
        public GameObject entity;
        public float waveAmplitudeEnemy;
        public float waveFrequency;

        public AbstractEntityDataInstance(AbstractEntityData data, GameObject entity)
        {
            hp = data.Hp;
            speed = data.Speed;
            turnState = data.TurnState;
            this.entity = entity;
            waveAmplitudeEnemy = data.WaveAmplitudeEnemy;
            waveFrequency = data.WaveFrequencyEnemy;
        }

        public bool IsDead()
        {
            return hp <= 0;
        }
    }
}