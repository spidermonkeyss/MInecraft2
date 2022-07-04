using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot
{
    public Item item;
    public int amount;
    public bool isEmpty;

    public InventorySlot()
    {
        item = null;
        amount = 0;
        isEmpty = true;
    }

    public void CopyInventorySlot(InventorySlot inventorySlot)
    {
        item = inventorySlot.item;
        amount = inventorySlot.amount;
        isEmpty = inventorySlot.isEmpty;
    }

    public void EmptyThisSlot()
    {
        item = null;
        amount = 0;
        isEmpty = true;
    }
}
