using AI;
using Controller;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/CherieBomb", fileName = "CherieBombAttack")]
    public class CherieBomb : PlayerAttackAbstract
    {
        public override PlayerAttackAbstractInstance Instance()
        {
            return new CherieBombInstance(this);
        }
    }

    public class CherieBombInstance : PlayerAttackAbstractInstance, IPlayerAttack
    {
        public CherieBombInstance(CherieBomb data) : base(data) {}

        public void ProcessAttack()
        {
            if (FightManager.instance == null || PlayerController.instance == null || PlayerController.instance.selectedEnemy == null) return;

            var selectedEnemyAbstractAi = PlayerController.instance.selectedEnemy.GetComponent<AbstractAI>();

            foreach (var enemy in FightManager.instance.listOfJustEnemiesAlive)
            {
                var ai = enemy.entity.GetComponent<AbstractAI>();
                if (ai == selectedEnemyAbstractAi) continue;

                float damageOtherEnemy = attack.damage / 2f;
                ai.PvEnemy -= damageOtherEnemy;
            }
        }
    }
}