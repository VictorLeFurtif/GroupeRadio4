using System;
using System.Collections;
using Controller;
using DATA.Script.Entity_Data.AI;
using INTERFACE;
using MANAGER;
using UI.Link_To_Radio;
using UnityEngine;
using Random = UnityEngine.Random;

namespace AI.OLD_AI_BEFORE_FP_V2
{
    [RequireComponent(typeof(Rigidbody2D), (typeof(BoxCollider2D)))]
    public abstract class AbstractAI : MonoBehaviour, IAi
    {
        [SerializeField] protected AbstractEntityData _abstractEntityData;
        public AbstractEntityDataInstance _abstractEntityDataInstance;
        public AiFightState _aiFightState = AiFightState.OutFight;

        public bool isAiForTuto;

        private SpriteRenderer enemySpriteRenderer;
        public Animator animatorEnemyAi;
        private Collider2D myCollider;

        protected bool canAttack = true;
        private bool waitingForAction = false;
        [SerializeField] private float timeForAiTurn;
        protected bool isPerformingAttack = false;

        private bool isDead = false;

        public enum AiFightState
        {
            InFight,
            OutFight
        }

        private void Update()
        {
            if (isDead) return;

            AiIfReveal();

            if (_abstractEntityDataInstance.turnState == FightManager.TurnState.Turn && canAttack && !waitingForAction)
            {
                StartCoroutine(DelayedAiBehavior());
            }
        }

        private IEnumerator DelayedAiBehavior()
        {
            waitingForAction = true;
            yield return new WaitForSeconds(timeForAiTurn);
            if (!isDead) AiBehavior();
            waitingForAction = false;
        }

        public float PvEnemy
        {
            get => _abstractEntityDataInstance.hp;
            set
            {
                if (isDead) return;

                _abstractEntityDataInstance.hp = value;

                SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.SelectRandomSoundFromList(
                    SoundManager.instance.soundBankData.enemySound.listVocalEnemy));

                if (_abstractEntityDataInstance.IsDead())
                {
                    Die();
                }
                else
                {
                    RadioController.instance.UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
                    if (!isPerformingAttack)
                    {
                        animatorEnemyAi.Play("takeDamageMonster");
                    }
                }
            }
        }

        private void Die()
        {
            if (isDead) return;

            isDead = true;
            canAttack = false;

            animatorEnemyAi.Play("DeathAi");

            if (_aiFightState == AiFightState.InFight)
            {
                EndAiTurn();
            }

            StartCoroutine(DelayedDeath(1.2f)); // <-- adapte au timing de ton anim
        }

        private IEnumerator DelayedDeath(float delay)
        {
            yield return new WaitForSeconds(delay);
            HandleDeath();
        }

        private void HandleDeath()
        {
            TryPostZeroBombEffect();
            RadioController.instance.listOfEveryEnemy.Remove(this);
            TryRemoveFromDetectedList();
            TryRemoveFromListFighterAlive();
            Destroy(gameObject);
        }

        private void TryRemoveFromListFighterAlive()
        {
            if (FightManager.instance == null) return;
            // Ici tu peux implémenter si besoin
        }

        private void TryPostZeroBombEffect()
        {
            if (!_abstractEntityDataInstance.postZeroDeal.postZeroBomb) return;

            _abstractEntityDataInstance.postZeroDeal.postZeroBomb = false;
            Debug.Log("Caboum");

            foreach (AbstractEntityDataInstance enemyInstance in FightManager.instance.listOfJustEnemiesAlive)
            {
                AbstractAI enemyAI = enemyInstance.entity.GetComponent<AbstractAI>();
                if (enemyAI != null && enemyAI != this)
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

        private void Start()
        {
            Init();
            myCollider = GetComponent<Collider2D>();
            animatorEnemyAi = GetComponent<Animator>();
        }

        protected virtual void Init()
        {
            _abstractEntityDataInstance = _abstractEntityData.Instance(gameObject);
            _aiFightState = AiFightState.OutFight;
            enemySpriteRenderer = GetComponent<SpriteRenderer>();
            enemySpriteRenderer.enabled = false;
            AddAiToListOfEveryEnemy();
        }

        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            if (other.gameObject.layer != 6 || _aiFightState == AiFightState.InFight) return;

            if (isAiForTuto && TutorialFightManager.instance != null)
            {
                TutorialFightManager.instance.isInTutorialCombat = true;
                TutorialFightManager.instance.ShowCurrentStep();
            }

            _aiFightState = AiFightState.InFight;
            enemySpriteRenderer.enabled = true;
            FightManager.instance.fightState = FightManager.FightState.InFight;
            FightManager.instance.InitialiseList();

            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.SelectRandomSoundFromList(
                SoundManager.instance.soundBankData.enemySound.listVocalEnemy));

            if (!_abstractEntityDataInstance.reveal)
            {
                animatorEnemyAi.Play("SpawnAi");
            }

            RadioController.instance?.SelectEnemyByLight();
        }

        private void OnMouseUpAsButton()
        {
            if (isDead) return;

            if (PlayerController.instance._abstractEntityDataInstance.turnState == FightManager.TurnState.Turn &&
                FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                PlayerController.instance.selectedEnemy = gameObject;
            }
        }

        private void AiIfReveal()
        {
            if (!_abstractEntityDataInstance.reveal && _aiFightState != AiFightState.InFight) return;
            enemySpriteRenderer.enabled = true;
        }

        protected virtual void AiBehavior()
        {
            if (isDead || _abstractEntityDataInstance.turnState == FightManager.TurnState.NoTurn || !canAttack) return;

            if (PlayerController.instance == null)
            {
                Debug.LogError("No player instance found: Singleton problem with PlayerController");
                return;
            }

            if (_abstractEntityDataInstance.vodkaOndeRadio.isVodka)
            {
                foreach (var enemyAI in FightManager.instance.listOfJustEnemiesAlive)
                {
                    enemyAI.entity.GetComponent<AbstractAI>().PvEnemy -= 15; // MAGIC NUMBER
                    if (enemyAI.IsDead()) return;
                }

                _abstractEntityDataInstance.vodkaOndeRadio.compteurVodka++;

                if (_abstractEntityDataInstance.vodkaOndeRadio.compteurVodka == 2)
                {
                    _abstractEntityDataInstance.vodkaOndeRadio.compteurVodka = 0;
                    _abstractEntityDataInstance.vodkaOndeRadio.isVodka = false;
                }
            }

            float randomValueForFlash = Random.Range(0f, 1f);

            if (randomValueForFlash < 0.25f && _abstractEntityDataInstance.flashed)
            {
                Debug.Log("Flashed so can't attack");
                animatorEnemyAi.Play("Flashed");
                _abstractEntityDataInstance.flashed = false;
                canAttack = false;
                CallBackFeedBackPlayer.Instance.ShowMessage("Enemy is flashed");
                return;
            }

            _abstractEntityDataInstance.flashed = false;
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

        public void PlayAttackAiSound()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.SelectRandomSoundFromList(
                SoundManager.instance.soundBankData.enemySound.listEnemyAttack));
        }

        public void AddAiToListOfEveryEnemy()
        {
            if (RadioController.instance == null) return;
            RadioController.instance.listOfEveryEnemy.Remove(this);
            RadioController.instance.listOfEveryEnemy.Add(this);
        }
    }
}
