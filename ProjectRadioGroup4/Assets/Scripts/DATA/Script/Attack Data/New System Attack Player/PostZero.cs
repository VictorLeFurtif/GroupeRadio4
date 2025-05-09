using AI;
using Controller;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/PostZero", fileName = "PostZeroAttack")]
    public class PostZero : PlayerAttackAbstract
    {
        [field:Header("Post Zero Damage of the Bomb"),SerializeField] public float DamageBomb { get; private set; }
        
        public override PlayerAttackAbstractInstance Instance()
        {
            return new PostZeroInstance(this);
        }
    }

    public class PostZeroInstance : PlayerAttackAbstractInstance, IPlayerAttack
    {
        private readonly float damageBomb;
        
        public PostZeroInstance(PostZero data) : base(data)
        {
            damageBomb = data.DamageBomb;
        }

        public void ProcessAttack()
        {
            if (FightManager.instance == null || PlayerController.instance == null || PlayerController.instance.selectedEnemy == null) return;

            var enemySelectedData = PlayerController.instance.selectedEnemy.GetComponent<AbstractAI>()._abstractEntityDataInstance;
            enemySelectedData.postZeroDeal.postZeroBomb = true;
            enemySelectedData.postZeroDeal.damageStockForAfterDeath = damageBomb;
        }
    }
}