using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.NEW_AI;
using Controller;
using DATA.Script.Entity_Data.AI;
using ENUM;
using INTERFACE;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MANAGER
{
    public class FightManager : MonoBehaviour
    {
        #region Singleton
        public static FightManager instance;
        #endregion

        #region Fields
        [field: Header("Specific To Project - WIP")]
        private int numberOfEnemies;
        private int numberOfPlayer;
    
        [Header("List Parameters")]
        public List<AbstractEntityDataInstance> currentOrder;
        public List<AbstractEntityDataInstance> fighterAlive;
        public List<AbstractEntityDataInstance> listOfJustEnemiesAlive;
 
        [Header("StateMachine")]
        public FightState fightState = FightState.OutFight;
    
        private int numberOfTurn = 0;

        private NewPlayerController player;
        private NewRadioManager radio;

        [Header("Check for enemy in range of Player")] 
        [SerializeField] private float rangeDetectEnemy;
        
        private AbstractEntityDataInstance currentFighter;

        public FightAdvantage currentFightAdvantage = FightAdvantage.Neutral;

        private GameObject soundForFight;
        
        [Header("Combat Timing")]
        public float playerTurnDuration = 60f; 
        public float playerTurnTimer { get; private set; }
        public bool playerSuccess = false;
        
        
        #endregion

        #region Enums
        public enum FightAdvantage
        {
            Advantage,
            Disadvantage,
            Neutral,
        }
        
        public enum FightState 
        {
            OutFight,
            InFight
        }
        
        public enum TurnState
        {
            Turn,
            NoTurn,
        }
        #endregion

        #region Unity Methods
        private void Awake()
        {
            if (instance == null) instance = this;
            else Destroy(gameObject);

            player = NewPlayerController.instance;
            radio = NewRadioManager.instance;
        }
        
        private void Update()
        {
            if (fightState != FightState.InFight) return;
            HandleTimerPlayerForPlay();
            
        }
        #endregion

        #region Fight Management
        public void EndFighterTurn()
        {
            if (currentFighter != null)
            {
                currentFighter.turnState = TurnState.NoTurn;
            }
            
            if (currentFighter == player._abstractEntityDataInstance)
            {
                NewRadioManager.instance.StopMatchingGame();
                player.canMove = true;
            }
            
            UpdateListOfFighter();

            if (currentOrder.Count > 0 && currentOrder[0] == currentFighter)
            {
                currentOrder.RemoveAt(0);
            }

            if (currentOrder.Count == 0)
            {
                numberOfTurn++;
                currentOrder.AddRange(fighterAlive);
            }

            if (fightState == FightState.InFight )
            {
                CheckForEndFight();
                StartUnitTurn();
            }
            
        }

        public void InitialiseList() 
        {
            if (player == null) 
            {
                Debug.LogError("Player reference is null!");
                return;
            }
            
            currentOrder.Clear();
            listOfJustEnemiesAlive.Clear();
            
            currentOrder.Add(player._abstractEntityDataInstance);
            
            if (NewRadioManager.instance != null)
            {
                foreach (NewAi fighter in NewRadioManager.instance.listOfEveryEnemy)
                {
                    float distance = Vector3.Distance(fighter.transform.position, player.transform.position);
                    if (distance <= rangeDetectEnemy)
                    {
                        currentOrder.Add(fighter._abstractEntityDataInstance);
                        fighter._aiFightState = AiFightState.InFight;
                    }
                }
            }
            else
            {
                Debug.LogError("NewRadioManager instance is null!");
            }
            
            listOfJustEnemiesAlive.AddRange(currentOrder.Where(x => x != player._abstractEntityDataInstance));
            currentOrder.Sort((x, y) => y.speed.CompareTo(x.speed));
            fighterAlive = new List<AbstractEntityDataInstance>(currentOrder);

            player.canMove = false;
            
            StartUnitTurn();
            
            if (soundForFight == null)
            {
                soundForFight = SoundManager.instance?.InitialisationAudioObjectDestroyAtEnd(
                    SoundManager.instance.soundBankData.enemySound.enemySound, true, true, 1f, "FightSound");
            }
            else
            {
                soundForFight.SetActive(true);
            }
        }
        #endregion

        #region Helper Methods
        private void UpdateListOfFighter()
        {
            CheckForDeadsFighter();
        }

        private void CheckForDeadsFighter() 
        {
            fighterAlive.RemoveAll(fighter => fighter.IsDead());
            currentOrder.RemoveAll(fighter => fighter.IsDead());
            listOfJustEnemiesAlive.RemoveAll(fighter => fighter.IsDead());
        }

        private void CheckForEndFight() 
        {
            if (fighterAlive.Count == 1 && fighterAlive[0] == player._abstractEntityDataInstance)
            {
                Debug.Log("Player win");
                player._abstractEntityDataInstance.turnState = TurnState.NoTurn;
                ResetFightManagerAfterFight();
                soundForFight?.SetActive(false);
                player.canMove = true;
                
            }
            else if (!fighterAlive.Contains(player._abstractEntityDataInstance))
            {
                Debug.Log("IA win");
                ResetFightManagerAfterFight();
                soundForFight?.SetActive(false);
            }
            else
            {
                Debug.Log("Nobody Win continue");
            }
        }
    
        private void StartUnitTurn()
        {
            coroutine = null;
            
            if (currentOrder.Count <= 0) return;

            currentFighter = currentOrder[0];
            currentFighter.turnState = TurnState.Turn;
    
            if (currentFighter == player._abstractEntityDataInstance)
            {
                playerTurnTimer = playerTurnDuration;
                playerSuccess = false;
                
                NewAi ai = currentOrder[1]?.entity.GetComponent<NewAi>();
                if (ai != null)
                {
                    //ai.GenerateWavePatterns();
                    NewRadioManager.instance.StartMatchingGameInFight();
                }
            }
            else 
            {
                if (!playerSuccess) 
                {
                    AttackPlayer(currentFighter); 
                }
                else
                {
                    EndFighterTurn();
                }
            }
        }
        
        private void ResetFightManagerAfterFight()
        {
            Debug.LogError("HHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
            player.canMove = true;
            currentOrder.Clear();
            fighterAlive.Clear();
            fightState = FightState.OutFight;
        }
        
        private void AttackPlayer(AbstractEntityDataInstance attacker)
        {
            player.ManageLife(-10);
            Debug.Log("L'IA attaque le joueur !");
            NewAi ai = currentOrder[0]?.entity.GetComponent<NewAi>();
            if (ai != null)
            {
               ai.animatorEnemy.Play("attackAi");
               coroutine = StartCoroutine(EndFighterTurnWithTimeAnimation(ai._abstractEntityDataInstance.entityAnimation.attackAnimation));
            }
        }

        private Coroutine coroutine;
        
        public void PlayerSuccess()
        {
            NewAi ai = currentOrder[1]?.entity.GetComponent<NewAi>();
            if (ai != null)
            {
                float damage = player.GetPlayerDamage() * playerTurnTimer / playerTurnDuration;
                ai.PvEnemy -= damage;
                Debug.Log($"Player attack and did {damage}");
                //ai.GenerateWavePatterns();
            }
    
            playerSuccess = true;
            player.animatorPlayer.Play("goodsize anime attaque spé");
            coroutine = StartCoroutine(EndFighterTurnWithTimeAnimation(player._inGameData.entityAnimation.attackAnimation));
        }
        #endregion

        #region Time Related

        private void HandleTimerPlayerForPlay()
        {
            if (currentFighter != player._abstractEntityDataInstance || !(playerTurnTimer > 0)) return;

            if (coroutine != null)
            {
                playerTurnTimer = playerTurnDuration;
            }
            
            playerTurnTimer -= Time.deltaTime;

            if (!(playerTurnTimer <= 0) || playerSuccess) return;
            NewRadioManager.instance.StopMatchingGame();
            Debug.Log("Temps écoulé !");
           
            EndFighterTurn();
        }

        private IEnumerator EndFighterTurnWithTimeAnimation(AnimationClip _animation)
        {
            yield return new WaitForSeconds(_animation.length);
            EndFighterTurn();
        }
        #endregion

        
    }
}