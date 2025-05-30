using System;
using DATA.Script.Chips_data;
using INTERACT;
using UnityEngine;
using UnityEngine.UI;

namespace MANAGER
{
    public class ChipsManager : MonoBehaviour
    {
        #region Fields
        public static ChipsManager Instance;
        
        [Header("Tableau")]
        [SerializeField] private ChipsData[]chipsDatas = new ChipsData[6];
        [SerializeField] private ChipsDataInstance[]chipsDatasTab = new ChipsDataInstance[6];
        
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
            InitTabChipsDataInstance();
            InitializeInventoryUI();
        }

        #endregion

        #region Initialization

        private void InitTabChipsDataInstance()
        {
            for (int i = 0; i < chipsDatas.Length; i++)
            {
                chipsDatasTab[i] = chipsDatas[i].Instance();
                itemSlotsGO[i].GetComponent<InvetorySlot>().slotIndex = i; 
            }
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

        #endregion
    }
}