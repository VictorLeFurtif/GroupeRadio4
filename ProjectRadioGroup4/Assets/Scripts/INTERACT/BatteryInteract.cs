using System.Collections.Generic;
using INTERFACE;
using UnityEngine;

namespace INTERACT
{
    
    [System.Serializable]
    public class WaveSettings
    {
        public float frequency;
        public float amplitude;
        public float step;
    }
    
    public class BatteryInteract : MonoBehaviour, IInteractable, IWaveInteractable
    {
        [SerializeField] private List<WaveSettings> wavePatterns = new List<WaveSettings>();
        private int currentPatternIndex = 0;
        
        [SerializeField] private GameObject[] triggerZones = new GameObject[3];
        private int currentActiveZone = -1;
        private bool detected;
        private bool activationUsed = false;

        private void Start()
        {
            DisableAllZones();
            AddToInteractList();
        }
        
        #region ZoneHandler

        public bool CanBeActivated()
        {
            return !activationUsed;
        }
        
        public void MarkAsUsed()
        {
            activationUsed = true;
            DisableAllZones();
        }
        public void OnScan()
        {
            if (NewPlayerController.instance == null) return;
            int zoneIndex = (int)NewPlayerController.instance.currentScanType;
            SetActiveZone(zoneIndex);
        }

        public void Reveal()
        {
            throw new System.NotImplementedException();
        }

        private void SetActiveZone(int zoneIndex)
        {
            if (zoneIndex < 0 || zoneIndex >= triggerZones.Length)
            {
                Debug.LogWarning("Index de zone invalide. Doit être 0, 1 ou 2.");
                return;
            }

            if (currentActiveZone == zoneIndex)
                return;

            foreach (var t in triggerZones)
            {
                if (t != null)
                    t.SetActive(false);
            }

            triggerZones[zoneIndex].SetActive(true);
            currentActiveZone = zoneIndex;
        }

        private void DisableAllZones()
        {
            foreach (var t in triggerZones)
            {
                if (t != null)
                    t.SetActive(false);
            }

            currentActiveZone = -1;
        }

        #endregion

        public void AddToInteractList()
        {
            NewPlayerController.instance?.ListOfEveryElementInteractables.Add(this);
        }

        #region PhysicsAndContact

        private void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            if (detected)
            {
                //LOGIC IF PLAYER FOUND IT   
            }
            else
            {
                NewPlayerController.instance.ListOfEveryElementInteractables.Remove(this);
                NewPlayerController.instance.CanTurnOnPhase2Module = false;
                NewPlayerController.instance.currentInteractableInRange = null;
                NewPlayerController.instance.currentPhase2ModuleState = NewPlayerController.Phase2Module.Off;
                Destroy(gameObject);
            }
        }
        private void OnTriggerEnter2D(Collider2D other)
        {
            if (other == null || NewPlayerController.instance == null) return;
            if (other.CompareTag("Player"))
            {
                NewPlayerController.instance.CanTurnOnPhase2Module = true;
                NewPlayerController.instance.currentInteractableInRange = this;
            }
        }

        private void OnTriggerExit2D(Collider2D other)
        {
            if (other == null || NewPlayerController.instance == null) return;

            if (other.CompareTag("Player"))
            {
                NewPlayerController.instance.CanTurnOnPhase2Module = false;
                NewPlayerController.instance.currentInteractableInRange = null;
            }
        }

        #endregion

        #region Wave

        public WaveSettings GetCurrentWaveSettings()
        {
            if (wavePatterns.Count == 0 || currentPatternIndex >= wavePatterns.Count) 
                return null;
            return wavePatterns[currentPatternIndex];
        }
        
        public void MoveToNextPattern()
        {
            currentPatternIndex++;
            if (currentPatternIndex >= wavePatterns.Count)
            {
                wavePatterns.Clear();
            }
        }
        
        public bool HasRemainingPatterns() => wavePatterns.Count > 0;
        
        #endregion
        
    }
}