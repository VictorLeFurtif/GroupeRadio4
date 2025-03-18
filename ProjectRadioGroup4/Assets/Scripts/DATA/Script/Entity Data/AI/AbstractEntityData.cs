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

        public AbstractEntityDataInstance(AbstractEntityData data, GameObject entity)
        {
            hp = data.Hp;
            speed = data.Speed;
            turnState = data.TurnState;
            this.entity = entity;
        }

        public bool IsDead()
        {
            return hp <= 0;
        }
    }
}