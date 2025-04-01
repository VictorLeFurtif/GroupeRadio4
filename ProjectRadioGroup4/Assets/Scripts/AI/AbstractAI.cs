using Controller;
using DATA.Script.Entity_Data.AI;
using DATA.ScriptData.Entity_Data;
using MANAGER;
using UnityEngine;

namespace AI
{
    [RequireComponent(typeof(Rigidbody2D),(typeof(BoxCollider2D)))]
    public abstract class AbstractAI : MonoBehaviour
    {
        [SerializeField]
        private AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        public AiFightState _aiFightState;
        
        private SpriteRenderer enemySpriteRenderer;

        public enum AiFightState
        {
            InFight,
            OutFight
        }
        
        private void Update()
        {
            AiShift();
            AiBehavior();
        }

        public int PvEnemy
        {
            get => _abstractEntityDataInstance.hp;
            set
            {
                _abstractEntityDataInstance.hp = value;
                if (_abstractEntityDataInstance.IsDead())
                {
                    RadioController.instance.listOfEveryEnemy.Remove(this);
                    Destroy(gameObject);
                }
            }
        }

        protected abstract void AiShift();

        private void Start()
        {
            Init();
        }

        protected virtual void Init()
        {
            _abstractEntityDataInstance = _abstractEntityData.Instance(gameObject);
            _aiFightState = AiFightState.OutFight;
            enemySpriteRenderer = GetComponent<SpriteRenderer>();
            enemySpriteRenderer.enabled = false;
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            // no need to implement virtual/override because in each type of ai the trigger will launch the FightManager
            if (!other.CompareTag("Player") || _aiFightState == AiFightState.InFight) return;
            Debug.Log("Player in Fight Zone");
            _aiFightState = AiFightState.InFight;
            FightManager.instance.fightState = FightManager.FightState.InFight;
            FightManager.instance.InitialiseList();
        }
        
        private void OnMouseUpAsButton()
        {
            if (PlayerController.instance._abstractEntityDataInstance.turnState == FightManager.TurnState.Turn 
                && FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                PlayerController.instance.selectedEnemy = gameObject;
            }   
        }

        private void AiBehavior()
        {
            if (_abstractEntityDataInstance.notHidden || _aiFightState == AiFightState.InFight)
            {
                enemySpriteRenderer.enabled = true;
            }
        }
        
        protected void SwitchSpriteRenderer(SpriteRenderer _spriteRenderer)
        {
            _spriteRenderer.enabled = _spriteRenderer.enabled switch
            {
                true => false,
                false => true
            };
        }
    }
}
