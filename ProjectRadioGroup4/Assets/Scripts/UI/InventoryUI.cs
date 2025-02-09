using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventoryUI : MonoBehaviour
{
    [SerializeField] private List<Image> slots;
    [SerializeField] private InventoryItem potionItem;
    public static InventoryUI instance;

    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    private void Start()
    {
        InventoryManager.instance.AddItem(potionItem);
        InventoryManager.instance.AddItem(potionItem);
        InventoryManager.instance.AddItem(potionItem);
        InventoryManager.instance.AddItem(potionItem);
        InventoryManager.instance.AddItem(potionItem);
        InventoryManager.instance.AddItem(potionItem);
        InventoryManager.instance.AddItem(potionItem);
        
        UpdateInventoryUI();
    }

    public void UpdateInventoryUI()
    {
        List<InventoryItemInstance> inventory = InventoryManager.instance.GetInventory();

        for (int i = 0; i < slots.Count; i++)
        {
            if (slots[i] == null)
            {
                continue;
            }

            slots[i].sprite = i < inventory.Count ? inventory[i].icon : null;
        }
    }
}
