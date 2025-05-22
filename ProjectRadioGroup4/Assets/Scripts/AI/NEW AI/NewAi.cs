using Controller;
using DATA.Script.Entity_Data.AI;
using ENUM;
using INTERACT;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace AI.NEW_AI
{
    public class NewAi : BatteryInteract, IAi
    {
        #region Fields
        [Header("Attack Settings")]
        public float attackTriggerDelay = 2f; 
        public float attackTimer;
        public bool isTimerRunning;
        
        [Header("State Machine")]
        public AiFightState _aiFightState = AiFightState.OutFight;
        
        [Header("Data")]
        [SerializeField] protected AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        
        private bool isDead = false;
        protected bool canAttack = true;
        
        [Header("Combat Settings")]
        [SerializeField] private float combatDistance = 2f; 
        [SerializeField] private GameObject visualComponent;

        [HideInInspector] public Animator animatorEnemy;
        
        
        #endregion

        #region Unity Methods
        private void Update()
        {
            TimerIfInteractWithPlayer();
        }

        protected override void Start()
        {
            base.Start();
            AddAiToListOfEveryEnemy();
            _abstractEntityDataInstance = _abstractEntityData.Instance(gameObject);
            animatorEnemy = GetComponent<Animator>();
        }
        #endregion

        #region Life Handler
        public float PvEnemy
        {
            get => _abstractEntityDataInstance.hp;
            set
            {
                if (isDead) return;

                _abstractEntityDataInstance.hp = value;

                if (_abstractEntityDataInstance.IsDead())
                {
                    Die();
                }
            }
        }
        
        private void Die()
        {
            if (isDead) return;

            isDead = true;
            canAttack = false;
            
            if (_aiFightState == AiFightState.InFight)
            {
                EndAiTurn();
            }
        }
        #endregion
        
        #region Collision Handling
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            var fightManager = FightManager.instance;
            if (!other.gameObject.CompareTag("Player")) return;
            
            CancelInteractionAfterContact();
            
            StartCombatSequence();
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (_aiFightState != AiFightState.OutFight) return;
            base.OnTriggerEnter2D(other);
            
        }
        #endregion

        #region Combat Management
        private void TimerIfInteractWithPlayer()
        {
            if (!isTimerRunning) return;
            attackTimer -= Time.deltaTime;

            if (!(attackTimer <= 0f)) return;
            isTimerRunning = false;
            
            StartCombatSequence();
        }
        
        private void StartCombatSequence()
        {
            NewRadioManager.instance?.StopMatchingGame();
            spriteRenderer.enabled = true;
            var player = NewPlayerController.instance;
            if (player == null) return;

            Vector3 combatPosition = player.transform.position + 
                                 player.transform.right * combatDistance * 
                                 (player.spriteRendererPlayer.flipX ? -1 : 1);
            
            transform.position = combatPosition;
            FacePlayer();
            InitiateCombat();
        }
        
        private void FacePlayer()
        {
            var player = NewPlayerController.instance;
            if (player == null) return;
            
            transform.localScale = transform.position.x > 
                               player.transform.position.x ? new Vector3(-1, 1, 1) : Vector3.one;
        }

        private void InitiateCombat()
        {
            var instance = NewPlayerController.instance;
            if (instance != null) 
            {
                instance.currentInteractableInRange = this;
                instance.canMove = false; 
            }

            spriteRenderer.enabled = true;
    
            var fightManager = FightManager.instance;
            if (fightManager == null) return;

            _aiFightState = AiFightState.InFight;
            fightManager.fightState = FightManager.FightState.InFight;
    
            fightManager.currentFightAdvantage = Detected ? 
                FightManager.FightAdvantage.Advantage : 
                FightManager.FightAdvantage.Disadvantage;

            if (fightManager.currentFightAdvantage == FightManager.FightAdvantage.Disadvantage)
            {
                CameraController.instance?.Shake(CameraController.ShakeMode.Both,1,100);
            }
    
            fightManager.InitialiseList();
            
            NewRadioManager.instance?.StartMatchingGameInFight();
        }
        #endregion
        
        #region AI Methods
        public void EndAiTurn()
        {
            if (FightManager.instance == null)
            {
                Debug.LogError("No FightManager found");
                return;
            }

            FightManager.instance.EndFighterTurn();
            canAttack = true;
        }

        public void AddAiToListOfEveryEnemy()
        {
            if (NewRadioManager.instance == null)
            {
                Debug.LogError("NewRadioManager instance is null!");
                return;
            }
            
            NewRadioManager.instance.listOfEveryEnemy.Remove(this);
            NewRadioManager.instance.listOfEveryEnemy.Add(this);
        }
        #endregion
    }
}