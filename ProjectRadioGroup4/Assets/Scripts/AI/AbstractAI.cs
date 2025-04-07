using System;
using Controller;
using DATA.Script.Entity_Data.AI;
using DATA.ScriptData.Entity_Data;
using MANAGER;
using UnityEngine;
using Random = UnityEngine.Random;


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
        
        [SerializeField] private TypeOfAi aiType;

        public enum AiFightState
        {
            InFight,
            OutFight
        }
        
        public enum TypeOfAi
        {
            AlwaysAttack, //no logic behind
            SmartAi, // With thought process
            RandomAiWithCondition, //Pokemon like ai
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
            if (!_abstractEntityDataInstance.notHidden && _aiFightState != AiFightState.InFight) return;
            
            enemySpriteRenderer.enabled = true;
            
            if (_abstractEntityDataInstance.turnState == FightManager.TurnState.NoTurn)
            {
                return;
            }

            switch (aiType)
            {
                case TypeOfAi.AlwaysAttack:
                    AlwaysAttackAiBehavior();
                    break;
                case TypeOfAi.SmartAi:
                    SmartAiBehavior();
                    break;
                case TypeOfAi.RandomAiWithCondition:
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
            FightManager.instance.EndFighterTurn();
        }
        
        protected void SwitchSpriteRenderer(SpriteRenderer _spriteRenderer)
        {
            _spriteRenderer.enabled = _spriteRenderer.enabled switch
            {
                true => false,
                false => true
            };
        }

        private void AlwaysAttackAiBehavior()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.ManageLife(-5); // temporary
            }
            else
            {
                Debug.LogError("No player instance found : Singleton problem occured with PlayerController");
            }
        }

        private void SmartAiBehavior()
        {
            if (PlayerController.instance == null)
            {
                Debug.LogError("No player instance found: Singleton problem with PlayerController");
                return;
            }

            float randomValue = Random.Range(0f, 1f);
            
            if (_abstractEntityDataInstance.IsBatteryMoreThanHundred())
            {
                Kamikaze();
                return;
            }

            bool isLowBattery = !_abstractEntityDataInstance.IsBatteryEqualOrMoreThanFifty();
            float healthPercentage = (float)PvEnemy / _abstractEntityData.Hp;

            if (isLowBattery)
            {
                HandleLowBatteryActions(randomValue, healthPercentage);
            }
            else
            {
                HandleHighBatteryActions(randomValue, healthPercentage);
            }
        }

        private void HandleLowBatteryActions(float randomValue, float healthPercentage)
        {
            healthPercentage = Mathf.Clamp01(healthPercentage);
            switch (healthPercentage)
            {
                case < 0.33f:
                {
                    if (randomValue < 0.15f) NormalAttack();
                    else if (randomValue < 0.30f) HeavyAttack();
                    else if (randomValue < 0.55f) StealBatteries();
                    else StealALotBatteries();
                    break;
                }
                case < 0.66f:
                {
                    if (randomValue < 0.3f) NormalAttack();
                    else if (randomValue < 0.5f) HeavyAttack();
                    else if (randomValue < 0.8f) StealBatteries();
                    else StealALotBatteries();
                    break;
                }
                default:
                {
                    if (randomValue < 0.4f) NormalAttack();
                    else if (randomValue < 0.7f) HeavyAttack();
                    else if (randomValue < 0.9f) StealBatteries();
                    else StealALotBatteries();
                    break;
                }
            }
        }

        private void HandleHighBatteryActions(float randomValue, float healthPercentage)
        {
            healthPercentage = Mathf.Clamp01(healthPercentage);

            switch (healthPercentage)
            {
                case < 0.33f:
                {
                    if (randomValue < 0.15f) HeavyAttack();
                    else if (randomValue < 0.5f) StealBatteries();
                    else if (randomValue < 0.75f) StealALotBatteries();
                    else ElectricalLeak();
                    break;
                }
                case < 0.66f:
                {
                    if (randomValue < 0.33f) HeavyAttack();
                    else if (randomValue < 0.56f) StealBatteries();
                    else if (randomValue < 0.77f) StealALotBatteries();
                    else ElectricalLeak();
                    break;
                }
                default:
                {
                    if (randomValue < 0.35f) HeavyAttack();
                    else if (randomValue < 0.5f) StealBatteries();
                    else if (randomValue < 0.6f) StealALotBatteries();
                    else ElectricalLeak();
                    break;
                }
            }
        }


        #region AI Thought Process
        
        private void Kamikaze()
        {
            PlayerController.instance.ManageLife(-38); //temporary floating value
            PvEnemy = 0;
            Debug.Log("Explode");
        }
        private void ClassicAttack(int _damageDeal, int _batteryGain)
        {
            PlayerController.instance.ManageLife(_damageDeal);
            _abstractEntityDataInstance.battery += _batteryGain;
        }
        private void NormalAttack() => ClassicAttack(-18, 6);
        private void HeavyAttack() => ClassicAttack(-24,8);
        private void StealBatteries() => ClassicAttack(-12,8);
        private void StealALotBatteries() => ClassicAttack(-18, 18);

        private void ElectricalLeak() => ClassicAttack(-_abstractEntityDataInstance.battery / 5,
            -_abstractEntityDataInstance.battery / 5);

        #endregion

    }
}
