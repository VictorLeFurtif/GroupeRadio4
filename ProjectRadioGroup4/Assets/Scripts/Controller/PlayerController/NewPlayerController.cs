using System.Collections;
using System.Collections.Generic;
using Controller;
using DATA.Script.Entity_Data.AI;
using DATA.Script.Entity_Data.Player;
using MANAGER;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class NewPlayerController : MonoBehaviour
{
    public static NewPlayerController instance;

        [Header("Speed")]
        [SerializeField] private float moveSpeed;
        public bool canMove = true;

        [Header("Rigidbody2D")]
        public Rigidbody2D rb;

        [Header("Data Player")]
        [SerializeField] private AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        public PlayerDataInstance _inGameData;

        public SpriteRenderer spriteRendererPlayer;

        [Header("State Machine")]
        public PlayerStateExploration currentPlayerExplorationState = PlayerStateExploration.Exploration;

        [Header("Animation")]
        public Animator animatorPlayer;
        
        [Header("Battery")] private BatteryPlayer playerBattery;
        
        [SerializeField]
        private float epsilonValidationOscillation;
        
        private float nextFootstepTime;

        public enum PlayerStateExploration
        {
            Exploration,
            Guessing,
        }

        private void Awake()
        {
            Init();
        }
        
        private void Update()
        {
            PlayerMove();
            CheckForFlipX();
        }

        private void Init()
        {
            rb = GetComponent<Rigidbody2D>();

            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);

            _abstractEntityDataInstance = _abstractEntityData.Instance(gameObject);
            _inGameData = (PlayerDataInstance)_abstractEntityDataInstance;

            rb.interpolation = RigidbodyInterpolation2D.Interpolate;
            spriteRendererPlayer = GetComponent<SpriteRenderer>();
            animatorPlayer = GetComponent<Animator>();
        }

        #region LifeHandle

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
        
        public void PlayGameOver()
        {    
            GameManager.instance?.GameOver();
        }

        #endregion
        
        #region Movement

        private void PlayerMove()
        {
            if (!canMove)
            {
                rb.velocity = Vector2.zero;
                animatorPlayer.SetFloat("MoveSpeed", 0);
                return;
            }

            float x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(x * moveSpeed, rb.velocity.y);
            animatorPlayer.SetFloat("MoveSpeed", Mathf.Abs(rb.velocity.x));

            if (Mathf.Abs(x) > 0.1f && Time.time > nextFootstepTime)
            {
                SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.walk);
                nextFootstepTime = Time.time + 0.35f;
            }
        }

        private bool wasFacingLeft = false;

        private void CheckForFlipX()
        {
            if (Mathf.Abs(rb.velocity.x) > 0.1f)
                wasFacingLeft = rb.velocity.x < 0;

            spriteRendererPlayer.flipX = wasFacingLeft;
        }

        public void ChangeBoolPlayerCanMove(float active)
        {
            canMove = active > 0.5f;
        }

        #endregion
}
