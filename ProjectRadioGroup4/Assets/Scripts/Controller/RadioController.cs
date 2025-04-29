using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using DATA.Script.Attack_Data;
using DATA.Script.Entity_Data.AI;
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
        [Header("Canvas")] public Canvas canvaRadio;

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
        
        public Slider sliderOscillationPlayer;

        [Header("List of enemies detected"), SerializeField]
        public List<AbstractAI> listOfDetectedEnemy;

        [Header("Radio's parameter AM")] [SerializeField]
        private float desiredDistanceAm;

        [SerializeField] private float desiredDistanceFm;

        [Header("Layer Mask"), SerializeField] private LayerMask enemyLayerMask;

        [FormerlySerializedAs("selectedAM")] public bool selectedAm = false;
        
        [SerializeField] private TMP_Text descriptionEffectSelectedText;

        [SerializeField]
        private Image backgroundSliderFrequency;

        [Header("Color")] [Tooltip("Sélectione la couleur pour Attack/Effet")] [SerializeField] private Color colorSliderAttackAm;
        [SerializeField] private Color colorSliderAttackFm;

        [Header("Ampoule Manager")]
        public AmpouleManager ampouleManager;


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
            sliderForFrequencyAttack.onValueChanged.AddListener(delegate { ValueChangeCheck(); EffectInExploration();});
            InitializeRadioEnemy();
        
            //Intéressant au Start de placer la light sur l'élément 0. Pas plus de justification c'est moi qui décide
            UpdateRadioEnemyWithLight(0);
            InitializeSliderOscillationPlayer();
            
            UpdateAmplitude(0);
            InitializeTextEffectFm();
        }

        private void InitializeTextEffectFm()
        {
            if (PlayerController.instance == null) return;
            PlayerController.instance.selectedAttackEffect = null;
            UpdateEffectFMText(PlayerController.instance.selectedAttackEffect);
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

        private void EffectInExploration()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.lampTorch.intensity = 0;
                PlayerController.instance.gameObject.layer = 6;
            }
            
            if (PlayerController.instance.selectedAttack != null && FightManager.instance.fightState != FightManager.FightState.InFight)
            {
                PlayerController.instance.selectedAttack.ProcessAttackEffect();
            }
        }
        
        private void InitializeSliderFrequency()
        {
            sliderForFrequencyAttack.maxValue = maxValueSliderFrequencyAttack;
            backgroundSliderFrequency.color = colorSliderAttackFm;
            ValueChangeCheck();
        }
        
        private void InitializeSliderOscillationPlayer() //wave amp comprit entre 0 et 0.4
        {
            sliderOscillationPlayer.maxValue = 0.4f;
            sliderOscillationPlayer.onValueChanged.AddListener(UpdateAmplitude);
        }
       
        private void UpdateAmplitude(float newValue)
        {
            matRadioPlayer.SetFloat("_waves_Amp", newValue);
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
                        switch (selectedAm)
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
        
        public void SelectEffectFMButton()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.uxSound.click);
            if (FightManager.instance.fightState != FightManager.FightState.InFight ||
                PlayerController.instance == null)
            {
                return;
            }
            
            PlayerController.instance.selectedAttackEffect = null;
            
            foreach (PlayerAttackInstance attackInstance in PlayerController.instance.listOfPlayerAttackInstance)
            {
                if (attackInstance.attack.attackState != PlayerAttack.AttackState.Fm) continue;
                if (!(Mathf.Abs(sliderForFrequencyAttack.value - attackInstance.attack.indexFrequency) <
                      epsilonForSliderAttack)) continue;
                PlayerController.instance.selectedAttackEffect = attackInstance;
                
                if (TutorialFightManager.instance != null && TutorialFightManager.instance.isInTutorialCombat &&
                    TutorialFightManager.instance.currentStep == CombatTutorialStep.ExplainLockFM)
                {
                    TutorialFightManager.instance.AdvanceStep();
                }
                
                break;
            }
            UpdateEffectFMText(PlayerController.instance.selectedAttackEffect);
        }
        
        private void UpdateEffectFMText(PlayerAttackInstance effectInstance)
        {
            descriptionEffectSelectedText.text = effectInstance == null ?
                "No Effect selected" : $"Effet select : {effectInstance.attack.name}";
        }

        private void UpdateFrequenceText()
        {
            string amText = PlayerController.instance.selectedAttack != null
                ? $"Attaque or Effect : {PlayerController.instance.selectedAttack.attack.name}"
                : "Attaque or Effect : aucune";

            if (PlayerController.instance.selectedAttack != null && PlayerController.instance.selectedAttack.attack.attackState == PlayerAttack.AttackState.Am)
            {
                amText += $" Damage flat : {PlayerController.instance.selectedAttack.attack.damage}";
            }
            
            descriptionAttackSelectedText.text = amText;
        }

        
        private void SelectingAttackPlayer(PlayerAttackInstance _playerAttackInstance)
        {
            if (!(Mathf.Abs(sliderForFrequencyAttack.value - _playerAttackInstance.attack.indexFrequency)
                  < epsilonForSliderAttack)) return;
            PlayerController.instance.selectedAttack = _playerAttackInstance;
            UpdateFrequenceText();
        }

        public void AmButton()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.uxSound.click);
            
            if (PlayerController.instance == null)
            {
                return;
            }
            
            if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                selectedAm = true;
                backgroundSliderFrequency.color = colorSliderAttackAm;
                ValueChangeCheck();
                return;
            }
            
            PlayerController.instance.animatorPlayer.Play("scanPlayerFront");
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.ScanFast);
            
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

            AmFmActionIfListNotEmpty();
        }

       
        
        private void ChangeBoolSeenForAi()
        {
            foreach (AbstractAI enemySeen in listOfDetectedEnemy)
            {
                enemySeen._abstractEntityDataInstance.seenByRadio = true;
            }
        }

        private void AmFmActionIfListNotEmpty()
        {
            if (listOfDetectedEnemy.Count != 0)
            {
                listOfDetectedEnemy.Sort((x, y) =>
                    Vector3.Distance(PlayerController.instance.transform.position, x._abstractEntityDataInstance.entity.transform.position)
                        .CompareTo(
                            Vector3.Distance(PlayerController.instance.transform.position, y._abstractEntityDataInstance.entity.transform.position)));
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Guessing;
                ChangeBoolSeenForAi();
                UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);
            }
            else
            {
                Debug.Log("nobody detected");
                UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);   
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Exploration;
            }
        }
    
        public void FmButton()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.uxSound.click);
            
            if (PlayerController.instance == null)
            {
                return;
            }
            
            if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                selectedAm = false;
                backgroundSliderFrequency.color = colorSliderAttackFm;
                ValueChangeCheck();
                return;
            }
            
            PlayerController.instance.animatorPlayer.Play("ScanAround");
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.ScanSlow);
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

            AmFmActionIfListNotEmpty();
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
    
        private Coroutine transitionCoroutine;

        public void UpdateRadioEnemyWithLight(int index)
        {

            if (transitionCoroutine != null)
                StopCoroutine(transitionCoroutine);

            if (FightManager.instance.fightState == FightManager.FightState.OutFight)
            {
                if (listOfDetectedEnemy.Count - 1 < index || listOfDetectedEnemy[index]._abstractEntityDataInstance.reveal)
                {
                    transitionCoroutine = StartCoroutine(SmoothTransitionRadio(0f, 0f));
                    return;
                }

                var enemy = listOfDetectedEnemy[index]._abstractEntityDataInstance;
                transitionCoroutine = StartCoroutine(SmoothTransitionRadio(enemy.waveFrequency, enemy.waveAmplitudeEnemy));
            }
            else if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                if (FightManager.instance.listOfJustEnemiesAlive.Count - 1 < index)
                {
                    transitionCoroutine = StartCoroutine(SmoothTransitionRadio(0f, 0f));
                    return;
                }

                var enemy = FightManager.instance.listOfJustEnemiesAlive[index];
                float amp = (enemy.hp / enemy.maxHp) * 0.4f;
                float freq = enemy.waveFrequency;
                transitionCoroutine = StartCoroutine(SmoothTransitionRadio(freq, amp));
            }
        }


        [SerializeField]
        private float durationTimeLerpRadio;
        private IEnumerator SmoothTransitionRadio(float targetFreq, float targetAmp)
        {
            float elapsed = 0f;

            float startFreq = matRadioEnemy.GetFloat("_waves_Amount");
            float startAmp = matRadioEnemy.GetFloat("_waves_Amp");

            while (elapsed < durationTimeLerpRadio)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / durationTimeLerpRadio;

                float currentFreq = Mathf.Lerp(startFreq, targetFreq, t);
                float currentAmp = Mathf.Lerp(startAmp, targetAmp, t);

                matRadioEnemy.SetFloat("_waves_Amount", currentFreq);
                matRadioEnemy.SetFloat("_waves_Amp", currentAmp);

                yield return null;
            }

            matRadioEnemy.SetFloat("_waves_Amount", targetFreq);
            matRadioEnemy.SetFloat("_waves_Amp", targetAmp);
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
    
        private void SliderOscillationPlayerBehavior() // now useless
        {
            if (PlayerController.instance.currentPlayerExplorationState == PlayerController.PlayerStateExploration.Guessing)
            {
                sliderOscillationPlayer.interactable = true;
            }
            else
            {
                sliderOscillationPlayer.interactable = false;
            }
        }

        public void SelectEnemyByLight()
        {
            if (PlayerController.instance == null || FightManager.instance == null || 
                FightManager.instance.fightState == FightManager.FightState.OutFight || AmpouleManager.ampouleAllumee 
                >  FightManager.instance.listOfJustEnemiesAlive.Count - 1)
            {
             return;
            }
            PlayerController.instance.selectedEnemy =
                FightManager.instance.listOfJustEnemiesAlive[AmpouleManager.ampouleAllumee].entity;
        }
    }
}