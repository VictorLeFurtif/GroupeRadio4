using UnityEngine.UI;
using System.Collections;
using System.Globalization;
using MANAGER;
using TMPro;
using UnityEngine;

namespace Controller
{
    public class BatteryPlayer : MonoBehaviour
    {
        private NewPlayerController player;
        private float _timer;
        [SerializeField] private float batteryCheckInterval = 1f;
        [SerializeField] private TMP_Text lifeText;
        
        private Coroutine transitionCoroutineLife;
        private float currentDisplayedLife; 
        
        [SerializeField] private Slider lifeSlider;
        [SerializeField] private Image lifeSliderFill;
        [SerializeField] private Color fullLifeColor = Color.green;
        [SerializeField] private Color noLifeColor = Color.red;
        [SerializeField] private Color midLifeColor = Color.yellow;
        
        private Coroutine transitionCoroutineSlider;
        
        [Header("Phase 2 Drain")]
        [SerializeField] private float phase2BatteryDrainPerSecond = 1f;
        private float drainTimer;

        private void Start()
        {
            Init();
        }

        private void Init()
        {
            player = NewPlayerController.instance;

            if (player == null)
            {
                Debug.LogWarning("No Player Controller instance was found");
            }

            UpdateLifeText();
        }
        private void Update()
        {
            HandlePhase2BatteryDrain();
        }

        #region LifeText

        public void UpdateLifeText()
        {
            if (transitionCoroutineLife != null)
                StopCoroutine(transitionCoroutineLife);
    
            if (player == null)
            {
                Debug.LogWarning("No PlayerController Was Found");
                return;
            }
            
            transitionCoroutineLife = StartCoroutine(SmoothTransitionLife(player._abstractEntityDataInstance.hp));
        }

        [SerializeField]
        private float durationTimeLerpLife = 0.5f;

        private IEnumerator SmoothTransitionLife(float targetLife)
        {
            float elapsed = 0f;
            float startLife = currentDisplayedLife; 

            while (elapsed < durationTimeLerpLife)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / durationTimeLerpLife); 

                currentDisplayedLife = Mathf.Lerp(startLife, targetLife, t);
                lifeText.text = currentDisplayedLife.ToString("00.00", CultureInfo.InvariantCulture) + "%"; 

                yield return null;
            }
            currentDisplayedLife = targetLife;
            lifeText.text = currentDisplayedLife.ToString("00.00", CultureInfo.InvariantCulture) + "%";
        }

        #endregion

        #region SliderLife

        public void UpdateLifeSlider(float targetLife)
        {
            if (transitionCoroutineSlider != null)
                StopCoroutine(transitionCoroutineSlider);

            transitionCoroutineSlider = StartCoroutine(SmoothTransitionSlider(targetLife));
        }
        
        private IEnumerator SmoothTransitionSlider(float targetLife)
        {
            float elapsed = 0f;
            float startValue = lifeSlider.value;
            float normalizedTarget = Mathf.Clamp01(targetLife / 100f);

            while (elapsed < durationTimeLerpLife)
            {
                elapsed += Time.deltaTime;
                float t = Mathf.Clamp01(elapsed / durationTimeLerpLife);

                float currentValue = Mathf.Lerp(startValue, normalizedTarget, t);
                lifeSlider.value = currentValue;

                lifeSliderFill.color = EvaluateLifeColor(currentValue);

                yield return null;
            }

            lifeSlider.value = normalizedTarget;
            lifeSliderFill.color = EvaluateLifeColor(normalizedTarget);
        }

        private Color EvaluateLifeColor(float normalizedValue)
        {
            return normalizedValue < 0.5f ? Color.Lerp(noLifeColor, midLifeColor, normalizedValue / 0.5f)
                : Color.Lerp(midLifeColor, fullLifeColor, (normalizedValue - 0.5f) / 0.5f);
        }

        #endregion
        
        private void HandlePhase2BatteryDrain()
        {
            if (!NewRadioManager.instance.IsActive || FightManager.instance?.fightState == FightManager.FightState.InFight) return;
    
            drainTimer += Time.deltaTime;
            if (drainTimer >= 1f)
            {
                drainTimer = 0f;
                ConsumeBattery(phase2BatteryDrainPerSecond);
            }
        }

        public void ConsumeBattery(float amount)
        {
            if (player == null) return;
    
            player.ManageLife(-amount);
    
            if (player._abstractEntityDataInstance.hp <= 0)
            {
                NewRadioManager.instance.StopMatchingGame();
            }
        }
        
        /*
        private void TickBatteryTimer()
        {
            if (!CanConsumeBattery()) return;

            _timer += Time.deltaTime;

            if (!(_timer >= batteryCheckInterval)) return;
            _timer = 0f;
            ApplyBatteryCost();
        }


        private void ApplyBatteryCost()
        {
            float cost = player.selectedAttack.attack.costBatteryExploration;

            if (RadioController.instance == null)return;
            if (cost <= 0f || !RadioController.instance.canTakeBattery) return;

            player.ManageLife(-cost);

            Debug.Log($" -{cost} batterie consommée pour l'attaque : {player.selectedAttack.attack.name}");
        }*/

    }
}