using Controller;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/ Le Gros Bouclier", fileName = "data")]
    public class LeGrosBouclier : PlayerAttackAbstract
    {
        public override PlayerAttackAbstractInstance Instance()
        {
            return new LeGrosBouclierInstance(this);
        }
    }

    public class LeGrosBouclierInstance : PlayerAttackAbstractInstance, IPLayerEffect
    {
        public LeGrosBouclierInstance(LeGrosBouclier data) : base(data){}

        public void ProcessEffect()
        {
            if (FightManager.instance == null || PlayerController.instance == null) return;

            var player = PlayerController.instance;

            switch (FightManager.instance.fightState)
            {
                case FightManager.FightState.InFight when player.selectedEnemy != null:
                    player._inGameData.grosBouclier = true;
                    break;
                case FightManager.FightState.OutFight:
                    player.gameObject.layer = 7;
                    break;
            }
        }

        public void CancelEffectWhenEnterFight()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.gameObject.layer = 6;
            }

        }
    }
}