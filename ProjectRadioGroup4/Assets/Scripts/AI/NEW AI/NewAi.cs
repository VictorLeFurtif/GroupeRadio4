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
        [SerializeField] private float healthLerpDuration = 0.3f;
        private Coroutine healthLerpCoroutine;

        [Header("Enemy Chips")] 
        [SerializeField] private List<ChipsData> chipsDatasListTempo = new List<ChipsData>();
         public List<ChipsDataInstance> chipsDatasList = new List<ChipsDataInstance>();
        [HideInInspector] public List<ChipsDataInstance> chipsDatasListSave = new List<ChipsDataInstance>();

        [Header("Eye Settings")]
        public Image monsterEyes;
        private int currentChipIndex = 0;
        
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
            healthSlider.gameObject.SetActive(false);

            foreach (var t in chipsDatasListTempo)
            {
                chipsDatasList.Add(t.Instance());
            }
            chipsDatasListSave.AddRange(chipsDatasList);
            monsterEyes.material.color = Color.white; 
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
                
                UpdateHealthSlider(previousHealth, newHealth);

                if (_abstractEntityDataInstance.IsDead())
                {
                    Die();
                }
            }
        }
        
        private void UpdateHealthSlider(float fromHealth, float toHealth)
        {
            if (healthSlider == null) return;

            if (healthLerpCoroutine != null)
            {
                StopCoroutine(healthLerpCoroutine);
            }

            healthLerpCoroutine = StartCoroutine(LerpHealthSlider(
                fromHealth / _abstractEntityDataInstance.maxHp,
                toHealth / _abstractEntityDataInstance.maxHp
            ));
        }

        private IEnumerator LerpHealthSlider(float fromValue, float toValue)
        {
            float elapsedTime = 0f;

            while (elapsedTime < healthLerpDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / healthLerpDuration);
                healthSlider.value = Mathf.Lerp(fromValue, toValue, t);
                yield return null;
            }

            healthSlider.value = toValue;
            healthLerpCoroutine = null;
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
            StartCoroutine(DelayedDeath(1.2f));
        }
        
        private IEnumerator DelayedDeath(float delay)
        {
            yield return new WaitForSeconds(delay);
            HandleDeath();
        }

        private void HandleDeath()
        {
            Destroy(gameObject);
        }
        
        #endregion
        
        #region Collision Handling
        protected override void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            BeginFight();
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
        
        public void BeginFight()
        {
            CancelInteractionAfterContact();
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
            healthSlider.gameObject.SetActive(true);
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
    
            var fightManager = FightManager.instance;
            if (fightManager == null) return;

            _aiFightState = AiFightState.InFight;
            fightManager.fightState = FightManager.FightState.InFight;
    
            fightManager.currentFightAdvantage = Detected ? 
                FightManager.FightAdvantage.Advantage : 
                FightManager.FightAdvantage.Disadvantage;

            if (fightManager.currentFightAdvantage == FightManager.FightAdvantage.Disadvantage)
            {
                CameraController.instance?.Shake(CameraController.ShakeMode.Both,1,45);
            }
    
            fightManager.InitialiseList();
            
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

        

        public void RemoveGuessedChips(int count)
        {
            if (chipsDatasList == null || count <= 0) return;
            
            chipsDatasList.RemoveRange(0, Mathf.Min(count, chipsDatasList.Count));
    
            Debug.Log($"Removed {count} chips from enemy sequence. Remaining: {chipsDatasList.Count}");
        }
        #endregion

        #region Color System
        public void UpdateEyeColorToCurrentChip()
        {
            if (chipsDatasList == null || chipsDatasList.Count == 0 || currentChipIndex >= chipsDatasList.Count)
            {
                monsterEyes.color = Color.white;
                return;
            }
    
            string colorName = chipsDatasList[currentChipIndex].colorLinkChips;
            monsterEyes.color = ConvertColorNameToColor(colorName);
        }

        public void MoveToNextChip()
        {
            if (chipsDatasList == null || currentChipIndex >= chipsDatasList.Count - 1) return;
    
            currentChipIndex++;
            UpdateEyeColorToCurrentChip();
        }

        public void ResetSequenceIndex(List<ChipsDataInstance> sequenceToUse)
        {
            currentChipIndex = 0;
            if (sequenceToUse != null && sequenceToUse.Count > 0)
            {
                UpdateEyeColorToCurrentChip();
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