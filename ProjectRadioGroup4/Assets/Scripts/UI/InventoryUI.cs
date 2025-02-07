using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<Image> slots;
    [SerializeField] private InventoryItem potionItem;

    private void Start()
    {
        InventoryManager.instance.AddItem(potionItem);
        UpdateInventoryUI();
    }

    private void UpdateInventoryUI()
    {
        List<InventoryItemInstance> inventory = InventoryManager.instance.GetInventory();

        for (int i = 0; i < slots.Count; i++)
        {
            if (i < inventory.Count)
            {
                slots[i].sprite = inventory[i].icon;
            }
            else
            {
                slots[i].sprite = null;
            }
        }
    }
}
