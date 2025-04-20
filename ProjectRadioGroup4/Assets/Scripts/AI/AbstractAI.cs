using System;
using Controller;
using DATA.Script.Entity_Data.AI;
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

        private enum TypeOfAi
        {
            AlwaysAttack, //no logic behind
            SmartAi, // With thought process
            RandomAiWithCondition, //Pokemon like ai even if it bothers Stoian -_-
        }
        
        private void Update()
        {
            AiShift();
            AiBehavior();
        }

        public float PvEnemy
        {
            get => _abstractEntityDataInstance.hp;
            set
            {
                _abstractEntityDataInstance.hp = value;

                if (_abstractEntityDataInstance.IsDead())
                {
                    HandleDeath();
                }
                else
                {
                    RadioController.instance.UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
                }
            }
        }

        #region Relier a PvEnemy pour moins profondeur ducoup plus de lisibilité

        private void HandleDeath()
        {
            TryPostZeroBombEffect();

            RadioController.instance.listOfEveryEnemy.Remove(this);
    
            TryRemoveFromDetectedList();

            Destroy(gameObject);
        }

        private void TryPostZeroBombEffect()
        {
            if (!_abstractEntityDataInstance.postZeroDeal.postZeroBomb) return;
            _abstractEntityDataInstance.postZeroDeal.postZeroBomb = false;
            Debug.Log("Caboum");
            foreach (AbstractEntityDataInstance enemyInstance in FightManager.instance.listOfJustEnemiesAlive)
            {
                AbstractAI enemyAI = enemyInstance.entity.GetComponent<AbstractAI>();
                if (enemyAI != null)
                {
                    enemyAI.PvEnemy -= _abstractEntityDataInstance.postZeroDeal.damageStockForAfterDeath;
                }
            }

            
        }

        private void TryRemoveFromDetectedList()
        {
            try
            {
                RadioController.instance.listOfDetectedEnemy.Remove(this);
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        #endregion
        

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
            if (!_abstractEntityDataInstance.reveal && _aiFightState != AiFightState.InFight) return;
            
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
            
            float randomValueForFlash = Random.Range(0f, 1f);

            if (randomValueForFlash < 0.25f && _abstractEntityDataInstance.flashed)
            {
                Debug.Log("Flashed so cant attack");
                _abstractEntityDataInstance.flashed = false;
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
        private void ClassicAttack(float _damageDeal, float _batteryGain,string _attackName)
        {
            PlayerController.instance.ManageLife(_damageDeal);
            _abstractEntityDataInstance.battery += _batteryGain;
            Debug.Log(_attackName);
        }
        private void NormalAttack() => ClassicAttack(
            _abstractEntityDataInstance.normalAttack.damage,
            _abstractEntityDataInstance.normalAttack.batteryGain,
            "Normal Attack Done");
        
        private void HeavyAttack() => ClassicAttack(
            _abstractEntityDataInstance.heavyAttack.damage,
            _abstractEntityDataInstance.heavyAttack.batteryGain,
            "Heavy Attack Done");
        private void StealBatteries() => ClassicAttack(
            _abstractEntityDataInstance.stealBatteries.damage,
            _abstractEntityDataInstance.stealBatteries.batteryGain,
            "Steal Batteries Done");
        private void StealALotBatteries() => ClassicAttack( 
            _abstractEntityDataInstance.stealALotBatteries.damage,
            _abstractEntityDataInstance.stealALotBatteries.batteryGain,
            "Steal a lot of Batteries Done");

        private void ElectricalLeak() => ClassicAttack(
            -_abstractEntityDataInstance.battery,
            -_abstractEntityDataInstance.battery,
            "Electrical Leak Done");

        #endregion

    }
}
