using System.Collections.Generic;
using INTERFACE;
using MANAGER;
using UnityEngine;

namespace INTERACT
{
    
    [System.Serializable]
    public class WaveSettings
    {
        public float frequency;
        public float amplitude;
        public float step;
        
        public WaveSettings(float freq, float amp, float st)
        {
            frequency = freq;
            amplitude = amp;
            step = st;
        }
    }
    
    public class BatteryInteract : MonoBehaviour,IWaveInteractable
    {
        public List<WaveSettings> wavePatterns = new List<WaveSettings>();
        public int currentPatternIndex = 0;
        
        [SerializeField] private GameObject[] triggerZones = new GameObject[3];
        private int currentActiveZone = -1;
        private bool activationUsed = false;

        [Header("Reward")]
        [SerializeField] private float lifeAmountToGive;
        
        [Header("Wave Generation Settings")]
        [SerializeField] private int basePatternCount = 3;
        [SerializeField] private int maxPatternCount = 5;
        [SerializeField] private float minFrequency = 0.2f;
        [SerializeField] private float maxFrequency = 1f;
        [SerializeField] private float minAmplitude = 0.1f;
        [SerializeField] private float maxAmplitude = 0.4f;
        [SerializeField] private float minStep = 0.1f;
        [SerializeField] private float maxStep = 1f;
        [SerializeField] private float generationRadius = 10f;
        
        protected SpriteRenderer spriteRenderer;
        private bool detected;
        private Transform playerTransform;
        

        protected virtual void Start()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            playerTransform = NewPlayerController.instance?.transform;
            DisableAllZones();
            AddToInteractList();
            UpdateSpriteVisibility();
            PositionTriggerZonesRandomly();
            PutAllColliderInRangeFinderLists();
        }
        
        public void GenerateWavePatterns()
        {
            wavePatterns.Clear();
            currentPatternIndex = 0;

            if (playerTransform == null) return;
            
            float distance = Vector3.Distance(transform.position, playerTransform.position);
            int patternCount = Mathf.Clamp(
                basePatternCount + Mathf.FloorToInt(distance / generationRadius * (maxPatternCount - basePatternCount)),
                basePatternCount,
                maxPatternCount
            );
            
            for (int i = 0; i < patternCount; i++)
            {
                wavePatterns.Add(new WaveSettings(
                    Random.Range(minFrequency, maxFrequency),
                    Random.Range(minAmplitude, maxAmplitude),
                    Random.Range(minStep, maxStep)
                ));
            }
        }
        
        public void GenerateWavePatterns(Transform _transform)
        {
            wavePatterns.Clear();
            currentPatternIndex = 0;

            if (playerTransform == null) return;
            
            float distance = Vector3.Distance(_transform.position, playerTransform.position);
            int patternCount = Mathf.Clamp(
                basePatternCount + Mathf.FloorToInt(distance / generationRadius * (maxPatternCount - basePatternCount)),
                basePatternCount,
                maxPatternCount
            );
            
            for (int i = 0; i < patternCount; i++)
            {
                wavePatterns.Add(new WaveSettings(
                    Random.Range(minFrequency, maxFrequency),
                    Random.Range(minAmplitude, maxAmplitude),
                    Random.Range(minStep, maxStep)
                ));
            }
        }
        
        public void GenerateWavePatterns(bool avantage)
        {
            wavePatterns.Clear();
            currentPatternIndex = 0;

            int patternCount = avantage? basePatternCount : maxPatternCount;
            
            for (int i = 0; i < patternCount; i++)
            {
                wavePatterns.Add(new WaveSettings(
                    Random.Range(minFrequency, maxFrequency),
                    Random.Range(minAmplitude, maxAmplitude),
                    Random.Range(minStep, maxStep)
                ));
            }
        }
        
        #region ZoneHandler

        public bool CanBeActivated()
        {
            return !activationUsed;
        }

        public void Activate()
        {
            GenerateWavePatterns();
        }
        
        public void MarkAsUsed()
        {
            activationUsed = true;
            DisableAllZones();
        }

        public bool Detected 
        { 
            get => detected;
            set
            {
                detected = value;
                UpdateSpriteVisibility();
            }
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

        protected virtual void OnCollisionEnter2D(Collision2D other)
        {
            if (!other.gameObject.CompareTag("Player")) return;
            
            if (Detected)
            {
                ContactWithPlayerAfterDetected();  
            }
            
            CancelInteractionAfterContact();
            Destroy(gameObject);
        }

        protected virtual void CancelInteractionAfterContact()
        {
            NewPlayerController.instance.ListOfEveryElementInteractables.Remove(this);
            NewPlayerController.instance.CanTurnOnPhase2Module = false;
            NewPlayerController.instance.currentInteractableInRange = null;
            NewPlayerController.instance.currentPhase2ModuleState = NewPlayerController.Phase2Module.Off;
        }
        
        protected virtual void OnTriggerEnter2D(Collider2D other)
        {
            
            if (other == null || NewPlayerController.instance == null) return;
            if (other.CompareTag("Player"))
            {
                NewPlayerController.instance.CanTurnOnPhase2Module = true;
                NewPlayerController.instance.currentInteractableInRange = this;
            }
        }

        protected virtual void OnTriggerExit2D(Collider2D other)
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

        protected virtual void ContactWithPlayerAfterDetected()
        {
            NewPlayerController.instance?.ManageLife(lifeAmountToGive);
        }
        
        private void UpdateSpriteVisibility()
        {
            if (spriteRenderer != null)
            {
                spriteRenderer.enabled = detected;
            }
        }

        #region  Handle Position collider

        
        private void PositionTriggerZonesRandomly()
        {
            foreach (GameObject zone in triggerZones)
            {
                if (zone == null) continue;
                
                BoxCollider2D collider = zone.GetComponent<BoxCollider2D>();
                if (collider == null) continue;
                
                Vector2 randomLocalPos = GetRandomPositionInsideCollider(collider);
                
                zone.transform.localPosition = randomLocalPos;
            }
        }
        
        private Vector2 GetRandomPositionInsideCollider(BoxCollider2D collider)
        {
            float maxX = collider.size.x * 0.5f;
            float maxY = collider.size.y * 0.5f;
            
            float randomX = Random.Range(-maxX, maxX);
            float randomY = Random.Range(-maxY, maxY);
            
            return new Vector2(randomX, randomY);
        }

        private void PutAllColliderInRangeFinderLists()
        {
            NewPlayerController.instance?.rangeFinderManager.listStrongColliders.Add(triggerZones[0].GetComponent<BoxCollider2D>());
            NewPlayerController.instance?.rangeFinderManager.listMidColliders.Add(triggerZones[1].GetComponent<BoxCollider2D>());
            NewPlayerController.instance?.rangeFinderManager.listWeakColliders.Add(triggerZones[2].GetComponent<BoxCollider2D>());
        }
        #endregion
        
        
    }
}