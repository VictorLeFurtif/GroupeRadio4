using Controller;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace DATA.Script.Attack_Data.New_System_Attack_Player
{
    [CreateAssetMenu(menuName = "Attack Player/ClassiqueEcho", fileName = "ClassiqueEchoAttack")]
    public class ClassiqueEcho : PlayerAttackAbstract
    {
        public override PlayerAttackAbstractInstance Instance()
        {
            return new ClassiqueEchoInstance(this);
        }
    }

    public class ClassiqueEchoInstance : PlayerAttackAbstractInstance, IPLayerEffect
    {
        public ClassiqueEchoInstance(ClassiqueEcho data) : base(data) {}

        public void ProcessEffect()
        {
            if (FightManager.instance == null || PlayerController.instance == null) return;
            PlayerController.instance._inGameData.classicEcho = true;
        }

        public void CancelEffectWhenEnterFight()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance._inGameData.classicEcho = false;
            }
        }
    }
}