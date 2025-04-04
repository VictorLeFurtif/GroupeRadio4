using System;
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
    

        public virtual AbstractEntityDataInstance Instance()
        {
            return new AbstractEntityDataInstance(this);
        }
    }

    [Serializable]
    public class AbstractEntityDataInstance
    {
        public int hp;
        public int speed;

        public AbstractEntityDataInstance(AbstractEntityData data)
        {
            hp = data.Hp;
            speed = data.Speed;
        }

        public bool IsDead()
        {
            return hp <= 0;
        }
    }
}