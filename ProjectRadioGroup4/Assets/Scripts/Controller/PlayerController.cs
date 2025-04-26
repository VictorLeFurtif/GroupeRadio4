using System;
using System.Collections;
using System.Collections.Generic;
using AI;
using DATA.Script.Attack_Data;
using DATA.Script.Entity_Data.AI;
using DATA.Script.Entity_Data.Player;
using MANAGER;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;
using Slider = UnityEngine.UI.Slider;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
    
        [Header("Speed")] [SerializeField] private float moveSpeed;
        [SerializeField] private float moveSpeedRunning;
        public bool canMove = true;

        [Header("Rigidbody2D")] [SerializeField]
        public Rigidbody2D rb;

        [Header("Data Player")]
        [SerializeField]
        private AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        public PlayerDataInstance _inGameData;

        public SpriteRenderer spriteRendererPlayer;

        [Header("UI")]
       // [SerializeField] private GameObject playerFightCanva;

        [Header("Selected Fighter")] public GameObject selectedEnemy;

        [Header("Attacks Player")] [SerializeField] private List<PlayerAttack> listOfPlayerAttack;
        
        public List<PlayerAttackInstance> listOfPlayerAttackInstance = new List<PlayerAttackInstance>();

        public PlayerAttackInstance selectedAttack;
        public PlayerAttackInstance selectedAttackEffect;

        [FormerlySerializedAs("currentPlayerCoreGameState")] [Header("State Machine")]
        public PlayerStateExploration currentPlayerExplorationState = PlayerStateExploration.Exploration;
        
        [Header("Animation")]

        public Animator animatorPlayer;

        [Header("Lighting")] public Light2D lampTorch;
        public float lampTorchOnValue;
        public bool isLampTorchOn;

        [Header("Battery")] private BatteryPlayer playerBattery;
        
        public enum PlayerStateExploration
        {
            Exploration,
            Guessing,
        }
    
        private void Awake()
        {
            rb = GetComponent<Rigidbody2D>();

            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            _abstractEntityDataInstance = _abstractEntityData.Instance(gameObject); 
            _inGameData = (PlayerDataInstance)_abstractEntityDataInstance;
        
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; // pour fix le bug lié à la caméra qui faisait trembler le perso
            spriteRendererPlayer = GetComponent<SpriteRenderer>();
            animatorPlayer = GetComponent<Animator>();
            playerBattery = GetComponent<BatteryPlayer>();
        }

        public void ManageLife(float valueLifeChanger) 
        {
            HealthPlayer += valueLifeChanger;
        }

        private float HealthPlayer 
        {
            get => _inGameData != null ? _inGameData.hp : 0f;
            set
            {
                if (_inGameData == null)
                {
                    Debug.LogError(" _inGameData est NULL !");
                    return;
                }
                
                _inGameData.hp = Mathf.Max(0, value);
                
                if (_inGameData.IsDead())
                {
                    canMove = false;
                    rb.velocity = Vector2.zero;
                    //animatorPlayer.Play("Death");
                    GameManager.instance.GameOver();
                    return;
                }
                playerBattery.UpdateLifeText();

                if (FightManager.instance != null && FightManager.instance.fightState == FightManager.FightState.InFight && _inGameData.turnState != FightManager.TurnState.Turn)
                {
                    animatorPlayer.Play("HitReceived");
                }
            }
        }

    
        /// <summary>
        /// Temporary shity Update
        /// </summary>
        
        private void Update()
        {
            PlayerMove();
            CheckForFlipX();
            //ManageFight();

            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log(rb.velocity);
            }
        }

        private void Start()
        {
            InitializeListOfAttackPlayer();
            lampTorch.intensity = 0;
        }
        
        private void PlayerMove()
        {
            if (!canMove || FightManager.instance.fightState != FightManager.FightState.OutFight)
            {
                rb.velocity = new Vector2(0,0);
                animatorPlayer.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
                return;
            }
            var x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(x  * moveSpeed,rb.velocity.y);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                rb.velocity = new Vector2(x  * moveSpeedRunning,rb.velocity.y);
            }
            animatorPlayer.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
        }
        
        private bool wasFacingLeft = false; //bool pour stock là où il regardait

        private void CheckForFlipX()
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                wasFacingLeft = rb.velocity.x < 0;
            }
            spriteRendererPlayer.flipX = wasFacingLeft;
            lampTorch.transform.localRotation = Quaternion.Euler(wasFacingLeft ? new Vector3(0,180,-90) : new Vector3(0,0,-90));
        }
        

        private void InitializeListOfAttackPlayer()
        {
            foreach (PlayerAttack attack in listOfPlayerAttack)
            {
                listOfPlayerAttackInstance.Add(attack.Instance());
            }
        }
        
        public void ChangeBoolPlayerCanMove(float active)
        {
            canMove = active > 0.5f;
        }
        
        [SerializeField]
        private float epsilonValidationOscillation;
        
        public void ValidButton()
        {
            Debug.Log("On Clcik");
            if (_inGameData.grosBouclier)
            {
                _inGameData.grosBouclier = false;
            }
            
            if (currentPlayerExplorationState == PlayerStateExploration.Guessing &&
                FightManager.instance.fightState == FightManager.FightState.OutFight) //HORS FIGHT DC EXPLO
            {
                if (!(Mathf.Abs(RadioController.instance.sliderOscillationPlayer.value - 
                    RadioController.instance.matRadioEnemy.GetFloat("_waves_Amp")) 
                 < epsilonValidationOscillation))
                {
                    return;
                }
                RadioController.instance.listOfDetectedEnemy[AmpouleManager.ampouleAllumee]
                    ._abstractEntityDataInstance.reveal = true;
                RadioController.instance.listOfDetectedEnemy[AmpouleManager.ampouleAllumee]
                                    .animatorEnemyAi.Play("SpawnAi");
                RadioController.instance.UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
            }

            if (FightManager.instance.fightState == FightManager.FightState.InFight &&
                _abstractEntityDataInstance.turnState == FightManager.TurnState.Turn &&
                selectedAttack != null && selectedEnemy != null)
            {
                PlayerAttack.AttackClassic attackData = selectedAttack.attack;
                float sliderMax = RadioController.instance.sliderOscillationPlayer.maxValue;
                float ratio = sliderMax > 0 ? RadioController.instance.sliderOscillationPlayer.value / sliderMax : 0f;

                
                //TODO MEC FINAL DAMAGE AUCUN SENS AVEC LOGIQUE
                float finalDamage = attackData.damageMaxBonus * ratio + attackData.damage;
                float currentOverloadChance = attackData.chanceOfOverload * ratio;
                
                bool isOverload = Random.Range(0f, 100f) <= currentOverloadChance;
                
                if (isOverload)
                {
                    Debug.Log("OVERLOAD déclenché ");
                    ManageLife(-finalDamage / 2);
                    animatorPlayer.Play("Overload");
                    return;
                }
                
                selectedEnemy.GetComponent<AbstractAI>().PvEnemy -= finalDamage;
                
                selectedAttack.ProcessAttackLogic();
                selectedAttack.TakeLifeFromPlayer();

                if (selectedAttackEffect != null)
                {
                    float lifeTaken = selectedAttack.attack.costBatteryInFight *
                                      selectedAttackEffect.multiplicatorLifeTaken;
                    selectedAttackEffect.ProcessAttackEffect();
                    selectedAttackEffect.TakeLifeFromPlayer(lifeTaken);
                }

                animatorPlayer.Play(attackData.damageMaxBonus * ratio == 0 ? "goodsize anime attaque" : "goodsize anime attaque spé");
                
                if (TutorialFightManager.instance.isInTutorialCombat &&
                    TutorialFightManager.instance.currentStep == CombatTutorialStep.ExplainPlayButton)
                {
                    TutorialFightManager.instance.AdvanceStep();
                }
                
                Debug.Log($"Dégâts infligés : {finalDamage} | Chance d'Overload : {currentOverloadChance}%");
            }
            else
            {
                Debug.Log("Attaque pas");
                Debug.Log(selectedAttack);
                Debug.Log(selectedAttackEffect);
                
            }
        }
        
        public void EndFighterTurnForPlayer()
        {
            if (FightManager.instance == null)
            {
                Debug.LogWarning("No FightManager Detected");
                return;
            }
            FightManager.instance.EndFighterTurn();
        }
        
    }
}
