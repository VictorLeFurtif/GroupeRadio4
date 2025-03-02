using System.Collections;
using System.Collections.Generic;
using DATA.ScriptData.Item_link_to_invetory;
using UnityEngine;

[CreateAssetMenu(menuName = "Heal Item")]
public class HealItem : InventoryItem
{
    [field:Header("Heal Value"),SerializeField]
    public int healValue {get ; private set;}

    public override InventoryItemInstance Instance()
    {
        return new HealItemInstance(this);
    }
}

public class HealItemInstance : InventoryItemInstance
{
    public int healValue;
    public HealItemInstance(HealItem data) : base(data)
    {
        healValue = data.healValue;
    }
    
    
}