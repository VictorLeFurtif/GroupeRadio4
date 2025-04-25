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
            VodkaOndeRadio,
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
            public float costBatteryExploration;
            public float costBatteryInFightMax;
            public float costBatteryInFight;
        }
        
        [field:Header("Concern only for Post Zero"),SerializeField] public float DamageBomb { get; private set; }
        [field: Header("Attack Player"),SerializeField] public AttackClassic AttackP { get; private set; }
        
        [field: Header("Multiplicateur de vie prise"),SerializeField] public float MultiplicatorLifeTaken { get; private set; }

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
        public float multiplicatorLifeTaken;
        
        
        public PlayerAttackInstance(PlayerAttack data)
        {
            attack = data.AttackP;
            damageBomb = data.DamageBomb;
            multiplicatorLifeTaken = data.MultiplicatorLifeTaken;
        }

        public void ProcessAttackEffect()
        {
            if (FightManager.instance == null || PlayerController.instance == null)
            {
                Debug.LogError("No FighManager or PlayerController instances were Found");
                return;
            }
            
            PlayerController player = PlayerController.instance;
            
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
                    }
                    else if (FightManager.instance.fightState == FightManager.FightState.OutFight)
                    {
                        player._inGameData.classicEcho = true;
                    }
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

        public void ProcessAttackLogic()
        {
            PlayerController player = PlayerController.instance;
            switch (attack.attackEffect)
            {
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
                    if (FightManager.instance.fightState == FightManager.FightState.InFight && player.selectedEnemy != null)
                    {
                        var selectedEnemyAbstractAi = player.selectedEnemy.GetComponent<AbstractAI>();
                        
                        foreach (var enemies in FightManager.instance.listOfJustEnemiesAlive)
                        {
                            var enemy = enemies.entity.GetComponent<AbstractAI>();
                            float damageOtherEnemy =
                                PlayerController.instance.selectedAttackEffect.attack.damage / 2;
                            if (enemy == selectedEnemyAbstractAi)
                            {
                                continue;
                            }
                            enemy.PvEnemy -= damageOtherEnemy;
                        }
                    }
                    break;
                case PlayerAttack.AttackEffect.VodkaOndeRadio:
                    if (FightManager.instance.fightState == FightManager.FightState.InFight && player.selectedEnemy != null)
                    {
                        var enemySelectedData = player.selectedEnemy.GetComponent<AbstractAI>()._abstractEntityDataInstance;
                        enemySelectedData.vodkaOndeRadio.isVodka = true;
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        public void TakeLifeFromPlayer()
        {
            if (PlayerController.instance != null)
            {
                Debug.Log(-attack.costBatteryInFightMax + "Attack ");
                PlayerController.instance.ManageLife(-attack.costBatteryInFightMax);
            }
        }
        
        public void TakeLifeFromPlayer(float damage)
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.ManageLife(-damage);
                Debug.Log(-damage + "Effect");
            }
        }
        
    }
}
