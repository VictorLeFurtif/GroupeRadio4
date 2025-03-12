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
    /*
        [field: Header("Links")]
        [SerializeField]
        private TextMeshProUGUI unitTurnText;
        [SerializeField]
        private TextMeshProUGUI numberOfUnitText;
    */
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

        private void Awake()
        {
            if (instance == null)instance = this;
            else Destroy(gameObject);

            player = PlayerController.instance;
            radio = RadioController.instance;
        }

        public enum FightState // temporaire
        {
            OutFight,
            InFight
        }

        private void UpdateListOfFighter() //call after each endTurn
        {
            CheckForDeadsFighter();
        }

        private void EndFighterTurn() //take out fighter from list
        {
            UpdateListOfFighter();
            
            if (currentOrder.Count != 0)
            {
                currentOrder.RemoveAt(0);
            }
            else
            {
                numberOfTurn++;
                currentOrder.AddRange(fighterAlive);
            }

            StartUnitTurn();
        }

        public void InitialiseList() // detect every enemy in a rayon
        {
            Debug.Log("Initialise");
            if (player == null)
            {
                return;
            }

            foreach (AbstractAI fighter in radio.listOfEveryEnemy)
            {
                if (Math.Abs(fighter.gameObject.transform.position.x - player.transform.position.x) < rangeDetectEnemy)
                {
                    currentOrder.Add(fighter._abstractEntityDataInstance);  
                }
            }
            currentOrder.Add(player._abstractEntityDataInstance);
        
            currentOrder.Sort((x, y) => x.speed.CompareTo(y.speed));
            fighterAlive = new List<AbstractEntityDataInstance>(currentOrder); 
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
            }
            else if (!fighterAlive.Contains(player._abstractEntityDataInstance))
            {
                Debug.Log("IA win");
            }
            else
            {
                Debug.Log("Nobody won");
            }

        }
    
        private void StartUnitTurn()
        {
            UpdateUI();
        }

        private void UpdateUI()
        {
        
        }
    }
}