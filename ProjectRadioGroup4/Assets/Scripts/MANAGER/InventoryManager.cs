using System;
using System.Collections;
using System.Collections.Generic;
using DATA.ScriptData.Item_link_to_invetory;
using MANAGER;
using UnityEngine;

public class InventoryManager : MonoBehaviour
{
    [SerializeField] private int maxSlots = 12;
    [SerializeField] private List<InventoryItemInstance> inventory = new List<InventoryItemInstance>();
    public static InventoryManager instance;
    [SerializeField] private GameObject panelInventoryAction;
    [SerializeField] private InventoryItemInstance currentItem = null;

    //au cas où n'oublie pas que ce code ne prend pas en compte la quantity dc risque de dupplication
    private void Awake()
    {
        if (instance == null) instance = this;
        else Destroy(gameObject);
        panelInventoryAction.SetActive(false);
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

    public void SelectItem(int index)
    {
        if (index < 0 || index >= inventory.Count) return;
        if (inventory[index] == null) return;
        currentItem = inventory[index];
        panelInventoryAction.SetActive(true);
    }

    public void UseItemSelected()
    {
        if (FightManager.instance.fightState != FightManager.FightState.OutFight )
        {
            //FightManager.instance.EndTurn();
        }
        //do stuff
        RemoveItem(currentItem);
        InventoryUI.instance.UpdateInventoryUI();
        currentItem = null;
        panelInventoryAction.SetActive(false);
    }

    public void ThrowItemSelected()
    {
        RemoveItem(currentItem);
        InventoryUI.instance.UpdateInventoryUI();
        currentItem = null;
        panelInventoryAction.SetActive(false);
    }
    
    public List<InventoryItemInstance> GetInventory()
    {
        return inventory;
    }
}
