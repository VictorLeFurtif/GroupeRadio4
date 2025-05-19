using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI;
using AI.OLD_AI_BEFORE_FP_V2;
using DATA.Script.Attack_Data.New_System_Attack_Player;
using INTERFACE;
using MANAGER;
using TMPro;
using UI.Link_To_Radio;
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

        [Header("Sprites")] 
        [Tooltip("Sélectionne le sprite pour Attack/Effet")]
        [SerializeField] private Sprite spriteSliderAttackAm;

        [SerializeField] private Sprite spriteSliderAttackFm;


        [Header("Manager & Controller")]
        public AmpouleManager ampouleManager;

        public bool canTakeBattery = false;

        


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
            sliderForFrequencyAttack.onValueChanged.AddListener(delegate { ValueChangeCheck();});
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
        

        private void EffectInExploration()
        {
            if (PlayerController.instance != null)
            {
                PlayerController.instance.lampTorch.intensity = 0;
                PlayerController.instance.gameObject.layer = 6;
            }
            
        }
        
        private void InitializeSliderFrequency()
        {
            sliderForFrequencyAttack.maxValue = maxValueSliderFrequencyAttack;
            backgroundSliderFrequency.sprite = spriteSliderAttackFm;
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
            
            switch (FightManager.instance.fightState)
            {
                case FightManager.FightState.OutFight:
                    
                    if (PlayerController.instance.selectedAttack is IPLayerEffect)
                    {
                        DisableActiveEffect();
                        canTakeBattery = false;
                    }
                    
                    PlayerController.instance.selectedAttack = null;
                    
                    foreach (var attackInstance in PlayerController.instance.listOfPlayerAttackInstance)
                    {
                        if (attackInstance.attack.attackState == PlayerAttackAbstract.AttackState.Fm)
                        {
                            SelectingAttackPlayer(attackInstance);
                        }
                    }
                    break;
            
                case FightManager.FightState.InFight:
                    if (selectedAm)
                    {
                        PlayerController.instance.selectedAttack = null;
                        foreach (var attack in PlayerController.instance.listOfPlayerAttackInstance)
                        {
                            if (attack.attack.attackState == PlayerAttackAbstract.AttackState.Am && 
                                Mathf.Abs(sliderForFrequencyAttack.value - attack.attack.indexFrequency) < epsilonForSliderAttack)
                            {
                                PlayerController.instance.selectedAttack = attack;
                                break;
                            }
                        }
                    }
                    else
                    {
                        PlayerController.instance.selectedAttackEffect = null;
                        foreach (var attack in PlayerController.instance.listOfPlayerAttackInstance)
                        {
                            if (attack.attack.attackState == PlayerAttackAbstract.AttackState.Fm && 
                                Mathf.Abs(sliderForFrequencyAttack.value - attack.attack.indexFrequency) < epsilonForSliderAttack)
                            {
                                PlayerController.instance.selectedAttackEffect = attack;
                                break;
                            }
                        }
                    }
                    break;
            }
    
            UpdateFrequenceText();
            UpdateEffectFMText(PlayerController.instance.selectedAttackEffect);
        }
        
        private void DisableActiveEffect()
        {
            PlayerController.instance._inGameData.classicEcho = false;
            PlayerController.instance.lampTorch.intensity = 0;
            PlayerController.instance.gameObject.layer = 6;
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
            
            foreach (PlayerAttackAbstractInstance attackInstance in PlayerController.instance.listOfPlayerAttackInstance)
            {
                if (attackInstance.attack.attackState != PlayerAttackAbstract.AttackState.Fm) continue;
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
        
        public void UpdateFrequenceText()
        {
            string attackText = PlayerController.instance.selectedAttack != null
                ? $"Attaque: {PlayerController.instance.selectedAttack.attack.name}"
                : "Aucune attaque";
    
            //USELESS NOW FOR V2 FP
            /*
            if (PlayerController.instance.selectedAttack != null && 
                PlayerController.instance.selectedAttack.attack.attackState == PlayerAttackAbstract.AttackState.Am)
            {
                attackText += $"\nDégâts: {PlayerController.instance.selectedAttack.attack.damage}";
            }*/
    
            descriptionAttackSelectedText.text = attackText;
        }

        public void UpdateEffectFMText(PlayerAttackAbstractInstance effectInstance)
        {
            string effectText = effectInstance != null
                ? $"Effet: {effectInstance.attack.name}"
                : "Aucun effet";
    
            descriptionEffectSelectedText.text = effectText;
        }

        
        private void SelectingAttackPlayer(PlayerAttackAbstractInstance _playerAttackInstance)
        {
            if (!(Mathf.Abs(sliderForFrequencyAttack.value - _playerAttackInstance.attack.indexFrequency)
                  < epsilonForSliderAttack)) return;
            PlayerController.instance.selectedAttack = _playerAttackInstance;
            UpdateFrequenceText();
        }

        public void AmButton()
        {
            if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                var previousEffect = PlayerController.instance.selectedAttackEffect;
                
                selectedAm = true;
                backgroundSliderFrequency.sprite = spriteSliderAttackAm;
                
                PlayerController.instance.selectedAttack = null;
                foreach (var attack in PlayerController.instance.listOfPlayerAttackInstance)
                {
                    if (attack.attack.attackState == PlayerAttackAbstract.AttackState.Am && 
                        Mathf.Abs(sliderForFrequencyAttack.value - attack.attack.indexFrequency) < epsilonForSliderAttack)
                    {
                        PlayerController.instance.selectedAttack = attack;
                        break;
                    }
                }
                
                PlayerController.instance.selectedAttackEffect = previousEffect;
        
                UpdateFrequenceText();
                UpdateEffectFMText(PlayerController.instance.selectedAttackEffect);
                return;
            }
            
            PlayerController.instance.animatorPlayer.Play("ScanAround");
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.ScanFast);

            int cpt = 0;
            List<AbstractAI> newList = new List<AbstractAI>();
            Vector3 playerPos = PlayerController.instance.transform.position;
            bool isFacingLeft = PlayerController.instance.spriteRendererPlayer.flipX;

            foreach (AbstractAI enemy in listOfEveryEnemy.ToList())
            {
                if (cpt >= 3) break;

                Vector3 enemyPos = enemy.transform.position;
                float distance = Vector3.Distance(playerPos, enemyPos);

                bool isEnemyOnCorrectSide = (isFacingLeft && enemyPos.x < playerPos.x) || (!isFacingLeft && enemyPos.x > playerPos.x);

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
                CallBackFeedBackPlayer.Instance.ShowMessage($"{listOfDetectedEnemy.Count} enemies found");
            }
            else
            {
                Debug.Log("nobody detected");
                CallBackFeedBackPlayer.Instance.ShowMessage("Nobody detected");
                UpdateRadioEnemyWithLight(AmpouleManager.ampouleAllumee);   
                PlayerController.instance.currentPlayerExplorationState = PlayerController.PlayerStateExploration.Exploration;
            }
        }
    
        public void FmButton()
        {
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.uxSound.click);

            if (PlayerController.instance == null)
                return;

            if (FightManager.instance.fightState == FightManager.FightState.InFight)
            {
                var previousAttack = PlayerController.instance.selectedAttack;
                selectedAm = false;
                backgroundSliderFrequency.sprite = spriteSliderAttackFm;
                
                PlayerController.instance.selectedAttackEffect = null;
                foreach (var attack in PlayerController.instance.listOfPlayerAttackInstance)
                {
                    if (attack.attack.attackState == PlayerAttackAbstract.AttackState.Fm && 
                        Mathf.Abs(sliderForFrequencyAttack.value - attack.attack.indexFrequency) < epsilonForSliderAttack)
                    {
                        PlayerController.instance.selectedAttackEffect = attack;
                        break;
                    }
                }
                
                PlayerController.instance.selectedAttack = previousAttack;
        
                UpdateFrequenceText();
                UpdateEffectFMText(PlayerController.instance.selectedAttackEffect);
                return;
            }
            
            if (FightManager.instance.fightState == FightManager.FightState.OutFight)
            {
                var selectedEffect = PlayerController.instance.selectedAttack;
                if (selectedEffect is IPLayerEffect effect)
                {
                    effect.ProcessEffect();
                    CallBackFeedBackPlayer.Instance.ShowMessage("Effet déclenché !");
                    canTakeBattery = true;
                }
                else
                {
                    CallBackFeedBackPlayer.Instance.ShowMessage("Aucun effet sélectionné.");
                }
            }

            #region Useless FM

            //USELESS FOR NOW AFTER V2 FP
            /*
            PlayerController.instance.animatorPlayer.Play("ScanAround");
            SoundManager.instance?.PlayMusicOneShot(SoundManager.instance.soundBankData.avatarSound.ScanSlow);

            int cpt = 0;
            listOfDetectedEnemy.Clear();
            Vector3 playerPos = PlayerController.instance.transform.position;
            Collider2D[] hitColliders = Physics2D.OverlapCircleAll(playerPos, desiredDistanceFm, enemyLayerMask);

            foreach (Collider2D col in hitColliders)
            {
                if (cpt >= 3) break;

                AbstractAI enemy = col.GetComponent<AbstractAI>();
                if (enemy != null && !IsEnemyAlreadyInList(enemy))
                {
                    listOfDetectedEnemy.Add(enemy);
                    cpt++;
                }
            }

            AmFmActionIfListNotEmpty();*/

            #endregion
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