using System.Collections.Generic;
using AI;
using AI.OLD_AI_BEFORE_FP_V2;
using DATA.Script.Attack_Data;
using DATA.Script.Attack_Data.New_System_Attack_Player;
using DATA.Script.Attack_Data.Old_System_Attack_Player;
using DATA.Script.Entity_Data.AI;
using DATA.Script.Entity_Data.Player;
using INTERFACE;
using MANAGER;
using UI.Link_To_Radio;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;


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
        
        
        [Header("Selected Fighter")] public GameObject selectedEnemy;

        [Header("Attacks Player")] [SerializeField] private List<PlayerAttackAbstract> listOfPlayerAttack;
        public List<PlayerAttackAbstractInstance> listOfPlayerAttackInstance = new List<PlayerAttackAbstractInstance>();
        
        public PlayerAttackAbstractInstance selectedAttack;
        public PlayerAttackAbstractInstance selectedAttackEffect;
        

        [FormerlySerializedAs("currentPlayerCoreGameState")] [Header("State Machine")]
        public PlayerStateExploration currentPlayerExplorationState = PlayerStateExploration.Exploration;
        
        [Header("Animation")]

        public Animator animatorPlayer;

        [Header("Lighting")] public Light2D lampTorch;
        public float lampTorchOnValue;
        public bool isLampTorchOn;

        [Header("Battery")] private BatteryPlayer playerBattery;
        
        private float nextFootstepTime;
        
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
            get => _inGameData?.hp ?? 0f;
            set
            {
                if (_inGameData == null)
                {
                    Debug.LogError(" _inGameData est NULL !");
                    return;
                }
                
                _inGameData.hp = Mathf.Max(0, value);
                
                playerBattery.UpdateLifeText();
                playerBattery.UpdateLifeSlider(_inGameData.hp);
                
                if (_inGameData.IsDead())
                {
                    canMove = false;
                    rb.velocity = Vector2.zero;
                    animatorPlayer.Play("Death");
                    return;
                }
                

                if (FightManager.instance != null && FightManager.instance.fightState == 
                    FightManager.FightState.InFight && _inGameData.turnState != FightManager.TurnState.Turn)
                {
                    animatorPlayer.Play("HitReceived");
                }
            }
        }


        /// <summary>
        /// Temporary shity Update
        /// </summary>

        public void PlayGameOver()
        {    
            GameManager.instance?.GameOver();
        }
        
        private void Update()
        {
            PlayerMove();
            CheckForFlipX();
            
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
            animatorPlayer.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));
            
            if (Mathf.Abs(x) > 0.1f && Time.time > nextFootstepTime)
            {
                SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.walk);
                nextFootstepTime = Time.time + 0.35f; 
            }
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
            foreach (PlayerAttackAbstract attack in listOfPlayerAttack)
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

        public bool canAttack = true;
        
        public void ValidButton()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.uxSound.click);
            
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
                    CameraController.instance?.Shake(CameraController.ShakeMode.Both,1f,10f);
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
                selectedAttack != null && selectedEnemy != null && canAttack && selectedAttack.attack.attackState == PlayerAttackAbstract.AttackState.Am)
            {
                PlayerAttackAbstract.AttackClassic attackData = selectedAttack.attack;
                float sliderMax = RadioController.instance.sliderOscillationPlayer.maxValue;
                float ratio = sliderMax > 0 ? RadioController.instance.sliderOscillationPlayer.value / sliderMax : 0f;
                canAttack = false;
                
                //TODO MEC FINAL DAMAGE AUCUN SENS AVEC LOGIQUE
                float finalDamage = attackData.damageMaxBonus * ratio + attackData.damage;
                float currentOverloadChance = attackData.chanceOfOverload * ratio;
                
                bool isOverload = Random.Range(0f, 100f) <= currentOverloadChance;
                
                if (isOverload)
                {
                    CallBackFeedBackPlayer.Instance.ShowMessage("Overload on player");
                    ManageLife(-finalDamage / 2);
                    animatorPlayer.Play("Overload");
                    return;
                }
                
                selectedEnemy.GetComponent<AbstractAI>().PvEnemy -= finalDamage;

                if (selectedAttack is { attack: { attackState: PlayerAttackAbstract.AttackState.Am } })
                {
                    if (selectedAttack is IPlayerAttack attackSelectedByPlayer)
                    {
                        attackSelectedByPlayer.ProcessAttack();
                        selectedAttack.TakeLifeFromPlayer();
                    }
                }
                else
                {
                    return;
                }
                

                if (selectedAttackEffect != null && selectedAttack is { attack: { attackState: PlayerAttackAbstract.AttackState.Am } })
                {
                    if (selectedAttackEffect is IPLayerEffect effect)
                    {
                        effect.ProcessEffect();
                        float lifeTaken = selectedAttack.attack.costBatteryInFight *
                                          selectedAttackEffect.multiplicatorLifeTaken;
                        selectedAttackEffect.TakeLifeFromPlayer(lifeTaken);
                    }
                }
                
                if (_inGameData.hp == 0)
                {
                    return;
                }
                
                animatorPlayer.Play(attackData.damageMaxBonus * ratio == 0 ? "goodsize anime attaque" : "goodsize anime attaque spé");

                CallBackFeedBackPlayer.Instance.ShowMessage(selectedAttackEffect != null
                    ? $"You apply {selectedAttack.attack.name} and {selectedAttackEffect.attack.name}"
                    : $"You apply {selectedAttack.attack.name} ");

                if (TutorialFightManager.instance != null && TutorialFightManager.instance.isInTutorialCombat &&
                    TutorialFightManager.instance.currentStep == CombatTutorialStep.ExplainPlayButton)
                {
                    TutorialFightManager.instance.AdvanceStep();
                }
                
                Debug.Log($"Dégâts infligés : {finalDamage} | Chance d'Overload : {currentOverloadChance}%");
            }
            
        }

        public void PlaySoundAttackPlayerLow()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.attackLowWave);
        }
        
        public void PlaySoundAttackPlayerStrong()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.attackStrongWave);
        }
        
        public void EndFighterTurnForPlayer()
        {
            if (FightManager.instance == null)
            {
                Debug.LogWarning("No FightManager Detected");
                return;
            }

            canAttack = true;
            FightManager.instance.EndFighterTurn();
        }
        
    }
}
