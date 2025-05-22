using System;
using System.Collections.Generic;

using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    public class RangeFinderManager : MonoBehaviour
    {
        #region FIELDS

        private NewPlayerController player;

        [SerializeField] private GameObject prefabsSizeMid;
        [SerializeField] private GameObject prefabsSizeStrong;
        [SerializeField] private GameObject prefabsSizeWeak;

        [SerializeField] private float ratio;
        
        private List<UIElementData> uiElements = new List<UIElementData>();
        
        public List<BoxCollider2D> listWeakColliders = new List<BoxCollider2D>();
        public List<BoxCollider2D> listMidColliders = new List<BoxCollider2D>();
        public List<BoxCollider2D> listStrongColliders = new List<BoxCollider2D>();
        
        [SerializeField] private GameObject parentRangeFinder;

        #endregion

        #region UNITY METHOD

        private void Start()
        {
            Init();
        }
        
        private void Update()
        {
            if (player.currentScanType == NewPlayerController.ScanType.None)
            {
                InitUiRangeFinder(); 
                return;
            }
            if (!player.transform.hasChanged) return;
            UpdateUiRangeFinder();
            player.transform.hasChanged = false;
        }

        #endregion

        #region INITIALISATION

        private void Init()
        {
            player = NewPlayerController.instance;
            
            if (player == null)
            {
                Debug.LogError("No Player found");
            }
        }
    
        private void InitUiRangeFinder()
        {
            uiElements.Clear();

            if (player.currentScanType == NewPlayerController.ScanType.None) return;

            List<BoxCollider2D> selectedList = GetCollidersForScanType(player.currentScanType);
            GameObject prefabToUse = GetPrefabForScanType(player.currentScanType);
            if (prefabToUse == null) return;

            uiElements.Clear();

            foreach (var collider in selectedList)
            {
                if (collider == null || !collider.enabled) continue;

                Bounds bounds = collider.bounds;

                float distanceX = bounds.center.x - player.transform.position.x;
                float uiX = distanceX * ratio;
                float uiWidth = bounds.size.x * ratio;

                GameObject ui = Instantiate(
                    prefabToUse,
                    parentRangeFinder.transform.position + new Vector3(uiX, 0, 0),
                    Quaternion.identity,
                    parentRangeFinder.transform
                );

                RectTransform rect = ui.GetComponent<RectTransform>();
                Image image = ui.GetComponent<Image>();

                if (rect != null)
                {
                    rect.sizeDelta = new Vector2(uiWidth, rect.sizeDelta.y);
                }

                uiElements.Add(new UIElementData
                {
                    uiObject = ui,
                    rectTransform = rect,
                    image = image
                });
            }
        }

        #endregion

        #region GET FROM SCAN TYPE

        private List<BoxCollider2D> GetCollidersForScanType(NewPlayerController.ScanType scanType)
        {
            return scanType switch
            {
                NewPlayerController.ScanType.Type1 => listStrongColliders,
                NewPlayerController.ScanType.Type2 => listMidColliders,
                NewPlayerController.ScanType.Type3 => listWeakColliders,
                _ => new List<BoxCollider2D>()
            };
        }

        private GameObject GetPrefabForScanType(NewPlayerController.ScanType scanType)
        {
            return scanType switch
            {
                NewPlayerController.ScanType.Type3 => prefabsSizeWeak,
                NewPlayerController.ScanType.Type2 => prefabsSizeMid,
                NewPlayerController.ScanType.Type1 => prefabsSizeStrong,
                NewPlayerController.ScanType.None => null,
                _ => throw new ArgumentOutOfRangeException(nameof(scanType), scanType, null)
            };
        }
        
        private Color GetColorForScanType(NewPlayerController.ScanType scanType)
        {
            return scanType switch
            {
                NewPlayerController.ScanType.Type1 => Color.red,
                NewPlayerController.ScanType.Type2 => Color.yellow,
                NewPlayerController.ScanType.Type3 => Color.green,
                _ => Color.white
            };
        }


        #endregion

        #region UPDATE UI

        public void UpdateUiRangeFinder()
        {
            List<BoxCollider2D> selectedList = GetCollidersForScanType(player.currentScanType);

            if (selectedList.Count != uiElements.Count) //CORRIGE BUG DE ERIC
            {
                InitUiRangeFinder();
                return;
            }

            for (int i = 0; i < selectedList.Count; i++)
            {
                if (selectedList[i] == null || uiElements[i] == null || uiElements[i].uiObject == null) continue;

                Bounds bounds = selectedList[i].bounds;
                float distanceX = bounds.center.x - player.transform.position.x;
                float uiX = distanceX * ratio;

                Vector3 newPosition = parentRangeFinder.transform.position + new Vector3(uiX, 0, 0);
                uiElements[i].uiObject.transform.position = newPosition;

                if (uiElements[i].rectTransform != null)
                {
                    uiElements[i].rectTransform.sizeDelta = new Vector2(bounds.size.x * ratio, uiElements[i].rectTransform.sizeDelta.y);
                }

                if (uiElements[i].image != null)
                {
                    uiElements[i].image.color = GetColorForScanType(player.currentScanType);
                }
            }
        }

        #endregion
        
        private class UIElementData
        {
            public GameObject uiObject;
            public RectTransform rectTransform;
            public Image image;
        }

    }
}
