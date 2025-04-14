using System.Collections.Generic;
using System.Linq;
using AI;
using DATA.Script.Attack_Data;
using MANAGER;
using TMPro;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;
using Random = UnityEngine.Random;


namespace Controller
{
    public class RadioController : MonoBehaviour
    {
        #region VARIABELS
        public static RadioController instance;
        [Header("Canvas")] [SerializeField] private Canvas canvaRadio;

        [FormerlySerializedAs("imageRadio")] [Header("Frequence Radio")] [SerializeField]
        private RawImage imageRadioPlayer;

        public Material matRadioPlayer;

        [SerializeField] private RawImage imageRadioEnemy;
        public Material matRadioEnemy;

        private RadioState currentRadioState;

        private AbstractAI currentClosestEnemy;

        [Header("Radio detection enemy parameters")]
        public List<AbstractAI> listOfEveryEnemy;

        [SerializeField] private float minDistance;
        [SerializeField] private float maxDistance;

        [Header("Frequency in fight parameter")] [SerializeField]
        private Slider sliderForFrequencyAttack;

        [SerializeField] private float maxValueSliderFrequencyAttack;

        [SerializeField] private TMP_Text descriptionAttackSelectedText;

        [Header("List of enemies detected"), SerializeField]
        public List<AbstractAI> listOfDetectedEnemy;

        [Header("Radio's parameter AM")] [SerializeField]
        private float desiredDistanceAm;

        [SerializeField] private float desiredDistanceFm;

        [Header("Layer Mask"), SerializeField] private LayerMask enemyLayerMask;

        public bool selectedAM = false;

        #endregion
    
        private enum RadioState
        {
            InFight,
            OutFight
        }

        private void Start()
        {
            currentRadioState = RadioState.OutFight;
            InitializeSliderFrequency();
            sliderForFrequencyAttack.onValueChanged.AddListener(delegate { ValueChangeCheck(); });
            InitializeRadioEnemy();
        
            //Intéressant au Start de placer la light sur l'élément 0. Pas plus de justification c'est moi qui décide
            UpdateRadioEnemyWithLight(0);
        }

        private void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }

            matRadioPlayer = imageRadioPlayer.material;
            matRadioEnemy = imageRadioEnemy.material;
        }
    
        #region OldSystemRadio
    
        private void UpdateRadioEnemyAfterDetection()
        {
            float waveAmpMoyenne = 0;
            float waveFreMoyenne = 0;
            foreach (AbstractAI enemy in listOfDetectedEnemy)
            {
                waveAmpMoyenne += enemy._abstractEntityDataInstance.waveAmplitudeEnemy;
                waveFreMoyenne += enemy._abstractEntityDataInstance.waveFrequency;
                print(waveAmpMoyenne + " " + waveFreMoyenne);
            }
            matRadioEnemy.SetFloat("_waves_Amount", waveFreMoyenne / listOfDetectedEnemy.Count);
            matRadioEnemy.SetFloat("_waves_Amp", waveAmpMoyenne / listOfDetectedEnemy.Count);
        } //useless but keep it in case
    
        private float CheckForClosestPlayer()
        {
            float stockDistance = Mathf.Infinity;
            AbstractAI stockEntity = null;
            float playerX = PlayerController.instance.transform.position.x;

            foreach (AbstractAI enemy in listOfEveryEnemy)
            {
                float enemyX = enemy.transform.position.x;
                float distance = Mathf.Abs(enemyX - playerX);
                if (distance < stockDistance)
                {
                    stockDistance = distance;
                    stockEntity = enemy;
                }
            }

            currentClosestEnemy = stockEntity;
            return stockDistance;
        }

        public void AddWave(float i)
        {
            var value = matRadioPlayer.GetFloat("_waves_Amount");
            matRadioPlayer.SetFloat("_waves_Amount", value + i);
        }

        public void AddWaveAmp(float i)
        {
            var value = matRadioPlayer.GetFloat("_waves_Amp");
            matRadioPlayer.SetFloat("_waves_Amp", value + i);
        }

        private void UpdateRadioUiInExploration()
        {
            if (listOfEveryEnemy.Count == 0 || currentRadioState == RadioState.InFight)
            {
                return;
            }

            var distance = CheckForClosestPlayer();

            float waveAmp = CalculateWaveAmplitude(distance, minDistance, maxDistance);
            float waveAmount = CalculateWaveAmount(distance, minDistance, maxDistance);

            matRadioPlayer.SetFloat("_waves_Amp", waveAmp);
            matRadioPlayer.SetFloat("_waves_Amount", waveAmount);
        }

        private float CalculateWaveAmplitude(float distance, float _minDistance, float _maxDistance)
        {
            float normalizedDistance = Mathf.InverseLerp(_minDistance, _maxDistance, distance);
            return Mathf.Lerp(1f, 0.1f, normalizedDistance);
        }

        private float CalculateWaveAmount(float distance, float _minDistance, float _maxDistance)
        {
            float normalizedDistance = Mathf.InverseLerp(_minDistance, _maxDistance, distance);
            return Mathf.Lerp(20f, 1f, normalizedDistance);
        }

        #endregion

        private void InitializeSliderFrequency()
        {
            sliderForFrequencyAttack.maxValue = maxValueSliderFrequencyAttack;
            ValueChangeCheck();
        }

        [SerializeField] private float epsilonForSliderAttack;
        private void ValueChangeCheck()
        {
            PlayerController.instance.selectedAttack = null;
            switch (FightManager.instance.fightState)
            {
                //so exploration
                case FightManager.FightState.OutFight:
                {
                    foreach (PlayerAttackInstance attackInstance in PlayerController.instance.listOfPlayerAttackInstance)
                    {
                        if (attackInstance.attack.attackState == PlayerAttack.AttackState.Fm)
                        {
                            SelectingAttackPlayer(attackInstance);
                        }
                    }
                    break;
                }
                
                case FightManager.FightState.InFight:
                {
                    foreach (PlayerAttackInstance attackInstance in PlayerController.instance.listOfPlayerAttackInstance)
                    {
                        switch (selectedAM)
                        {
                            case false :
                                if (attackInstance.attack.attackState == PlayerAttack.AttackState.Fm)
                                {
                                    SelectingAttackPlayer(attackInstance);
                                }
                                break;
                            case true :
                                if (attackInstance.attack.attackState == PlayerAttack.AttackState.Am)
                                {
                                    SelectingAttackPlayer(attackInstance);
                                }
                                break;
                        }
                    }
                    break;
                }
            }

            UpdateFrequenceText();
        }

        private void SelectingAttackPlayer(PlayerAttackInstance _playerAttackInstance)
        {
            if (Mathf.Abs(sliderForFrequencyAttack.value - _playerAttackInstance.attack.indexFrequency)
                < epsilonForSliderAttack)
            {
                PlayerController.instance.selectedAttack = _playerAttackInstance;
                UpdateFrequenceText();
            }
        }
        
        private void UpdateFrequenceText()
        {
            if (PlayerController.instance == null || PlayerController.instance.selectedAttack == null)
            {
                descriptionAttackSelectedText.text = "";
                return;
            }
            descriptionAttackSelectedText.text = PlayerController.instance.selectedAttack.attack.name;
        }

        public void AmButton()
        {
            if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                selectedAM = true;
                ValueChangeCheck();
                return;
            }
            
            int cpt = 0;
            List<AbstractAI> newList = new List<AbstractAI>();
            Vector3 playerPos = PlayerController.instance.transform.position;
            bool isFacingLeft = PlayerController.instance.spriteRendererPlayer.flipX;

            foreach (AbstractAI enemy in listOfEveryEnemy.ToList())
            {
                if (cpt >= 3)break;
            
                Vector3 enemyPos = enemy.transform.position;
                float distance = Vector3.Distance(playerPos, enemyPos);
            
                bool isEnemyOnCorrectSide = (isFacingLeft && enemyPos.x < playerPos.x) || 
                                            (!isFacingLeft && enemyPos.x > playerPos.x);

                if (distance <= desiredDistanceAm && isEnemyOnCorrectSide)
                {
                    float probability = Mathf.Pow(1f - Mathf.Clamp01(distance / desiredDistanceAm), 0.5f);
                    if (Random.value <= probability)
                    {
                        newList.Add(enemy);
                        cpt++;
                    }
                }
            }

            listOfDetectedEnemy = newList;

            if (listOfDetectedEnemy.Count != 0)
            {
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Guessing;
                ChangeBoolSeenForAi();
                UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
            }
            else
            {
                Debug.Log("nobody detected");
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Exploration;
            }
        }

        private void ChangeBoolSeenForAi()
        {
            foreach (AbstractAI enemySeen in listOfDetectedEnemy)
            {
                enemySeen._abstractEntityDataInstance.seenByRadio = true;
            }
        }
    
        public void FmButton()
        {
            if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                selectedAM = false;
                ValueChangeCheck();
                return;
            }
            
            int cpt = 0;
            listOfDetectedEnemy.Clear();
            Vector3 playerPos = PlayerController.instance.transform.position;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerPos, desiredDistanceFm, enemyLayerMask);
            foreach (Collider2D  col in hitColliders)
            {
                if (cpt >= 3)break;
                AbstractAI enemy = col.GetComponent<AbstractAI>();
                if (enemy != null && !IsEnemyAlreadyInList(enemy))
                {
                    listOfDetectedEnemy.Add(enemy);
                    cpt++;
                }
            }

            if (listOfDetectedEnemy.Count != 0)
            {
                //UpdateRadioEnemyAfterDetection();
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Guessing;
                ChangeBoolSeenForAi();
                UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
            }
            else
            {
                Debug.Log("nobody detected");
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Exploration;
            }
        }
    
        private bool IsEnemyAlreadyInList(AbstractAI enemyToCheck)
        {
            foreach (AbstractAI existingEnemy in listOfDetectedEnemy)
            {
                if (existingEnemy == enemyToCheck)
                {
                    return true;
                }
            }
            return false;
        }

        private void InitializeRadioEnemy()
        {
            matRadioEnemy.SetFloat("_waves_Amount", 0);
            matRadioEnemy.SetFloat("_waves_Amp", 0);
        }
    
        public void UpdateRadioEnemyWithLight(int index)
        {
            if (listOfDetectedEnemy.Count - 1 < index || PlayerController.instance.currentPlayerExplorationState
                == PlayerController.PlayerStateExploration.Exploration)
            {
                InitializeRadioEnemy();
                return;
            }

            if (listOfDetectedEnemy[index]._abstractEntityDataInstance.reveal) // cant add it in the block on top because dependencies with a Singleton
            {
                InitializeRadioEnemy();
                return;
            }
        
            float waveAmp = listOfDetectedEnemy[index]._abstractEntityDataInstance.waveAmplitudeEnemy;
            float waveFre = listOfDetectedEnemy[index]._abstractEntityDataInstance.waveFrequency;
        
            matRadioEnemy.SetFloat("_waves_Amount", waveFre);
            matRadioEnemy.SetFloat("_waves_Amp", waveAmp);
        }
    
        void OnDrawGizmos()
        {
            if (PlayerController.instance == null) return;
            Transform player = PlayerController.instance.transform;
            Gizmos.color = Color.cyan;
            Gizmos.DrawLine(player.position, player.position + player.right * desiredDistanceAm); 
            Gizmos.DrawLine(player.position, player.position - player.right * desiredDistanceAm); 
        
            Gizmos.DrawWireSphere(PlayerController.instance.transform.position, desiredDistanceFm);
        }
    
    
    }
}
