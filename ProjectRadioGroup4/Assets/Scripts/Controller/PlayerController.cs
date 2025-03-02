using DATA.ScriptData.Entity_Data;
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

        [Header("Data Player")] [SerializeField]
        private AbstractEntityDataInstance _abstractEntityDataInstance;
        private PlayerDataInstance _inGameData; 
    
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
        
            _inGameData = (PlayerDataInstance)_abstractEntityDataInstance;
        
            rb.interpolation = RigidbodyInterpolation2D.Interpolate; // pour fix le bug lié à la caméra qui faisait trembler le perso
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
        }

        private void PlayerMove()
        {
            if (FightManager.instance.currentFighter != FightManager.FightState.NoFight) return;
            var x = Input.GetAxisRaw("Horizontal");
            rb.velocity = new Vector2(x  * moveSpeed,rb.velocity.y);

        }
    }
}
