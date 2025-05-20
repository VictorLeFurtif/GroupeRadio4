using System;
using System.Collections;
using System.Collections.Generic;
using AI.NEW_AI;
using Controller;
using INTERACT;
using INTERFACE;
using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    public class NewRadioManager : MonoBehaviour
    {
        [Header("Shader Material Player")]
        [SerializeField] private RawImage imageRadioPlayer;
        [SerializeField] private RawImage imageRadioEnemy;
        
        private Material matRadioPlayer;
        private Material matRadioEnemy;

        [Header("Sliders")]
        [SerializeField] private Slider sliderAmplitude;
        [SerializeField] private Slider sliderFrequency;
        [SerializeField] private Slider sliderStep;

        [Header("Shader Property Limits")]
        [SerializeField] private float maxAmplitude = 0.4f;
        [SerializeField] private float maxFrequency = 1f;
        [SerializeField] private float maxStep = 1f;
        [SerializeField] private Color playerBaseColor;
        [SerializeField] private Color enemyBaseColor;
        
        [Header("Game Settings")]
        [SerializeField] private float matchThreshold = 0.1f;
        [SerializeField] private float checkInterval = 0.5f;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private float pulseDuration = 0.2f;
        
        private Coroutine currentTransition;

        public static NewRadioManager instance;
        
        [Header("Canvas")] public Canvas canvaRadio;
        
        [Header("List")] public List<NewAi> listOfEveryEnemy ;
        
        
        
        private float lastCheckTime;
        private bool isMatching;

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
        
        public bool IsActive => isMatching;

        private void Update()
        {
            if (!isMatching) return;
    
            if (Time.time - lastCheckTime > checkInterval)
            {
                CheckWaveMatch();
                lastCheckTime = Time.time;
            }
            
        }

        private void Start()
        {
            InitializeSliders();
            ResetMaterials();
        }

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

            if (sliderStep != null)
            {
                sliderStep.maxValue = maxStep;
                sliderStep.onValueChanged.AddListener((value) => UpdateShaderParam("_Step", value));
            }
        }
        

        #region Coroutine

        private IEnumerator StartMatchingRoutine(IWaveInteractable waveInteractable)
        {
            isMatching = true;
            yield return HandleRadioTransition(waveInteractable.GetCurrentWaveSettings());
        }

        private IEnumerator HandlePatternMatched(IWaveInteractable waveInteractable)
        {
            isMatching = false;
            
            yield return PulseEffect();
            
            waveInteractable.MoveToNextPattern();

            if (!waveInteractable.HasRemainingPatterns()) //CEST WIN ICI ATTENTION
            {
                waveInteractable.MarkAsUsed();
                waveInteractable.Detected = true;
                var controller = NewPlayerController.instance;
                if (controller != null) controller.canMove = true;
                yield return HandleRadioTransition(new WaveSettings(0,0,0)); 
                yield break;
            }
            
            yield return HandleRadioTransition(waveInteractable.GetCurrentWaveSettings());
            isMatching = true;
        }
        
        
        private IEnumerator HandleRadioTransition(WaveSettings targetSettings)
        {
            float startFreq = matRadioEnemy.GetFloat("_waves_Amount");
            float startAmp = matRadioEnemy.GetFloat("_waves_Amp");
            float startStep = matRadioEnemy.GetFloat("_Step");

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;

                matRadioEnemy.SetFloat("_waves_Amount", 
                    Mathf.Lerp(startFreq, targetSettings.frequency, t));
                matRadioEnemy.SetFloat("_waves_Amp", 
                    Mathf.Lerp(startAmp, targetSettings.amplitude, t));
                matRadioEnemy.SetFloat("_Step", 
                    Mathf.Lerp(startStep, targetSettings.step, t));

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
            yield return HandleRadioTransition(new WaveSettings(0,0,0)); // J le reset a 0
        }

        #endregion

        #region DealWithGameState
        
        public void StartMatchingGame()
        {
            var waveInteractable = NewPlayerController.instance.currentInteractableInRange as IWaveInteractable;
            if (waveInteractable == null || !waveInteractable.CanBeActivated()) return;

            waveInteractable.Activate();
    
            if (!waveInteractable.HasRemainingPatterns()) return;

            if (currentTransition != null)
                StopCoroutine(currentTransition);

            currentTransition = StartCoroutine(StartMatchingRoutine(waveInteractable));
        }

        public void StopMatchingGame()
        {
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
            if (settings == null) return;

            if (IsMatch(settings))
            {
                if (currentTransition != null)
                    StopCoroutine(currentTransition);
                    
                currentTransition = StartCoroutine(HandlePatternMatched(waveInteractable));
            }
        }
        
        private bool IsMatch(WaveSettings settings)
        {
            float freqDiff = Mathf.Abs(matRadioPlayer.GetFloat("_waves_Amount") - settings.frequency);
            float ampDiff = Mathf.Abs(matRadioPlayer.GetFloat("_waves_Amp") - settings.amplitude);
            float stepDiff = Mathf.Abs(matRadioPlayer.GetFloat("_Step") - settings.step);

            return freqDiff < matchThreshold && ampDiff < matchThreshold && stepDiff < matchThreshold;
        }

        #endregion

        #region DealWithShader
        
        private void ApplySettingsImmediate(Material mat, WaveSettings settings)
        {
            mat.SetFloat("_waves_Amount", settings.frequency);
            mat.SetFloat("_waves_Amp", settings.amplitude);
            mat.SetFloat("_Step", settings.step);
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
        

        

        
    }
}