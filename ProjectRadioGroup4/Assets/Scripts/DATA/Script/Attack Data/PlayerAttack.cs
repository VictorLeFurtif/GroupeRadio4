using System;
using AI;
using Controller;
using MANAGER;
using UnityEngine;
using UnityEngine.Serialization;

namespace DATA.Script.Attack_Data
{
    
    [CreateAssetMenu(menuName = "ScriptableObject/PlayerAttack", fileName = "playerAttack")]
    public class PlayerAttack : ScriptableObject
    {
        public enum AttackState
        {
            Am,
            Fm,
        }
        
        public enum AttackEffect
        {
            Skylight,
            LeGrosBouclier,
            ClassiqueEcho,
            SilenceRadio,
            PostZero,
            DeadHandSignal,
            CherieBomb,
            Enigma,
        }
        
        [Serializable] public struct AttackClassic
        {
            public float damage;
            public float chanceOfOverload;
            public float damageMaxBonus;
            public string name;
            public AttackEffect attackEffect;
            public float indexFrequency;
            public AttackState attackState;
            public float costBattery;
        }
        
        [field:Header("Concern only for Post Zero"),SerializeField] public float DamageBomb { get; private set; }
        
        [field: Header("Attack Player"),SerializeField] public AttackClassic AttackP { get; private set; }

        public PlayerAttackInstance Instance()
        {
            return new PlayerAttackInstance(this);
        }
    }
    
    [Serializable]
    public class PlayerAttackInstance
    {
        public PlayerAttack.AttackClassic attack;
        public float damageBomb;
        
        
        public PlayerAttackInstance(PlayerAttack data)
        {
            attack = data.AttackP;
            damageBomb = data.DamageBomb;
        }

        public void ProcessAttackEffect()
        {
            if (FightManager.instance == null || PlayerController.instance == null)
            {
                Debug.LogError("No FighManager or PlayerController instances were Found");
                return;
            }
            PlayerController player = PlayerController.instance;
            
            //SECOND CONDITION IS ALWAYS OUTFIGHT LOGIC
            switch (attack.attackEffect)
            {
                case PlayerAttack.AttackEffect.Skylight:
                    if (FightManager.instance.fightState == FightManager.FightState.InFight && player.selectedEnemy != null)
                    {
                        player.selectedEnemy.GetComponent<AbstractAI>()._abstractEntityDataInstance
                            .flashed = true;
                    }
                    else if (FightManager.instance.fightState == FightManager.FightState.OutFight)
                    {
                        player.lampTorch.intensity = player.lampTorchOnValue;
                    }

                    break;
                case PlayerAttack.AttackEffect.LeGrosBouclier:
                    if (FightManager.instance.fightState == FightManager.FightState.InFight && player.selectedEnemy != null)
                    {
                        player._inGameData.grosBouclier = true;
                    }
                    else if (FightManager.instance.fightState == FightManager.FightState.OutFight)
                    {
                        player.gameObject.layer = 7;
                    }
                    break;
                case PlayerAttack.AttackEffect.ClassiqueEcho:
                    if (FightManager.instance.fightState == FightManager.FightState.InFight && player.selectedEnemy != null)
                    {
                        player._inGameData.classicEcho = true;
                        attack.costBattery *= 1.75f;
                    }
                    else if (FightManager.instance.fightState == FightManager.FightState.OutFight)
                    {
                        player._inGameData.classicEcho = true;
                    }
                    break;
                case PlayerAttack.AttackEffect.SilenceRadio:
                    break;
                case PlayerAttack.AttackEffect.PostZero:
                    if (FightManager.instance.fightState == FightManager.FightState.InFight && player.selectedEnemy != null)
                    {
                        var enemySelectedData = player.selectedEnemy.GetComponent<AbstractAI>()._abstractEntityDataInstance;
                        enemySelectedData.postZeroDeal.postZeroBomb = true;
                        enemySelectedData.postZeroDeal.damageStockForAfterDeath = damageBomb;
                    }
                    break;
                case PlayerAttack.AttackEffect.DeadHandSignal:
                    break;
                case PlayerAttack.AttackEffect.CherieBomb:
                    break;
                case PlayerAttack.AttackEffect.Enigma:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void CancelEffectWhenEnterFight() // always FM for exploration impact
        {
            if (PlayerController.instance == null || PlayerController.instance.selectedAttack == null)
            {
                return;
            }
            
            PlayerController player = PlayerController.instance;
            
            switch (attack.attackEffect)
            {
                case PlayerAttack.AttackEffect.Skylight:
                    player.lampTorch.intensity = 0;
                    break;
                case PlayerAttack.AttackEffect.LeGrosBouclier:
                    player.gameObject.layer = 6;
                    break;
                case PlayerAttack.AttackEffect.ClassiqueEcho:
                    player._inGameData.classicEcho = false;
                    break;
                case PlayerAttack.AttackEffect.SilenceRadio:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        
        
    }
}
