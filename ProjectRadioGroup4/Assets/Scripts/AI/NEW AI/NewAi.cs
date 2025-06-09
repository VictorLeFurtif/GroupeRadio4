using System.Collections;
using System.Collections.Generic;
using Controller;
using DATA.Script.Chips_data;
using DATA.Script.Entity_Data.AI;
using ENUM;
using INTERACT;
using INTERFACE;
using MANAGER;
using UnityEngine;
using UnityEngine.UI;

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

        [HideInInspector] public Transform originalPos;
        
        [Header("Enemy Health Settings")]
        [SerializeField] private Slider healthSlider;
        private Coroutine healthLerpCoroutine;

        [Header("Enemy Chips")] 
        [SerializeField] private List<ChipsData> chipsDatasListTempo = new List<ChipsData>();
        public List<ChipsDataInstance> chipsDatasList = new List<ChipsDataInstance>();
        [HideInInspector] public List<ChipsDataInstance> chipsDatasListSave = new List<ChipsDataInstance>();
        
        [Header("Enemy Chips but dont include Pattern, just what will be implemented")]
        public List<ChipsData> chipsToAddToPattern = new List<ChipsData>();
        [HideInInspector] public List<ChipsDataInstance> chipsToAddToPatternReal = new List<ChipsDataInstance>();

        [Header("Eye Settings")]
        public Image monsterEyes;
        private int currentChipIndex = 0;
        
        [Header("Damage")]
        
        public float damageEnemy;

        [Header("NUMBER OF SWAP")]
        public int numberOfSwap;

        [Header("TIMER FIGHTMANAGER")] public float timerFightManager;
        
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
            originalPos = transform;

            foreach (var t in chipsDatasListTempo)
            {
                chipsDatasList.Add(t.Instance());
            }
            chipsDatasListSave.AddRange(chipsDatasList);
            monsterEyes.material.color = Color.white;
            monsterEyes.gameObject.SetActive(false);
            
            foreach (var t in chipsToAddToPattern)
            {
                chipsToAddToPatternReal.Add(t.Instance());
            }
        }
        #endregion

        #region Life Handler
        public float PvEnemy
        {
            get => _abstractEntityDataInstance.hp;
            set
            {
                if (isDead || _abstractEntityDataInstance == null) return;

                float newHealth = Mathf.Clamp(value, 0f, _abstractEntityDataInstance.maxHp);
                float previousHealth = _abstractEntityDataInstance.hp;
                _abstractEntityDataInstance.hp = newHealth;
                
                

                if (_abstractEntityDataInstance.IsDead())
                {
                    StartCoroutine(Die());
                }
            }
        }
        
        
        private IEnumerator Die()
        {
            if (isDead) yield break ;

            isDead = true;
            canAttack = false;
            NewPlayerController.instance?.animatorPlayer.Play("goodsize anime attaque");

            yield return new WaitForSeconds(1f);
            
            animatorEnemy.Play("DeathAi");
            
            
            float deathAnimLength = GetDeathAnimationLength();
    
            if (_aiFightState == AiFightState.InFight)
            {
                EndAiTurn();
            }
            StartCoroutine(DelayedDeath(deathAnimLength));
        }

        private float GetDeathAnimationLength()
        {
            if (animatorEnemy == null)
            {
                return 2f; 
            }
            AnimatorStateInfo stateInfo = animatorEnemy.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.IsName("DeathAi"))
            {
                return stateInfo.length;
            }
            return 1.2f; 
        }

        private IEnumerator DelayedDeath(float delay)
        {
            yield return new WaitForSeconds(delay);
            HandleDeath();
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
            NewPlayerController.instance.canMove = true;
        }
        
        #endregion
        
        #region Collision Handling
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            StartFight();
            isTimerRunning = false;
        }
        
        protected override void OnTriggerEnter2D(Collider2D other)
        {
            if (_aiFightState != AiFightState.OutFight) return;
            base.OnTriggerEnter2D(other);
            
        }

        protected override void OnDestroy()
        {
            base.OnDestroy();
            NewRadioManager.instance?.listOfEveryEnemy.Remove(this);
        }

        #endregion

        #region Combat Management

        public void InitSwapNumber()
        {
            FightManager.instance.numberOfSwap = numberOfSwap;
        }
        
        public void StartFight()
        {
            if (_aiFightState == AiFightState.InFight) return;

            Debug.Log("1 - Method StartFight entered");
            NewPlayerController.instance.canMove = false;
            Debug.Log("2 - Player movement disabled");
            spriteRenderer.enabled = true;
            Debug.Log("3 - SpriteRenderer enabled");
            monsterEyes.gameObject.SetActive(true);
            Debug.Log("4 - Monster eyes activated");
            StartCoroutine(BeginFight());
            Debug.Log("5 - Coroutine started");
        }

        private IEnumerator BeginFight()
        {
            animatorEnemy.Play("SpawnAi");
            //yield return new WaitForSeconds(animatorEnemy.GetCurrentAnimatorStateInfo(0).length + 0.5f);
            yield return null;
            StartCombatSequence();
        }
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
            var player = NewPlayerController.instance;
            if (player == null) return;
            Vector3 combatPosition = player.transform.position + 
                                 player.transform.right * combatDistance;
            
            transform.position = combatPosition;
            InitiateCombat();
            
            
        }
        
        private void FacePlayer()
        {
            if (NewPlayerController.instance == null) return;
    
            bool playerIsOnLeft = NewPlayerController.instance.transform.position.x < transform.position.x;
            spriteRenderer.flipX = !playerIsOnLeft;
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
            monsterEyes.gameObject.SetActive(true);
    
            var fightManager = FightManager.instance;
            if (fightManager == null) return;

            _aiFightState = AiFightState.InFight;
            fightManager.fightState = FightManager.FightState.InFight;
    
            fightManager.currentFightAdvantage = Detected ? 
                FightManager.FightAdvantage.Advantage : 
                FightManager.FightAdvantage.Disadvantage;

            if (fightManager.currentFightAdvantage == FightManager.FightAdvantage.Disadvantage)
            {
                CameraController.instance?.Shake(CameraController.ShakeMode.Both,1,20);
                GameManager.instance.globalVolumeManager.GvColorToDesadvantage();
                SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.spawnNmiScreamer);
            }
            else
            {
                GameManager.instance.globalVolumeManager.GvColorToAdvantage();
                SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.spawnNmi);
            }
    
            fightManager.InitialiseFightManager();
            ChipsManager.Instance?.IniTabChipsDataInstanceInFight(this);
            InitSwapNumber();
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

        #region Color System
        public void UpdateEyeColorToCurrentChip()
        {
            if (chipsDatasList == null || chipsDatasList.Count == 0 || currentChipIndex >= chipsDatasListSave.Count)
            {
                monsterEyes.color = Color.white;
                return;
            }
    
            string colorName = chipsDatasListSave[currentChipIndex].colorLinkChips;
            monsterEyes.color = ConvertColorNameToColor(colorName);
        }

        public ChipsDataInstance GetActualInstanceChips()
        {
            return chipsDatasListSave[currentChipIndex];
        }

        public void MoveToNextChip()
        {
            if (chipsDatasList == null || currentChipIndex >= chipsDatasListSave.Count - 1)
            {
                return;
            }
    
            currentChipIndex++;
            UpdateEyeColorToCurrentChip();
        }

        public void ResetSequenceIndex(List<ChipsDataInstance> sequenceToUse)
        {
            currentChipIndex = 0;
            if (sequenceToUse != null && sequenceToUse.Count > 0)
            {
                UpdateEyeColorToCurrentChip();
                NewRadioManager.instance?.UpdateOscillationEnemy(this);
            }
        }
        
        private Color ConvertColorNameToColor(string colorName)
        {
            return colorName.ToLower() switch
            {
                "red" => Color.red,
                "green" => Color.green,
                "blue" => Color.blue,
                "yellow" => Color.yellow,
                "white" => Color.white,
                "black" => Color.black,
                _ => Color.magenta 
            };
        }
        #endregion
        
    }
}