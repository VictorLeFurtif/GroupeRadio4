using System;
using System.Collections;
using System.Globalization;
using MANAGER;
using TMPro;
using UnityEngine;

namespace Controller
{
    public class BatteryPlayer : MonoBehaviour
    {
        private PlayerController player;
        private float _timer;
        [SerializeField] private float batteryCheckInterval = 1f;
        [SerializeField] private TMP_Text lifeText;

        private void Start()
        {
            player = PlayerController.instance;

            if (player == null)
            {
                Debug.LogWarning("No Player Controller instance was found");
            }

            UpdateLifeText();
        }

        private void Update()
        {
            TickBatteryTimer();
        }

        private Coroutine transitionCoroutineLife;
        private float currentDisplayedLife; 

        public void UpdateLifeText()
        {
            if (transitionCoroutineLife != null)
                StopCoroutine(transitionCoroutineLife);
    
            if (PlayerController.instance == null)
            {
                Debug.LogWarning("No PlayerController Was Found");
                return;
            }
            
            transitionCoroutineLife = StartCoroutine(SmoothTransitionLife(PlayerController.instance._abstractEntityDataInstance.hp));
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
        
        private void TickBatteryTimer()
        {
            if (!CanConsumeBattery()) return;

            _timer += Time.deltaTime;

            if (!(_timer >= batteryCheckInterval)) return;
            _timer = 0f;
            ApplyBatteryCost();
        }

        private bool CanConsumeBattery()
        {
            return player != null
                   && player.selectedAttack != null
                   && FightManager.instance != null
                   && FightManager.instance.fightState != FightManager.FightState.InFight;
        }

        private void ApplyBatteryCost()
        {
            float cost = player.selectedAttack.attack.costBatteryExploration;

            if (cost <= 0f) return;

            player.ManageLife(-cost);

            Debug.Log($" -{cost} batterie consommée pour l'attaque : {player.selectedAttack.attack.name}");
        }
    }
}