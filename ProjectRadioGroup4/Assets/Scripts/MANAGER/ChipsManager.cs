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

        [Header("Tableau")]
        [SerializeField] private ChipsData[]chipsDatas = new ChipsData[6];
        [SerializeField] private ChipsDataInstance[]chipsDatasTab = new ChipsDataInstance[6];
        public GameObject[]itemSlotsGO = new GameObject[6];
        private DraggableItem[] draggableItems;
        
        [Header("UI")]
        [SerializeField] private GameObject chipSlotPrefab; 
        [SerializeField] private Transform inventoryGrid;  

        #endregion

        #region Unity Methods

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
            }
        }

        #endregion

        #region UI

        private void InitializeInventoryUI()
        {
            draggableItems = new DraggableItem[chipsDatasTab.Length];
            for (int i = 0; i < chipsDatasTab.Length; i++)
            {
                GameObject slot = Instantiate(chipSlotPrefab, inventoryGrid);
                Image chipImage = slot.GetComponentInChildren<Image>();
                chipImage.sprite = chipsDatasTab[i].visuelChips;
                slot.transform.SetParent(itemSlotsGO[i].transform);
                slot.GetComponent<DraggableItem>().chipIndex = i;
                
                draggableItems[i] = slot.GetComponent<DraggableItem>();
                draggableItems[i].chipIndex = i;
            }
            
        }

        public void SwapChips(int fromIndex, int toIndex)
        {
            (chipsDatasTab[fromIndex], chipsDatasTab[toIndex]) = (chipsDatasTab[toIndex], chipsDatasTab[fromIndex]);
            
            Transform fromChild = itemSlotsGO[fromIndex].transform.GetChild(0);
            Transform toChild = itemSlotsGO[toIndex].transform.GetChild(0);
    
            fromChild.SetParent(itemSlotsGO[toIndex].transform, false);
            toChild.SetParent(itemSlotsGO[fromIndex].transform, false);
            
            (draggableItems[fromIndex], draggableItems[toIndex]) = (draggableItems[toIndex], draggableItems[fromIndex]);
            draggableItems[fromIndex].chipIndex = fromIndex;
            draggableItems[toIndex].chipIndex = toIndex;
            
            fromChild.localPosition = Vector3.zero;
            toChild.localPosition = Vector3.zero;
        }

        #endregion
    }
}
