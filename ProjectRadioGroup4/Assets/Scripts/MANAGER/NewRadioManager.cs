using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.NEW_AI;
using Controller;
using INTERACT;
using INTERFACE;
using MANAGER;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    public class NewRadioManager : MonoBehaviour
    {
        #region Singleton
        public static NewRadioManager instance;
        #endregion

        #region Fields
        [Header("Shader Material Player")]
        [SerializeField] private RawImage imageRadioPlayer;
        [SerializeField] private RawImage imageRadioEnemy;
        
        private Material matRadioPlayer;
        private Material matRadioEnemy;

        [Header("Sliders")]
        [SerializeField] private Slider sliderAmplitude;
        [SerializeField] private Slider sliderFrequency;
        //[SerializeField] private Slider sliderStep;

        [Header("Shader Property Limits")]
        [SerializeField] private float maxAmplitude = 0.4f;
        [SerializeField] private float maxFrequency = 1f;
        [SerializeField] private float maxStep = 1f;
        [SerializeField] private Color playerBaseColor;
        [SerializeField] private Color enemyBaseColor;
        
        [Header("Game Settings")]
        [SerializeField] private float checkInterval = 0.5f;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private float pulseDuration = 0.2f;
        
        [Header("Matching Thresholds")]
        [SerializeField] private float amplitudeThreshold = 0.05f;
        [SerializeField] private float frequencyThreshold = 1f; 
        [SerializeField] private float stepThreshold = 1f;
        
        private Coroutine currentTransition;

        [Header("UI")] public Canvas canvaRadio;
        [SerializeField] private TMP_Text chronoInFight;
        
        [Header("List")] public List<NewAi> listOfEveryEnemy;

        [Header("Light Settings")]
        [SerializeField] private Image[] lights = new Image[6]; 
        [SerializeField] private Color offColor = new Color(0.2f, 0.2f, 0.2f, 1f); 
        [SerializeField] private Color pendingColor = Color.red;
        [SerializeField] private Color currentColor = Color.yellow;
        [SerializeField] private Color completedColor = Color.green;
        
        [Header("Combat Light Settings")]
        [SerializeField] private Color correctColor = Color.green;
        [SerializeField] private Color wrongColor = Color.red;
        [SerializeField] private float wrongColorDuration = 0.5f;
        private Coroutine wrongColorCoroutine;

        private int currentActiveLight = 0;
        
        private float lastCheckTime;
        private bool isMatching;
        #endregion

        #region Properties
        public bool IsActive => isMatching;
        #endregion

        #region Unity Methods
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

            if (imageRadioPlayer != null && imageRadioEnemy != null)
            {
                matRadioPlayer = imageRadioPlayer.material;
                matRadioEnemy = imageRadioEnemy.material;
                playerBaseColor = matRadioPlayer.GetColor("_Color");
                enemyBaseColor = matRadioEnemy.GetColor("_Color");
            }
        }

        private void Update()
        {
            TimerCheckInterval();
            UpdateText(chronoInFight,FightManager.instance?.playerTurnTimer.ToString("00.00"));
        }

        private void Start()
        {
            InitializeSliders();
            ResetMaterials();
            InitializeLights();
        }
        #endregion

        #region Initialization
        private void InitializeSliders()
        {
            if (sliderAmplitude != null)
            {
                sliderAmplitude.maxValue = maxAmplitude;
                sliderAmplitude.onValueChanged.AddListener((value) => UpdateShaderParam("_waves_Amp", value));
            }

            if (sliderFrequency != null)
            {
                sliderFrequency.maxValue = maxFrequency;
                sliderFrequency.onValueChanged.AddListener((value) => UpdateShaderParam("_waves_Amount", value));
            }
            /*
            if (sliderStep != null)
            {
                sliderStep.maxValue = maxStep;
                sliderStep.onValueChanged.AddListener((value) => UpdateShaderParam("_Step", value));
            }
            */
        }
        #endregion

        #region Coroutines
        private IEnumerator StartMatchingRoutine(IWaveInteractable waveInteractable)
        {
            isMatching = true;
            yield return HandleRadioTransition(waveInteractable.GetCurrentWaveSettings());
        }

        private IEnumerator HandlePatternMatched(IWaveInteractable waveInteractable)
        {
            ActivateNextLight();
            
            isMatching = false;
    
            yield return PulseEffect();
    
            waveInteractable.MoveToNextPattern();

            if (!waveInteractable.HasRemainingPatterns())
            {
                var controller = NewPlayerController.instance;

                if (FightManager.instance?.fightState == FightManager.FightState.OutFight && controller != null)
                {
                    waveInteractable.Detected = true;
                    controller.currentPhase2ModuleState = NewPlayerController.Phase2Module.Off;
                    InitializeLights();
                    if (waveInteractable is NewAi ai)
                    {
                        ai.BeginFight();
                        yield break;
                    }

                    controller.canMove = true;
                    waveInteractable.CanSecondPhase = false;
                }
                yield return HandleRadioTransition(new WaveSettings(0,0,0));
            }
            else
            {
                yield return HandleRadioTransition(waveInteractable.GetCurrentWaveSettings());
                isMatching = true;
            }
        }

        
        
        public IEnumerator HandleRadioTransition(WaveSettings targetSettings)
        {
            float startFreq = matRadioEnemy.GetFloat("_waves_Amount");
            float startAmp = matRadioEnemy.GetFloat("_waves_Amp");
          //  float startStep = matRadioEnemy.GetFloat("_Step");

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;

                matRadioEnemy.SetFloat("_waves_Amount", 
                    Mathf.Lerp(startFreq, targetSettings.frequency, t));
                matRadioEnemy.SetFloat("_waves_Amp", 
                    Mathf.Lerp(startAmp, targetSettings.amplitude, t));
             //   matRadioEnemy.SetFloat("_Step", 
               //     Mathf.Lerp(startStep, targetSettings.step, t));

                yield return null;
            }

            ApplySettingsImmediate(matRadioEnemy, targetSettings);
        }

        private IEnumerator PulseEffect()
        {
            matRadioPlayer.SetColor("_Color", Color.white);
            yield return new WaitForSeconds(pulseDuration);
            matRadioPlayer.SetColor("_Color", playerBaseColor);
        }
        
        private IEnumerator StopMatchingRoutine()
        {
            isMatching = false;
            yield return HandleRadioTransition(new WaveSettings(0,0,0));
        }
        #endregion

        #region Game State Management
        
        
        public void StartMatchingGameOutFight()
        {
            if (NewPlayerController.instance.currentInteractableInRange is not
                    IWaveInteractable waveInteractable || !waveInteractable.CanBeActivated()) return;

            
            waveInteractable.Activate();
            InitializeLights(waveInteractable);
            
            if (waveInteractable is NewAi ai)
            {
                ai.attackTimer = ai.attackTriggerDelay;
                ai.isTimerRunning = true;
            }

            if (!waveInteractable.HasRemainingPatterns())
            {
                Debug.LogError("ICICICIICCII");
                return;
            }

            if (currentTransition != null)
                StopCoroutine(currentTransition);

            currentTransition = StartCoroutine(StartMatchingRoutine(waveInteractable));
        }

        public void StartMatchingGameInFight()
        {
            ResetLights();
            if (NewPlayerController.instance.currentInteractableInRange is not NewAi ai) 
            {
                return;
            }
            
            ai.Activate();
            InitializeLights(ai);
            if (!ai.HasRemainingPatterns()) return;
            
            if (currentTransition != null)
                StopCoroutine(currentTransition);
            
            currentTransition = StartCoroutine(StartMatchingRoutine(ai));
            if (lights.Length > 0)
                lights[0].color = currentColor;
        }

        public void StopMatchingGame()
        {
            ResetLights();
            if (currentTransition != null)
                StopCoroutine(currentTransition);
            
            if (NewPlayerController.instance != null)
            {
                NewPlayerController.instance.currentPhase2ModuleState = NewPlayerController.Phase2Module.Off;
                NewPlayerController.instance.canMove = true;
            }

            StartCoroutine(StopMatchingRoutine());
        }
        
        private void CheckWaveMatch()
        {
            var waveInteractable = NewPlayerController.instance.currentInteractableInRange as IWaveInteractable;
            var settings = waveInteractable?.GetCurrentWaveSettings();

            if (settings == null || !IsMatch(settings)) return;
            StartCoroutine(HandlePatternMatched(waveInteractable));
        }
        
        private bool IsMatch(WaveSettings settings)
        {
            float freqDiff = Mathf.Abs(matRadioPlayer.GetFloat("_waves_Amount") - settings.frequency);
            float ampDiff = Mathf.Abs(matRadioPlayer.GetFloat("_waves_Amp") - settings.amplitude);
           // float stepDiff = Mathf.Abs(matRadioPlayer.GetFloat("_Step") - settings.step);
        
           return freqDiff < frequencyThreshold
                  && ampDiff < amplitudeThreshold;
           // && stepDiff < stepThreshold;
        }
        #endregion

        #region Shader Management
        
        private void ApplySettingsImmediate(Material mat, WaveSettings settings)
        {
            mat.SetFloat("_waves_Amount", settings.frequency);
            mat.SetFloat("_waves_Amp", settings.amplitude);
           // mat.SetFloat("_Step", settings.step);
        }

        private void UpdateShaderParam(string param, float value)
        {
            matRadioPlayer?.SetFloat(param, value);
        }

        private void ResetMaterials()
        {
            SetOscillationTo0(matRadioPlayer,playerBaseColor);
            SetOscillationTo0(matRadioEnemy,enemyBaseColor);
        }

        private void SetOscillationTo0(Material mat,Color _color)
        {
            mat.SetFloat("_waves_Amount", 0);
            mat.SetFloat("_waves_Amp", 0);
            mat.SetFloat("_Step", 0);
            mat.SetColor("_Color", _color);
        }
        #endregion

        #region UI

        public void UpdateText(TMP_Text _targetText,string _targetInnerText)
        {
            _targetText.text = _targetInnerText;
        }
        
        private void InitializeLights(IInteractable target)
        {
            foreach (var t in lights)
            {
                t.color = offColor;
            }
            currentActiveLight = 0;
            if (target is not BatteryInteract bat) return;
            for (int i = 0; i < bat.wavePatterns.Count; i++)
            {
                lights[i].color = pendingColor;
            }
        }

        private void InitializeLights()
        {
            foreach (var t in lights)
            {
                t.color = offColor;
            }

            currentActiveLight = 0;
        }

        private void ActivateNextLight()
        {
            if (currentActiveLight >= lights.Length) return;

            lights[currentActiveLight].color = completedColor;
    
            if (currentActiveLight + 1 < lights.Length)
            {
                lights[currentActiveLight + 1].color = currentColor;
            }
    
            currentActiveLight++;
        }

        public void ResetLights()
        {
            InitializeLights();
            foreach (var light in lights)
            {
                light.transform.localScale = Vector3.one;
            }
        }
        
        #endregion
        
        #region Combat Light Management

        public void InitializeCombatLights(int sequenceLength)
        {
            ResetLights();
            
            for (int i = 0; i < sequenceLength && i < lights.Length; i++)
            {
                lights[i].color = pendingColor; 
            }
        }

        public void UpdateCombatLight(int index, bool isCorrect)
        {
            if (index < 0 || index >= lights.Length) return;
    
            lights[index].color = isCorrect ? completedColor : pendingColor;
            if (isCorrect)
            {
                StartCoroutine(PulseLight(lights[index]));
            }
        }

        private IEnumerator PulseLight(Image light)
        {
            float duration = 0.2f;
            Vector3 originalScale = light.transform.localScale;
            Vector3 targetScale = originalScale * 1.2f;
    
            float elapsed = 0f;
            while (elapsed < duration)
            {
                light.transform.localScale = Vector3.Lerp(originalScale, targetScale, elapsed/duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
    
            elapsed = 0f;
            while (elapsed < duration)
            {
                light.transform.localScale = Vector3.Lerp(targetScale, originalScale, elapsed/duration);
                elapsed += Time.deltaTime;
                yield return null;
            }
    
            light.transform.localScale = originalScale;
        }

        #endregion

        #region Time Related

        private void TimerCheckInterval()
        {
            if (!isMatching) return;
    
            if (Time.time - lastCheckTime > checkInterval)
            {
                CheckWaveMatch();
                lastCheckTime = Time.time;
            }
        }

        #endregion

        #region Deal With Fight Oscillation

        [SerializeField] private float fromFrequencyToWaveNumber;
        public void UpdateOscillationEnemy(NewAi ai)
        {
            var targetFrequency = ai.GetActualInstanceChips().index * fromFrequencyToWaveNumber;
            WaveSettings enemyChipsWaveSettings = new WaveSettings(targetFrequency, 0.3f, 0);
            Debug.LogError(ai.chipsDatasList[0].index);
            StartCoroutine(HandleRadioTransition(enemyChipsWaveSettings));


        }

        #endregion
    }
}