using System;
using System.Collections.Generic;
using System.Linq;
using AI.NEW_AI;
using DATA.Script.Chips_data;
using INTERACT;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random; 


namespace MANAGER
{
    public class ChipsManager : MonoBehaviour
    {
        #region Fields
        public static ChipsManager Instance;
        
        [Header("Tableau")]
        [SerializeField] private ChipsData[]chipsDatas = new ChipsData[6];
        
        public ChipsDataInstance[]chipsDatasTab = new ChipsDataInstance[6];

        [SerializeField] private ChipsData[] everyChipsNotInstance;
        [SerializeField] private ChipsDataInstance[] everyChips;
        
        [Header("Player Selection")]
        public List<ChipsDataInstance> playerChoiceChipsOrder = new List<ChipsDataInstance>();
        
        [SerializeField] private GameObject[]itemSlotsGO = new GameObject[6];
        
        [Header("UI")]
        [SerializeField] private GameObject chipSlotPrefab; 
        [SerializeField] private Transform inventoryGrid;  

        #endregion

        #region Unity Methods

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
        }

        
        private void Start()
        {
            Init();
            InitializeInventoryUI();
        }

        #endregion

        #region Initialization

        private void Init()
        {
            for (int i = 0; i < everyChipsNotInstance.Length; i++)
            {
                everyChips[i] = everyChipsNotInstance[i].Instance();
            }
            
            for (int i = 0; i < chipsDatas.Length; i++)
            {
                itemSlotsGO[i].GetComponent<InvetorySlot>().slotIndex = i; 
            }
        }

        public void IniTabChipsDataInstanceInFight(NewAi ai)
        {
            Array.Clear(chipsDatasTab, 0, chipsDatasTab.Length);
    
            for (int i = 0; i < ai.chipsDatasList.Count && i < chipsDatasTab.Length; i++)
            {
                chipsDatasTab[i] = ai.chipsDatasList[i];
            }

            List<ChipsDataInstance> availableChips = everyChips
                .Where(chip => !ai.chipsDatasList.Contains(chip))
                .ToList();

            int startIndex = ai.chipsDatasList.Count;
            
            for (int i = startIndex; i < chipsDatasTab.Length && availableChips.Count > 0; i++)
            {
                int randomIndex = Random.Range(0, availableChips.Count);
                chipsDatasTab[i] = availableChips[randomIndex];
                availableChips.RemoveAt(randomIndex);
            }

            chipsDatasTab = chipsDatasTab.OrderBy(x => Random.value).ToArray();
        }

        
        #endregion

        #region UI

        private void InitializeInventoryUI()
        {
            for (int i = 0; i < chipsDatasTab.Length; i++)
            {
                GameObject slot = Instantiate(chipSlotPrefab, inventoryGrid);
                Image chipImage = slot.GetComponentInChildren<Image>();
                chipImage.sprite = chipsDatasTab[i].visuelChips;
                slot.transform.SetParent(itemSlotsGO[i].transform);
            }
        }
        
        public void SwapChips(int index1, int index2)
        {
           
            (chipsDatasTab[index1], chipsDatasTab[index2]) = (chipsDatasTab[index2], chipsDatasTab[index1]);

            (chipsDatas[index1], chipsDatas[index2]) = (chipsDatas[index2], chipsDatas[index1]);
        }
        
        public void DeselectAllChips(bool animate = true)
        {
            for (int i = 0; i < chipsDatasTab.Length; i++)
            {
                chipsDatasTab[i].isSelected = false;
                
                if (itemSlotsGO[i].transform.childCount > 0)
                {
                    var feedback = itemSlotsGO[i].GetComponentInChildren<ChipVisualFeedback>();
                    if (feedback != null)
                    {
                        feedback.SetSelected(false, animate);
                    }
                }
            }
        }
        #endregion

        #region Logic for Fight

        public void MatchChips()
        {
            playerChoiceChipsOrder.Clear();
            foreach (var chip in chipsDatasTab)
            {
                if (chip.isSelected)
                {
                    playerChoiceChipsOrder.Add(chip);
                }
            }
        }
        
        public void ReverseChips()
        {
            playerChoiceChipsOrder.Clear();
            
            for (int i = chipsDatasTab.Length - 1; i >= 0; i--)
            {
                if (chipsDatasTab[i].isSelected)
                {
                    playerChoiceChipsOrder.Add(chipsDatasTab[i]);
                }
            }
        }
        
        #endregion
    }
}