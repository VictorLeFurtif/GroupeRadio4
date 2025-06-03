using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

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

        [Header("COLOR")] 
        [SerializeField] private Color colorWeak;
        [SerializeField] private Color colorMid;
        [SerializeField] private Color colorStrong;

        [Header("ANIMATION SETTINGS")]
        [SerializeField] private float appearDuration = 0.3f;
        [SerializeField] private float disappearDuration = 0.2f;
        [SerializeField] private Ease appearEase = Ease.OutBack;
        [SerializeField] private Ease disappearEase = Ease.InBack;
        [SerializeField] private float popScale = 1.1f;

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
            DOTween.SetTweensCapacity(1000, 50);
            
            player = NewPlayerController.instance;
            
            if (player == null)
            {
                Debug.LogError("No Player found");
            }
        }
    
        private void InitUiRangeFinder()
        {
          
            foreach (var uiData in uiElements)
            {
                if (uiData.uiObject != null)
                {
                    uiData.uiObject.transform.DOKill();
                    uiData.uiObject.transform.DOScale(Vector3.zero, disappearDuration)
                        .SetEase(disappearEase)
                        .OnComplete(() => Destroy(uiData.uiObject));
                }
            }
            uiElements.Clear();

            if (player.currentScanType == NewPlayerController.ScanType.None) return;

            List<BoxCollider2D> selectedList = GetCollidersForScanType(player.currentScanType);
            GameObject prefabToUse = GetPrefabForScanType(player.currentScanType);
            if (prefabToUse == null) return;

            Color targetColor = GetColorForScanType(player.currentScanType);

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

                ui.transform.localScale = Vector3.zero;

                RectTransform rect = ui.GetComponent<RectTransform>();
                Image image = ui.GetComponent<Image>();

                if (rect != null)
                {
                    rect.sizeDelta = new Vector2(uiWidth, rect.sizeDelta.y);
                }

                if (image != null)
                {
                    image.color = targetColor;
                }

                ui.transform.DOScale(Vector3.one * popScale, appearDuration)
                    .SetEase(appearEase)
                    .OnComplete(() => {
                  
                        ui.transform.DOScale(Vector3.one, appearDuration * 0.5f).SetEase(Ease.OutBounce);
                    });

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
                NewPlayerController.ScanType.Type1 => colorStrong,
                NewPlayerController.ScanType.Type2 => colorMid,
                NewPlayerController.ScanType.Type3 => colorWeak,
                _ => Color.white
            };
        }

        #endregion

        #region UPDATE UI

        public void UpdateUiRangeFinder()
        {
            if (FightManager.instance?.fightState == FightManager.FightState.InFight)
            {
                return;
            }
            
            RemoveDestroyedElements();
            List<BoxCollider2D> selectedList = GetCollidersForScanType(player.currentScanType);

            if (selectedList.Count != uiElements.Count)
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
                
                uiElements[i].uiObject.transform.DOMove(newPosition, 0.3f).SetEase(Ease.OutQuad);

                if (uiElements[i].rectTransform != null)
                {
                    float targetWidth = bounds.size.x * ratio;
                    uiElements[i].rectTransform.DOSizeDelta(
                        new Vector2(targetWidth, uiElements[i].rectTransform.sizeDelta.y), 
                        0.3f).SetEase(Ease.OutQuad);
                }

                if (uiElements[i].image != null)
                {
                    Color targetColor = GetColorForScanType(player.currentScanType);
                    uiElements[i].image.DOColor(targetColor, 0.3f);
                }
            }
        }

        #endregion
        
        public void RemoveDestroyedElements()
        {
            listWeakColliders.RemoveAll(c => c == null || c.gameObject == null);
            listMidColliders.RemoveAll(c => c == null || c.gameObject == null);
            listStrongColliders.RemoveAll(c => c == null || c.gameObject == null);
    
            uiElements.RemoveAll(uiData => {
                if (uiData.uiObject == null) return true;

                if (uiData.uiObject.transform.IsChildOf(parentRangeFinder.transform)) return false;
                uiData.uiObject.transform.DOKill();
                Destroy(uiData.uiObject);
                return true;
            });
    
            foreach (Transform child in parentRangeFinder.transform)
            {
                bool found = false;
                foreach (var uiData in uiElements)
                {
                    if (uiData.uiObject != child.gameObject) continue;
                    found = true;
                    break;
                }

                if (found) continue;
                child.DOKill(); 
                child.DOScale(Vector3.zero, disappearDuration)
                    .SetEase(disappearEase)
                    .OnComplete(() => Destroy(child.gameObject));
            }
        }
        
        private class UIElementData
        {
            public GameObject uiObject;
            public RectTransform rectTransform;
            public Image image;
        }
    }
}