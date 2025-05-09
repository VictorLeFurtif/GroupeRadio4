using AI;
using Controller;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/VodkaOndeRadio", fileName = "VodkaOndeRadioAttack")]
    public class VodkaOndeRadio : PlayerAttackAbstract
    {
        public override PlayerAttackAbstractInstance Instance()
        {
            return new VodkaOndeRadioInstance(this);
        }
    }

    public class VodkaOndeRadioInstance : PlayerAttackAbstractInstance, IPlayerAttack
    {
        public VodkaOndeRadioInstance(VodkaOndeRadio data) : base(data) {}

        public void ProcessAttack()
        {
            if (FightManager.instance == null || PlayerController.instance == null || PlayerController.instance.selectedEnemy == null) return;

            var enemySelectedData = PlayerController.instance.selectedEnemy.GetComponent<AbstractAI>()._abstractEntityDataInstance;
            enemySelectedData.vodkaOndeRadio.isVodka = true;
            enemySelectedData.vodkaOndeRadio.compteurVodka = 0;
        }
    }
}