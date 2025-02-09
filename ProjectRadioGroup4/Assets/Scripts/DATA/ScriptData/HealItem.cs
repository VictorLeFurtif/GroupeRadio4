using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Heal Item")]
public class HealItem : InventoryItem
{
    [field:Header("Heal Value"),SerializeField]
    public int healValue {get ; private set;}

    public HealItemInstance CreateHealInstance()
    {
        return new HealItemInstance(this);
    }
}

public class HealItemInstance
{
    public int healValue;
    public HealItemInstance(HealItem data)
    {
        healValue = data.healValue;
    }
    
    
}