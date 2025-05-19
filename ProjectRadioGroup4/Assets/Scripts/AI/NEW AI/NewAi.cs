using DATA.Script.Entity_Data.AI;
using ENUM;
using INTERACT;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace AI.NEW_AI
{
    //NORMAL NEWAI HAS A LOT IN COMMON WITH THE BATTERY I SUPPOSE PLEASE IN THE FUTURE IF THE FIGHTMANAGER FUCKED UP 
    // EVERYTHING I M GONNA HANG MY SELF
    
    public class NewAi : BatteryInteract
    {
        
        [Header("Attack Settings")]
        [SerializeField] private float attackTriggerDelay = 2f; 
        private float attackTimer;
        private bool isTimerRunning;
        
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


        private void Update()
        {
            TimerIfInteractWithPlayer();
        }

        
        #region LIFE HANDLER

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
        
        
        #region OVERRIDE METHODE
        
        //USEFULL FOR INIT THE FIGHTMANAGER
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            var fightManager = FightManager.instance;
            if (!other.gameObject.CompareTag("Player")) return;
            
            CancelInteractionAfterContact();
            
            if (fightManager != null) 
            {
                fightManager.currentFightAdvantage = Detected ?
                FightManager.FightAdvantage.Advantage : FightManager.FightAdvantage.Disadvantage;
                fightManager.InitialiseList();
            }
        }

        protected override void OnTriggerEnter2D(Collider2D other)
        {
            base.OnTriggerEnter2D(other);
            
            
            if (!other.CompareTag("Player") || _aiFightState == AiFightState.InFight) return;
            
            attackTimer = attackTriggerDelay;
            isTimerRunning = true;
        }

        #endregion

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
            var player = NewPlayerController.instance;
            if (player == null) return;

            
            Vector3 combatPosition = player.transform.position + 
                                     player.transform.right * combatDistance * 
                                     (player.spriteRendererPlayer.flipX ? -1 : 1);
            
            transform.position = combatPosition;
            FacePlayer();
            
            if (visualComponent != null)
                visualComponent.SetActive(true);
            
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
            var fightManager = FightManager.instance;
            if (fightManager == null) return;

            _aiFightState = AiFightState.InFight;
            
            fightManager.currentFightAdvantage = Detected ? 
                FightManager.FightAdvantage.Advantage : 
                FightManager.FightAdvantage.Disadvantage;
                
            fightManager.InitialiseList();
        }
        
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
    }
}
