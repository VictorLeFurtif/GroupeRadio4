using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxSlots = 12;
    [SerializeField] private List<InventoryItemInstance> inventory = new List<InventoryItemInstance>();
    public static InventoryManager instance;

    //au cas où n'oublie pas que ce code ne prend pas en compte la quantity dc risque de dupplication
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
    }

    public void AddItem(InventoryItem item)
    {
        if (inventory.Count >= maxSlots) return;
        InventoryItemInstance newItem = item.Instance();
        inventory.Add(newItem);
    }

    public void RemoveItem(InventoryItemInstance targetItem)
    {
        if ( targetItem != null)
        {
            inventory.Remove(targetItem);
        }
    }
    
    public List<InventoryItemInstance> GetInventory()
    {
        return inventory;
    }
}
