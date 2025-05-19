using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.OLD_AI_BEFORE_FP_V2;
using Controller;
using DATA.Script.Entity_Data.AI;
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

        private PlayerController player;
        private RadioController radio;

        [Header("Check for enemy in range of Player")] [SerializeField]
        private float rangeDetectEnemy;
        
        private AbstractEntityDataInstance currentFighter;

        public FightAdvantage currentFightAdvantage = FightAdvantage.Neutral;

        private GameObject soundForFight;
        private void Awake()
        {
            if (instance == null)instance = this;
            else Destroy(gameObject);

            player = PlayerController.instance;
            radio = RadioController.instance;
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


        public void InitialiseList() // detect every enemy in a rayon
        {
            
            if (player == null)
            {
                return;
            }
            
            listOfJustEnemiesAlive.AddRange(currentOrder);
            
            // Sort en fonction de la distance avec le player pour que les ampoules soit plus logique
            
            listOfJustEnemiesAlive.Sort((x, y) =>
                Vector3.Distance(PlayerController.instance.transform.position, x.entity.transform.position)
                    .CompareTo(
                        Vector3.Distance(PlayerController.instance.transform.position, y.entity.transform.position)));

            
            currentOrder.Add(player._abstractEntityDataInstance);
        
            currentOrder.Sort((x, y) => y.speed.CompareTo(x.speed));

            fighterAlive = new List<AbstractEntityDataInstance>(currentOrder); 
            
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
            
            BehaviorPlayerEnteringFight();
        }

        private void CheckForDeadsFighter() //call end turn can use foreach + IsDead() bool
        {
            fighterAlive.RemoveAll(fighter => fighter.IsDead());
            currentOrder.RemoveAll(fighter => fighter.IsDead());
            listOfJustEnemiesAlive.RemoveAll(fighter => fighter.IsDead());
        }

        private void CheckForEndFight() //Check if player Dead or if every enemy Dead in ListAlive
        {
            if (fighterAlive.Count == 1 && fighterAlive[0] == player._abstractEntityDataInstance)
            {
                Debug.Log("Player win");
                PlayerController.instance._abstractEntityDataInstance.turnState = TurnState.NoTurn;
                ResetFightManagerAfterFight();
                RadioController.instance.UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
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
                
                //TODO ON A ENLEVER L'ANCIENNE LOGIQUE BON DIEU
            }
        }

        private void BehaviorPlayerEnteringFight()
        {
            
        }
        
        private void StartUnitTurn()
        {
            if (currentOrder.Count > 0)
            {
                currentFighter = currentOrder[0]; 
                currentFighter.turnState = TurnState.Turn; 
            }
    
            UpdateUI();
        }

        private void UpdateUI()
        {
        
        }

        private void ResetFightManagerAfterFight()
        {
            currentOrder.Clear();
            fighterAlive.Clear();
            fightState = FightState.OutFight;
            
            /*temporary
            int index = SceneManager.GetActiveScene().buildIndex;
            SceneManager.LoadScene(index);*/
        }
    }
}