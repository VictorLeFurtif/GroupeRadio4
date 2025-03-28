using System;
using UnityEngine;

namespace DATA.Script.Attack_Data
{
    
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack", fileName = "playerAttack")]
    public class PlayerAttack : ScriptableObject
    {
        [field: Header("Damage"), SerializeField]
        public float Damage { get; private set; }
    
        [field: Header("chance of overload"),SerializeField]
        public float ChanceOfOverload { get; private set; }

        [field: Header("Name"), SerializeField]
        public string Name{ get; private set; }
        
        [field: Header("Frequency"), SerializeField]
        public float Frequency{ get; private set; }

        public PlayerAttackInstance Instance()
        {
            return new PlayerAttackInstance(this);
        }
    }
    
    [Serializable]
    public class PlayerAttackInstance
    {
        public float chanceOfOverload;
        public float damage;
        public string name;
        public float frequency;
        
        public PlayerAttackInstance(PlayerAttack data)
        {
            chanceOfOverload = data.ChanceOfOverload;
            damage = data.Damage;
            name = data.Name;
            frequency = data.Frequency;
        }
    }
}
