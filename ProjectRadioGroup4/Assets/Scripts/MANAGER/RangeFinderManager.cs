using System;
using System.Collections.Generic;
using INTERACT;
using INTERFACE;
using UnityEngine;

namespace MANAGER
{
    public class RangeFinderManager : MonoBehaviour
    {
        private NewPlayerController player;

        [SerializeField] private GameObject prefabsSizeMid;
        [SerializeField] private GameObject prefabsSizeStrong;
        [SerializeField] private GameObject prefabsSizeWeak;

        [SerializeField] private float ratio;

        public List<BatteryInteract> everyScriptInteractable = new List<BatteryInteract>();
        private List<GameObject> uiElements = new List<GameObject>();
        
        [SerializeField] private GameObject parentRangeFinder;
        
        private void Start()
        {
            Init();
        }

        private void Init()
        {
            player = NewPlayerController.instance;
            
            if (player == null)
            {
                Debug.LogError("No Player found");
            }
            InitUiRangeFinder();
        }

        private void InitUiRangeFinder()
        {
            foreach (var ui in uiElements)
            {
                if (ui != null) Destroy(ui);
            }
            uiElements.Clear();
            
            foreach (var battery in everyScriptInteractable)
            {
                if (battery == null) continue;

                float distance = Mathf.Abs(player.transform.position.x - battery.transform.position.x);
                GameObject ui = Instantiate(
                    prefabsSizeMid, 
                    parentRangeFinder.transform.position + new Vector3(distance * ratio, 0, 0),
                    Quaternion.identity,
                    parentRangeFinder.transform);
                
                uiElements.Add(ui);
            }
        }

        private void UpdateUiRangeFinder()
        {
            if (everyScriptInteractable.Count != uiElements.Count) // pour gérer l'erreur avec Eric
            {
                InitUiRangeFinder();
                return;
            }

            for (int i = 0; i < everyScriptInteractable.Count; i++)
            {
                if (everyScriptInteractable[i] == null || uiElements[i] == null) continue;

                float distance = Mathf.Abs(player.transform.position.x - everyScriptInteractable[i].transform.position.x);
                Vector3 newPosition = parentRangeFinder.transform.position + new Vector3(distance * ratio, 0, 0);
                uiElements[i].transform.position = newPosition;
            }
        }

        private void Update()
        {
            if (!player.transform.hasChanged) return;
            UpdateUiRangeFinder();
            player.transform.hasChanged = false;
        }
    }
}
