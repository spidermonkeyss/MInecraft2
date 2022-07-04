using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingInventory : Inventory
{
    public int gridSize;

    public CraftingInventory(int _inventorySize, bool _isCreative) : base(_inventorySize, _isCreative)
    {
        inventorySize = _inventorySize + 1;
        inventorySlots = new InventorySlot[inventorySize];
        gridSize = (int)Mathf.Sqrt(_inventorySize);

        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i] = new InventorySlot();
    }

    protected override void OnInventoryUpdate()
    {
        base.OnInventoryUpdate();
        InventorySlot craftedItem = CraftingRecipe.CheckRecipes(this);
        if (craftedItem != null)
            inventorySlots[inventorySize - 1].CopyInventorySlot(craftedItem);
        else
            inventorySlots[inventorySize - 1].EmptyThisSlot();
    }

    public void ConfirmCraft()
    {
        //if inventory is full then drop crafted item
        Debug.Log("Craft confirmed");

        for (int i = 0; i < inventorySize - 1; i++)
            inventorySlots[i].EmptyThisSlot();
    }
}
