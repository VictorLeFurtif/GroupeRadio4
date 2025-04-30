using AI;
using Controller;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/ Skylight", fileName = "SkyLightAttack")]
    public class SkyLight : PlayerAttackAbstract
    {
        public override PlayerAttackAbstractInstance Instance()
        {
            return new SkyLightInstance(this);
        }
    }

    public class SkyLightInstance : PlayerAttackAbstractInstance, IPLayerEffect
    {
        public SkyLightInstance(SkyLight data) : base(data) {}

        public void ProcessEffect()
        {
            if (FightManager.instance == null || PlayerController.instance == null) return;

            var player = PlayerController.instance;
            switch (FightManager.instance.fightState)
            {
                case FightManager.FightState.InFight when player.selectedEnemy != null:
                    player.selectedEnemy.GetComponent<AbstractAI>()._abstractEntityDataInstance.flashed = true;
                    break;
                case FightManager.FightState.OutFight:
                    player.lampTorch.intensity = player.lampTorchOnValue;
                    break;
            }
        }

        public void CancelEffectWhenEnterFight()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.lampTorch.intensity = 0;
            }
        }
        
    }
}