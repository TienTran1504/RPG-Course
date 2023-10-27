using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class InventoryItem
{
    public ItemData data;
    public int stackSize;
    public InventoryItem(ItemData _newItemData){
        data = _newItemData;
        //Add to stack
        AddStack();
    }
    public void AddStack() => stackSize++;
    public void RemoveStack() => stackSize--;
}
