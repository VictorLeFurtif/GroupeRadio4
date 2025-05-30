using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.NEW_AI;
using Controller;
using DATA.Script.Chips_data;
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
        
        [Header("Chip Matching")]
        private bool isMatchingPhase = false;
        private NewAi currentEnemyTarget;
        
        
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
            coroutineAnimation = null;
    
            if (currentOrder.Count <= 0) return;

            currentFighter = currentOrder[0];
            currentFighter.turnState = TurnState.Turn;

            if (currentFighter == player._abstractEntityDataInstance)
            {
                StartPlayerTurn();
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
        
        private void StartPlayerTurn()
        {
            playerTurnTimer = playerTurnDuration;
            playerSuccess = false;
            isMatchingPhase = true;
            
            currentEnemyTarget = currentOrder[1].entity.GetComponent<NewAi>();
        }
        
        private void ResetFightManagerAfterFight()
        {
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
               coroutineAnimation = StartCoroutine(EndFighterTurnWithTimeAnimation
                   (ai._abstractEntityDataInstance.entityAnimation.attackAnimation));
            }
        }

        private Coroutine coroutineAnimation;
        
        private void ProcessPlayerGuess()
        {
            if (!isMatchingPhase || currentEnemyTarget == null)
            {
                Debug.Log("ProcessPlayerGuess appelé hors phase de matching ou sans ennemi cible");
                return;
            }

            var enemySequence = currentEnemyTarget.chipsDatasList;
            var playerSelection = ChipsManager.Instance.playerChoiceChipsOrder;

            Debug.Log($"Début vérification - Séquence ennemie: {string.Join(",", enemySequence.Select(c => c.index))}");
            Debug.Log($"Sélection joueur: {string.Join(",", playerSelection.Select(c => c.index))}");

            int correctCount = 0;
            bool allCorrect = true;

            // Vérification de la correspondance
            for (int i = 0; i < Mathf.Min(enemySequence.Count, playerSelection.Count); i++)
            {
                if (AreChipsEquivalent(enemySequence[i], playerSelection[i]))
                {
                    correctCount++;
                    Debug.Log($"Chip {i} correct: {enemySequence[i].index}");
                }
                else
                {
                    allCorrect = false;
                    Debug.Log($"Chip {i} incorrect: Attendu {enemySequence[i].index}, Reçu {playerSelection[i]?.index}");
                    break;
                }
            }

            if (correctCount > 0)
            {
                for (int i = correctCount - 1; i >= 0; i--)
                {
                    Debug.Log($"Suppression du chip {i} (Index: {enemySequence[i].index})");
                    currentEnemyTarget.chipsDatasList.RemoveAt(i);
                }

                if (allCorrect && playerSelection.Count >= enemySequence.Count)
                {
                    Debug.Log("Séquence COMPLÈTEMENT devinée!");
                    EnemySequenceGuessed();
                }
                else
                {
                    Debug.Log($"Séquence PARTIELLEMENT devinée ({correctCount} chips corrects)");
                }
            }
            else
            {
                Debug.Log("AUCUN chip correct - restauration de la séquence originale");
                currentEnemyTarget.chipsDatasList = new List<ChipsDataInstance>(currentEnemyTarget.chipsDatasListSave);
                playerSuccess = false;
                EndFighterTurn();
            }
        }
        private bool AreChipsEquivalent(ChipsDataInstance chip1, ChipsDataInstance chip2)
        {
            if (chip1 == null || chip2 == null) return false;
    
            return chip1.index == chip2.index 
                   && chip1.colorLinkChips == chip2.colorLinkChips;
        }
        private void EnemySequenceGuessed()
        {
            Debug.Log("EnemySequenceGuessed appelé");
            playerSuccess = true;
            
            float damage = player.GetPlayerDamage();
            currentEnemyTarget.PvEnemy -= damage;
            Debug.Log($"Dégâts appliqués: {damage} - PV restants: {currentEnemyTarget.PvEnemy}");
            
            if (currentEnemyTarget._abstractEntityDataInstance.IsDead())
            {
                Debug.Log("Ennemi vaincu!");
                listOfJustEnemiesAlive.Remove(currentEnemyTarget._abstractEntityDataInstance);
                fighterAlive.Remove(currentEnemyTarget._abstractEntityDataInstance);
                currentOrder.Remove(currentEnemyTarget._abstractEntityDataInstance);
            }
            
            isMatchingPhase = true; 
            playerTurnTimer = playerTurnDuration; 
            
            Debug.Log("Le joueur garde son tour pour continuer");
        }
        
        
        #endregion

        #region Time Related

        private void HandleTimerPlayerForPlay()
        {
            if (currentFighter != player._abstractEntityDataInstance || !isMatchingPhase || !(playerTurnTimer > 0))
            {
                Debug.Log($"Conditions non remplies: isPlayerTurn={currentFighter == player._abstractEntityDataInstance}, isMatching={isMatchingPhase}, timer={playerTurnTimer}");
                return;
            }

            playerTurnTimer -= Time.deltaTime;

            if (playerTurnTimer <= 0)
            {
                Debug.Log("Temps écoulé - fin du tour");
                isMatchingPhase = false;
                EndFighterTurn();
            }
        }

        private IEnumerator EndFighterTurnWithTimeAnimation(AnimationClip _animation)
        {
            yield return new WaitForSeconds(_animation.length);
            EndFighterTurn();
        }
        #endregion

        #region Link to Chips Manager Button Logic

        public void OnMatchButtonPressed()
        {
            if (!isMatchingPhase || currentEnemyTarget == null) return;
    
            ChipsManager.Instance.MatchChips();
            ProcessPlayerGuess();
        }

        public void OnReverseButtonPressed()
        {
            if (!isMatchingPhase || currentEnemyTarget == null) return;
    
            ChipsManager.Instance.ReverseChips();
            ProcessPlayerGuess();
        }


        #endregion
        
    }
}