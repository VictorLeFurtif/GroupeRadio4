using System;
using System.Collections.Generic;
using AI;
using DATA.Script.Attack_Data;
using DATA.Script.Entity_Data.AI;
using DATA.ScriptData.Entity_Data;
using MANAGER;
using UnityEngine;

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

        [Header("Selected Fighter")] public GameObject selectedEnemy;

        [Header("Attacks Player")] [SerializeField] private List<PlayerAttack> listOfPlayerAttack;
        
        public List<PlayerAttackInstance> listOfPlayerAttackInstance = new List<PlayerAttackInstance>();

        public PlayerAttackInstance selectedAttack;
        
        
        
        public enum PlayerState
        {
            Idle,
            Running,
            Walking,
            Dead
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
        }

        private void Start()
        {
            InitializeListOfAttackPlayer();
        }

        private void PlayerMove()
        {
            if (FightManager.instance.fightState != FightManager.FightState.OutFight)
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
        
        
    }
}
