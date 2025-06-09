using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using AI.NEW_AI;
using Controller;
using DG.Tweening;
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
        [SerializeField] private GameObject playerOscillationHolder;
        
        private Material matRadioPlayer;
        private Material matRadioEnemy;

        [Header("Sliders")]
        [SerializeField] private Slider sliderAmplitude;
        [SerializeField] private Slider sliderFrequency;

        [Header("Shader Property Limits")]
        [SerializeField] private float maxAmplitude = 0.4f;
        [SerializeField] private float maxFrequency = 1f;
        [SerializeField] private Color playerBaseColor;
        [SerializeField] private Color enemyBaseColor;
        
        [Header("Game Settings")]
        [SerializeField] private float checkInterval = 0.5f;
        [SerializeField] private float transitionDuration = 1f;
        [SerializeField] private float pulseDuration = 0.2f;
        
        [Header("Matching Thresholds")]
        [SerializeField] private float amplitudeThreshold = 0.05f;
        [SerializeField] private float frequencyThreshold = 1f;
        
        private Coroutine currentTransition;

        [Header("UI")] 
        public Canvas canvaRadio;
        [SerializeField] private TMP_Text chronoInFight;
        
        [Header("List")] 
        public List<NewAi> listOfEveryEnemy;

        [Header("Light Settings")]
        [SerializeField] private Image[] lightImages = new Image[6]; 
        [SerializeField] private Sprite offSprite;
        [SerializeField] private Sprite pendingSprite;
        [SerializeField] private Sprite currentSprite;
        [SerializeField] private Sprite completedSprite;
        [SerializeField] private Sprite correctSprite;
        [SerializeField] private Sprite wrongSprite;
        
        private int currentActiveLight = 0;
        private float lastCheckTime;
        public bool isMatching;

        [SerializeField] private BrasSexController brasSexController;

        [Header("TEXT UI SWAP")] 
        [SerializeField] private TMP_Text textSwap;

        private GameObject matchingSound;
        
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
            UpdateTextSwap();
        }

        private void Start()
        {
            InitializeSliders();
            ResetMaterials();
            InitializeLights();
            StartCoroutine(RadioBehaviorDependingFightState());
            UpdateTypeOfUiByFightState();
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
        }

        private void InitializeLights()
        {
            foreach (var light in lightImages)
            {
                if (light != null)
                {
                    light.sprite = offSprite;
                }
            }
            currentActiveLight = 0;
        }

        private void InitializeLights(IInteractable target)
        {
            InitializeLights();
            
            if (target is not BatteryInteract bat) return;
            
            for (int i = 0; i < bat.wavePatterns.Count && i < lightImages.Length; i++)
            {
                if (lightImages[i] != null)
                {
                    lightImages[i].sprite = pendingSprite;
                }
            }
        }
        #endregion

        #region Light Management
        private void ActivateNextLight()
        {
            if (currentActiveLight >= lightImages.Length) return;
            
            if (lightImages[currentActiveLight] != null)
            {
                lightImages[currentActiveLight].sprite = completedSprite;
                StartCoroutine(PulseLight(lightImages[currentActiveLight]));
            }
            
            if (currentActiveLight + 1 < lightImages.Length && lightImages[currentActiveLight + 1] != null)
            {
                lightImages[currentActiveLight + 1].sprite = currentSprite;
            }

            currentActiveLight++;
        }

        private IEnumerator PulseLight(Image light)
        {
            if (light == null) yield break;

            Vector3 originalScale = light.transform.localScale;
            Color originalColor = light.color;
            Sprite originalSprite = light.sprite;

            GameObject flashObj = new GameObject("FlashEffect", typeof(Image));
            Image flashEffect = flashObj.GetComponent<Image>();
            flashEffect.transform.SetParent(light.transform, false);
            flashEffect.rectTransform.anchorMin = Vector2.zero;
            flashEffect.rectTransform.anchorMax = Vector2.one;
            flashEffect.rectTransform.offsetMin = Vector2.zero;
            flashEffect.rectTransform.offsetMax = Vector2.zero;
            flashEffect.color = new Color(1, 1, 1, 0);
            flashEffect.sprite = light.sprite;

            Sequence successSequence = DOTween.Sequence();

            successSequence.Append(light.transform.DOPunchScale(Vector3.one * 0.3f, 0.4f, 10, 0.5f));

            successSequence.Join(flashEffect.DOFade(0.9f, 0.1f).SetEase(Ease.OutQuad));
            successSequence.Append(flashEffect.DOFade(0f, 0.2f).SetEase(Ease.InQuad));
            
            successSequence.Join(light.transform.DOScale(1.1f, 0.3f).SetLoops(2, LoopType.Yoyo));

            successSequence.Append(light.transform.DOScale(originalScale, 0.2f));

            yield return successSequence.WaitForCompletion();

            Destroy(flashObj);
        }

        public void ResetLights()
        {
            InitializeLights();
            foreach (var light in lightImages)
            {
                if (light != null)
                {
                    light.transform.localScale = Vector3.one;
                }
            }
        }

        public void InitializeCombatLights(int sequenceLength)
        {
            ResetLights();
            
            for (int i = 0; i < sequenceLength && i < lightImages.Length; i++)
            {
                if (lightImages[i] != null)
                {
                    lightImages[i].sprite = pendingSprite;
                }
            }
        }

        public void UpdateCombatLight(int index, bool isCorrect)
        {
            if (index < 0 || index >= lightImages.Length || lightImages[index] == null) return;
    
            lightImages[index].sprite = isCorrect ? completedSprite : wrongSprite;
            
            if (isCorrect)
            {
                StartCoroutine(PulseLight(lightImages[index]));
            }
        }
        
        public IEnumerator GoldenRunLightCelebration()
        {
            if (lightImages == null || lightImages.Length == 0) yield break;

            Sprite[] originalSprites = new Sprite[lightImages.Length];
            for (int i = 0; i < lightImages.Length; i++)
            {
                if (lightImages[i] != null)
                {
                    originalSprites[i] = lightImages[i].sprite;
                    lightImages[i].sprite = offSprite;
                }
            }

            foreach (var t in lightImages)
            {
                if (t != null)
                {
                    t.sprite = currentSprite;
                    StartCoroutine(PulseLight(t));
                    yield return new WaitForSeconds(0.2f);
                }
            }

            yield return new WaitForSeconds(0.2f);

            for (int i = 0; i < lightImages.Length; i++)
            {
                if (lightImages[i] != null)
                {
                    lightImages[i].sprite = originalSprites[i];
                }
            }
            ResetLights();
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
            SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.validation);
            
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
                        ai.StartFight();
                        ai.isTimerRunning = false;
                        yield break;
                    }
                    if (matchingSound != null)
                    {
                        matchingSound.SetActive(false);
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

            float elapsed = 0f;

            while (elapsed < transitionDuration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / transitionDuration;

                matRadioEnemy.SetFloat("_waves_Amount", 
                    Mathf.Lerp(startFreq, targetSettings.frequency, t));
                matRadioEnemy.SetFloat("_waves_Amp", 
                    Mathf.Lerp(startAmp, targetSettings.amplitude, t));

                yield return null;
            }

            ApplySettingsImmediate(matRadioEnemy, targetSettings);
        }

        
        private IEnumerator PulseEffect()
        {
            float elapsedTime = 0f;
            Color startColor = playerBaseColor;
            Color targetColor = Color.magenta;
            
            while (elapsedTime < pulseDuration)
            {
                elapsedTime += Time.deltaTime;
                float t = Mathf.Clamp01(elapsedTime / pulseDuration);
        
                float pulseValue = Mathf.Sin(t * Mathf.PI); 
                matRadioPlayer.SetColor("_Color", Color.Lerp(startColor, targetColor, pulseValue));
        
                yield return null;
            }
    
            matRadioPlayer.SetColor("_Color", playerBaseColor);
        }
        
        private IEnumerator StopMatchingRoutine()
        {
            isMatching = false;
            if (matchingSound != null)
            {
                matchingSound.SetActive(false);
            }
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
                return;
            }

            if (currentTransition != null)
                StopCoroutine(currentTransition);

            if (matchingSound == null)
            {
                matchingSound = SoundManager.instance?.InitialisationAudioObjectDestroyAtEnd(
                    SoundManager.instance.soundBankData.enemySound.bruitRadioMatch, true, true, 1f, "EnemyBreath");
            }
            else
            {
                matchingSound.SetActive(true);
            }
            
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
            if (lightImages.Length > 0 && lightImages[0] != null)
                lightImages[0].sprite = currentSprite;
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
        
            return freqDiff < frequencyThreshold
                   && ampDiff < amplitudeThreshold;
        }
        #endregion

        #region Shader Management
        private void ApplySettingsImmediate(Material mat, WaveSettings settings)
        {
            mat.SetFloat("_waves_Amount", settings.frequency);
            mat.SetFloat("_waves_Amp", settings.amplitude);
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

        [Header("Canva Part to Display")] 
        [SerializeField] private GameObject canvaExploration;
        [SerializeField] private GameObject canvaFight;
        
        public void UpdateTypeOfUiByFightState()
        {
            if (FightManager.instance?.fightState == FightManager.FightState.InFight)
            {
                canvaExploration.SetActive(false);
                canvaFight.SetActive(true);
            }
            else
            {
                canvaExploration.SetActive(true);
                canvaFight.SetActive(false);
            }
        }

        private void UpdateTextSwap()
        {
            textSwap.text = FightManager.instance.numberOfSwap.ToString();
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
            StartCoroutine(HandleRadioTransition(enemyChipsWaveSettings));
        }
        #endregion

        #region Fight State Block
        
        public IEnumerator RadioBehaviorDependingFightState()
        {
            if (FightManager.instance?.fightState == FightManager.FightState.InFight)
            {
                if (NewPlayerController.instance?.rangeFinderManager?.rfAnimation != null)
                {
                    StartCoroutine(NewPlayerController.instance.rangeFinderManager.rfAnimation.TurnOffRangeFinder());
                }
                sliderAmplitude.interactable = false;
                sliderFrequency.interactable = false;
                playerOscillationHolder.SetActive(false);
                matRadioEnemy.SetFloat("_speed", 0);
                matRadioPlayer.SetFloat("_speed", 0);
                SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.apparitionUiCombat);
                StartCoroutine(brasSexController.TransitionBrasSexUi());
                yield return new WaitForSeconds(0.3f);
                UpdateTypeOfUiByFightState();
                
                
            }
            else
            {
                if (NewPlayerController.instance?.rangeFinderManager?.rfAnimation != null)
                {
                    StartCoroutine(NewPlayerController.instance.rangeFinderManager.rfAnimation.TurnOnRangeFinder());
                }
                sliderAmplitude.interactable = true;
                sliderFrequency.interactable = true;
                playerOscillationHolder.SetActive(true);
                matRadioEnemy.SetFloat("_speed", 0.02f);
                matRadioPlayer.SetFloat("_speed", 0.02f);
                SoundManager.instance.PlayMusicOneShot(SoundManager.instance.soundBankData.eventSound.disparitionUiCombat);
                StartCoroutine(brasSexController.TransitionBrasSexUi());
                yield return new WaitForSeconds(0.3f);
                UpdateTypeOfUiByFightState();
        
                
            }
        }
        #endregion
    }
}