using System;
using System.Collections.Generic;
using System.Linq;
using AI;
using Controller;
using DATA.Script.Entity_Data.AI;
using DATA.ScriptData.Entity_Data;
using TMPro;
using UnityEngine;

namespace MANAGER
{
    public class FightManager : MonoBehaviour
    {
        public static FightManager instance;
    
        [field: Header("Specific To Project - WIP")]
        private int numberOfEnemies;
        private int numberOfPlayer;
    
        [Header("List Parameters")]
        [SerializeField] private List<AbstractEntityDataInstance> currentOrder;
        [SerializeField] private List<AbstractEntityDataInstance> fighterAlive;
 
        [Header("StateMachine")]
        public FightState fightState = FightState.OutFight;
    
        private int numberOfTurn = 0;

        private PlayerController player;
        private RadioController radio;

        [Header("Check for enemy in range of Player")] [SerializeField]
        private float rangeDetectEnemy;
        
        private AbstractEntityDataInstance currentFighter;

        [SerializeField] private FightAdvantage currentFightAdvantage = FightAdvantage.Neutral;

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

        public void EndFighterTurn() //take out fighter from list
        {
            if (currentFighter != null)
            {
                currentFighter.turnState = TurnState.NoTurn; 
            }
            
            UpdateListOfFighter();
    
            currentOrder.RemoveAt(0);

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
            
            bool hasAdvantage = false;
            bool hasDisadvantage = false;

            foreach (AbstractAI fighter in radio.listOfEveryEnemy)
            {
                if (!(Mathf.Abs(fighter.transform.position.x - player.transform.position.x) < rangeDetectEnemy)) continue;

                currentOrder.Add(fighter._abstractEntityDataInstance);
                fighter._aiFightState = AbstractAI.AiFightState.InFight;

                if (!fighter._abstractEntityDataInstance.reveal && !fighter._abstractEntityDataInstance.seenByRadio)
                {
                    hasDisadvantage = true;
                }
                else if (fighter._abstractEntityDataInstance.seenByRadio && fighter._abstractEntityDataInstance.reveal)
                {
                    hasAdvantage = true;
                }
            }

            if (hasDisadvantage)
            {
                currentFightAdvantage = FightAdvantage.Disadvantage;
            }
            else if (hasAdvantage)
            {
                currentFightAdvantage = FightAdvantage.Advantage;
            }
            else
            {
                currentFightAdvantage = FightAdvantage.Neutral;
            }
            
            Debug.Log(currentFightAdvantage);
            
            currentOrder.Add(player._abstractEntityDataInstance);
        
            currentOrder.Sort((x, y) => y.speed.CompareTo(x.speed));

            fighterAlive = new List<AbstractEntityDataInstance>(currentOrder); 
            
            StartUnitTurn();
        }

        private void CheckForDeadsFighter() //call end turn can use foreach + IsDead() bool
        {
            fighterAlive.RemoveAll(fighter => fighter.IsDead());
            currentOrder.RemoveAll(fighter => fighter.IsDead());
        }

        private void CheckForEndFight() //Check if player Dead or if every enemy Dead in ListAlive
        {
            if (fighterAlive.Count == 1 && fighterAlive[0] == player._abstractEntityDataInstance)
            {
                Debug.Log("Player win");
                PlayerController.instance._abstractEntityDataInstance.turnState = TurnState.NoTurn;
                fightState = FightState.OutFight;
            }
            else if (!fighterAlive.Contains(player._abstractEntityDataInstance))
            {
                Debug.Log("IA win"); //fin de partie
            }
            else
            {
                Debug.Log("Nobody won");
            }
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
    }
}