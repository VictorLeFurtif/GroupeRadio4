using System;
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
        public static FightManager instance;
    
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

        [Header("Check for enemy in range of Player")] [SerializeField]
        private float rangeDetectEnemy;
        
        private AbstractEntityDataInstance currentFighter;

        public FightAdvantage currentFightAdvantage = FightAdvantage.Neutral;

        private GameObject soundForFight;
        private void Awake()
        {
            if (instance == null)instance = this;
            else Destroy(gameObject);

            player = NewPlayerController.instance;
            radio = NewRadioManager.instance;
        }
        
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

        private void UpdateListOfFighter() //call after each endTurn
        {
            CheckForDeadsFighter();
        }

        public void EndFighterTurn()
        {
            if (currentFighter != null)
            {
                currentFighter.turnState = TurnState.NoTurn;
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

            CheckForEndFight();
            StartUnitTurn();
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
            Debug.Log($"Player added to currentOrder: {player._abstractEntityDataInstance.entity.name}");
            
            if (NewRadioManager.instance != null)
            {
                foreach (NewAi fighter in NewRadioManager.instance.listOfEveryEnemy)
                {
                    float distance = Vector3.Distance(fighter.transform.position, player.transform.position);
                    if (distance <= rangeDetectEnemy)
                    {
                        currentOrder.Add(fighter._abstractEntityDataInstance);
                        fighter._aiFightState = AiFightState.InFight;
                        Debug.Log($"Enemy added: {fighter.name} at distance {distance}");
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

            Debug.Log($"Total fighters: {currentOrder.Count} (Player + {listOfJustEnemiesAlive.Count} enemies)");
    
            StartUnitTurn();
            
            
            if (soundForFight == null)
            {
                soundForFight = SoundManager.instance?.InitialisationAudioObjectDestroyAtEnd(SoundManager.instance.soundBankData.enemySound.
                    enemySound,true,true,1f,"FightSound");
            }
            else
            {
                soundForFight.SetActive(true);
            }
            
             
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
                soundForFight.SetActive(false);
            }
            else if (!fighterAlive.Contains(player._abstractEntityDataInstance))
            {
                Debug.Log("IA win");
                ResetFightManagerAfterFight();
                soundForFight.SetActive(false);
            }
            else
            {
                Debug.Log("Nobody Win continue");
            }
        }
    
        private void StartUnitTurn()
        {
            if (currentOrder.Count <= 0) return;
            currentFighter = currentOrder[0]; 
            currentFighter.turnState = TurnState.Turn;

        }
        
        private void ResetFightManagerAfterFight()
        {
            currentOrder.Clear();
            fighterAlive.Clear();
            fightState = FightState.OutFight;
            
        }
    }
}