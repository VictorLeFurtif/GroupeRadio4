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
using INTERACT;
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
        private GameObject soundEnemyInFight;
        
        [Header("Combat Timing")]
        public float playerTurnDuration = 60f; 
        public float playerTurnTimer { get; private set; }
        public bool playerSuccess = false;
        
        [Header("Chip Matching")]
        private bool isMatchingPhase = false;
        private NewAi currentEnemyTarget;
        
        [Header("Eye Settings")]
        public Renderer monsterEyes;
        private int currentSequenceIndex = 0;

        private SpotLightFightManager spotLightFightManager;

        
        [HideInInspector] public int numberOfSwap; 
        
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
            spotLightFightManager = GetComponent<SpotLightFightManager>();
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

        public void InitialiseFightManager() 
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
                    SoundManager.instance.soundBankData.musicSound.DarkerThanDark, true, true, 1f, "FightSound");
            }
            else
            {
                soundForFight.SetActive(true);
            }

            if (soundEnemyInFight == null)
            {
                soundEnemyInFight = SoundManager.instance?.InitialisationAudioObjectDestroyAtEnd(
                    SoundManager.instance.soundBankData.enemySound.respirationNmiCombat, true, true, 1f, "EnemyBreath");
            }
            else
            {
                soundEnemyInFight.SetActive(true);
            }

            
            firstAttempt = true;
            NewRadioManager.instance.InitializeCombatLights(currentEnemyTarget.chipsDatasListSave.Count);
            NewRadioManager.instance.StartCoroutine(NewRadioManager.instance.RadioBehaviorDependingFightState());

            if (currentFightAdvantage == FightAdvantage.Disadvantage)
            {
                AttackPlayer(true);
                
                if (!NewPlayerController.instance._inGameData.IsDead())
                {
                    NewPlayerController.instance.animatorPlayer.Play("Overload");
                }
                
            }
            
            
            
            spotLightFightManager.InitLight();
            
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
                soundEnemyInFight?.SetActive(false);
               
            }
            else if (!fighterAlive.Contains(player._abstractEntityDataInstance))
            {
                Debug.Log("IA win");
                ResetFightManagerAfterFight();
                soundForFight?.SetActive(false);
                soundEnemyInFight?.SetActive(false);
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
                    AttackPlayer(); 
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
            currentEnemyTarget.ResetSequenceIndex(currentEnemyTarget.chipsDatasList);
            NewRadioManager.instance?.UpdateOscillationEnemy(currentEnemyTarget);
        }
        
        private void ResetFightManagerAfterFight()
        {
            currentOrder.Clear();
            fighterAlive.Clear();
            fightState = FightState.OutFight;
            numberOfSwap = 0;
            StartCoroutine(NewRadioManager.instance?.HandleRadioTransition(new WaveSettings(0, 0, 0))
            );
            NewRadioManager.instance?.ResetLights();
            if (NewRadioManager.instance != null)
                NewRadioManager.instance.StartCoroutine(NewRadioManager.instance.RadioBehaviorDependingFightState());
            currentSequenceIndex = 0;
            spotLightFightManager.CleanLight();
            GameManager.instance.globalVolumeManager.GvColorToExplo();
            
        }

        
        private void AttackPlayer()
        {
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.bruitCoupNMI);
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.grognementAttaque);
            NewAi ai = currentOrder[0]?.entity.GetComponent<NewAi>();
            if (ai != null)
            { 
                player.ManageLife(-ai.damageEnemy);
               ai.animatorEnemy.Play("attackAi");
               coroutineAnimation = StartCoroutine(EndFighterTurnWithTimeAnimation
                   (ai._abstractEntityDataInstance.entityAnimation.attackAnimation));
               numberOfSwap = ai.numberOfSwap;
            }
            else
            {
                float baseDamageIfError = 10;
                player.ManageLife(-baseDamageIfError);
            }
        }
        
        private void AttackPlayer(bool isInit)
        {
            if (!isInit)return;
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.bruitCoupNMI);
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.grognementAttaque);
            NewAi ai = currentOrder[1]?.entity.GetComponent<NewAi>();
            if (ai != null)
            { 
                player.ManageLife(-ai.damageEnemy);
            }
            else
            {
                float baseDamageIfError = 10;
                player.ManageLife(-baseDamageIfError);
            }
        }
        
        private void AttackPlayer(NewAi ai)
        {
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.bruitCoupNMI);
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.grognementAttaque);
            player.ManageLife(-ai.damageEnemy);
            
            if (ai != null)
            {
                ai.animatorEnemy.Play("attackAi");
            }
        }

        private Coroutine coroutineAnimation;
        private bool firstAttempt;
        //TODO PUTAIN COMMENT JE SUIS CENSER CORRIGER CE BUG DE MERDE
        private void ProcessPlayerGuess()
        {
            if (!isMatchingPhase || currentEnemyTarget == null)
            {
                return;
            }
            
            currentSequenceIndex++;
            
            var currentSequence = currentEnemyTarget.chipsDatasList;
            var playerSelection = ChipsManager.Instance.playerChoiceChipsOrder;

            if (firstAttempt)
            {
                firstAttempt = false;
            }
            
            int correctCount = 0;
            bool allCorrect = true;
            
            allCorrect = !(currentSequence.Count < playerSelection.Count);
            
            for (int i = 0; i < Mathf.Min(currentSequence.Count, playerSelection.Count); i++)
            {
                if (AreChipsEquivalent(currentSequence[i], playerSelection[i]))
                {
                    correctCount++;
                }
                else
                {
                    allCorrect = false;
                    break;
                }
            }
            
            if (correctCount > 0 && allCorrect)
            {
                int totalSequenceLength = currentEnemyTarget.chipsDatasListSave.Count;
                int remainingChips = currentSequence.Count;
                int firstCorrectIndex = totalSequenceLength - remainingChips;
                
                for (int i = 0; i < correctCount; i++)
                {
                    NewRadioManager.instance.UpdateCombatLight(firstCorrectIndex + i, true);
                }
                
                for (int i = 0; i < correctCount; i++)
                {
                    currentEnemyTarget.MoveToNextChip();
                    NewRadioManager.instance?.UpdateOscillationEnemy(currentEnemyTarget);
                }
                
                currentSequence.RemoveRange(0, correctCount);
                
                SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.enemySound.takeDamage);
                
                if (currentSequence.Count == 0)
                {
                    EnemySequenceGuessed();
                    SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.validationFinal);
                }
                else
                {
                    SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.validation);
                }
                
            }
            else
            {
                AttackPlayer(currentEnemyTarget);
                SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.failMatchRevers);
            }
            
        }
        private bool AreChipsEquivalent(ChipsDataInstance chip1, ChipsDataInstance chip2)
        {
            if (chip1 == null || chip2 == null) return false;
    
            return chip1.index == chip2.index 
                   && chip1.colorLinkChips == chip2.colorLinkChips;
        }

        [SerializeField] private float goldenRunLifeGiven = 10f;
        private void EnemySequenceGuessed()
        {
            if (currentSequenceIndex == 1)
            {
                NewPlayerController.instance?.ManageLife(goldenRunLifeGiven);
                Debug.Log("You did a golden run well play man");
            }
            
            
            
            playerSuccess = true;
            
            currentEnemyTarget.PvEnemy -= currentEnemyTarget.PvEnemy;
            
            if (currentEnemyTarget._abstractEntityDataInstance.IsDead())
            {
                listOfJustEnemiesAlive.Remove(currentEnemyTarget._abstractEntityDataInstance);
                fighterAlive.Remove(currentEnemyTarget._abstractEntityDataInstance);
                currentOrder.Remove(currentEnemyTarget._abstractEntityDataInstance);
            }
            
            isMatchingPhase = true; 
            playerTurnTimer = playerTurnDuration;

            
            
        }
        
        
        #endregion

        #region Time Related

        private void HandleTimerPlayerForPlay()
        {
            if (currentFighter != player._abstractEntityDataInstance || !isMatchingPhase || !(playerTurnTimer > 0))
            {
                return;
            }

            playerTurnTimer -= Time.deltaTime;

            if (playerTurnTimer <= 0)
            {
                NewRadioManager.instance.InitializeCombatLights(currentEnemyTarget.chipsDatasListSave.Count);
                currentEnemyTarget.ResetSequenceIndex(currentEnemyTarget.chipsDatasListSave);
                NewRadioManager.instance?.UpdateOscillationEnemy(currentEnemyTarget);
                currentEnemyTarget.chipsDatasList = new List<ChipsDataInstance>(currentEnemyTarget.chipsDatasListSave);
                playerSuccess = false;
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
            ChipsManager.Instance?.ResetAllChipsSelected();
        }

        public void CostForEachChipsAdded()
        {
            int numberOfElementInPlayerChoice = ChipsManager.Instance.playerChoiceChipsOrder.Count;
            
            if (numberOfElementInPlayerChoice is 1 or 0)
            {
                return;
            }

            for (int i = 0; i < numberOfElementInPlayerChoice - 1; i++)
            {
                NewPlayerController.instance?.ManageLife(-ChipsManager.Instance.damageForEachChip);
            }
        }
        
        public void OnReverseButtonPressed()
        {
            if (!isMatchingPhase || currentEnemyTarget == null) return;
            ChipsManager.Instance.ReverseChips();
            ProcessPlayerGuess();
            ChipsManager.Instance?.ResetAllChipsSelected();
        }


        #endregion

        public bool IsInFight()
        {
            return fightState == FightState.InFight;
        }
    }
}