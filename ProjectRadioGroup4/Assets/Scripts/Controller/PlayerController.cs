using System;
using System.Collections.Generic;
using AI;
using DATA.Script.Attack_Data;
using DATA.Script.Entity_Data.AI;
using DATA.ScriptData.Entity_Data;
using MANAGER;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UIElements;
using Slider = UnityEngine.UI.Slider;

namespace Controller
{
    [RequireComponent(typeof(Rigidbody2D),typeof(CapsuleCollider2D))]
    public class PlayerController : MonoBehaviour
    {
        public static PlayerController instance;
    
        [Header("Speed")] [SerializeField] private float moveSpeed;

        [Header("Rigidbody2D")] [SerializeField]
        private Rigidbody2D rb;

        [Header("Data Player")]
        [SerializeField]
        private AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        private PlayerDataInstance _inGameData;

        public SpriteRenderer spriteRendererPlayer;

        [Header("UI")]
        [SerializeField] private GameObject playerFightCanva;
        [SerializeField] private Slider sliderOscillationPlayer;

        [Header("Selected Fighter")] public GameObject selectedEnemy;

        [Header("Attacks Player")] [SerializeField] private List<PlayerAttack> listOfPlayerAttack;
        
        public List<PlayerAttackInstance> listOfPlayerAttackInstance = new List<PlayerAttackInstance>();

        public PlayerAttackInstance selectedAttack;

        [FormerlySerializedAs("currentPlayerCoreGameState")] [Header("State Machine")]
        public PlayerStateExploration currentPlayerExplorationState = PlayerStateExploration.Exploration;
        
        public enum PlayerStateAnimation
        {
            Idle,
            Running,
            Walking,
            Dead
        }

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
        }

        public void ManageLife(int valueLifeChanger)
        {
            HealthPlayer += valueLifeChanger;
        }

        private int HealthPlayer 
        {
            get => _inGameData.hp;

            set
            {
                _inGameData.hp = value;
                if (_inGameData.IsDead())
                {
                    GameManager.instance.GameOver();
                    //They lose
                }
            }
        }
    
        private void Update()
        {
            PlayerMove();
            CheckForFlipX();
            ManageFight();
            SliderOscillationPlayerBehavior();
        }

        private void Start()
        {
            InitializeListOfAttackPlayer();
            InitializeSliderOscillationPlayer();
        }

        private void InitializeSliderOscillationPlayer() //wave amp comprit entre 0 et 0.4
        {
            sliderOscillationPlayer.maxValue = 0.4f;
            sliderOscillationPlayer.onValueChanged.AddListener(UpdateAmplitude);
        }
       
        private void UpdateAmplitude(float newValue)
        {
            RadioController.instance.matRadioPlayer.SetFloat("_waves_Amp", newValue);
        }
        
        private void PlayerMove()
        {
            if (FightManager.instance.fightState != FightManager.FightState.OutFight || currentPlayerExplorationState == PlayerStateExploration.Guessing)
            {
                rb.velocity = new Vector2(0,0);
                return;
            }
            var x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(x  * moveSpeed,rb.velocity.y);
        }

        private bool wasFacingLeft = false; //bool pour stock là où il regardait

        private void CheckForFlipX()
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
            {
                wasFacingLeft = rb.velocity.x < 0;
            }
            spriteRendererPlayer.flipX = wasFacingLeft;
        }

        private void ManageFight() //temporary
        {
            playerFightCanva.SetActive(_inGameData.turnState == FightManager.TurnState.Turn);
        }

        public void KillInstantEnemy()
        {
            if (selectedEnemy == null ) 
            { 
                return;
            }
            selectedEnemy.GetComponent<AbstractAI>().PvEnemy = -5;
            FightManager.instance.EndFighterTurn();
        }

        private void InitializeListOfAttackPlayer()
        {
            foreach (PlayerAttack attack in listOfPlayerAttack)
            {
                listOfPlayerAttackInstance.Add(attack.Instance());
            }
        }

        private void SliderOscillationPlayerBehavior()
        {
            if (currentPlayerExplorationState == PlayerStateExploration.Guessing)
            {
                sliderOscillationPlayer.interactable = true;
            }
            else
            {
                sliderOscillationPlayer.interactable = false;
            }
        }

        [SerializeField]
        private float epsilonValidationOscillation;
        
        public void ValidButtonOscillation()
        {
            if (!(Mathf.Abs(sliderOscillationPlayer.value -
                            RadioController.instance.matRadioEnemy.GetFloat("_waves_Amp")) <
                  epsilonValidationOscillation) ||
                currentPlayerExplorationState != PlayerStateExploration.Guessing)
            {
                return;
            }
            Debug.Log("Pass the epsilon");
            foreach (AbstractAI enemy in RadioController.instance.listOfDetectedEnemy)
            {
                enemy._abstractEntityDataInstance.notHidden = true;
            }
        }
    }
}
