using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class InventoryItem : ScriptableObject
{
    [field: Header("Item Name"), SerializeField]
    public string ItemName{ get; private set; }
    
    [field: Header("Item Icon"), SerializeField]
    public Sprite Icon{get; private set;}
    
    [field: Header("Item Description"), SerializeField, TextArea(15,20)]
     public string Description{get; private set;}
    
    public InventoryItemInstance Instance()
    {
        return new InventoryItemInstance(this);
    }
}

public class InventoryItemInstance
{
    public string itemName;
    public Sprite icon;
    public string description;

    public InventoryItemInstance(InventoryItem data)
    {
        itemName = data.ItemName;
        icon = data.Icon;
        description = data.Description;
    }

    

}
