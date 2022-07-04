using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Inventory
{
    public int inventorySize;
    public InventorySlot[] inventorySlots;
    public bool isCreative;

    public Inventory(int _inventorySize ,bool _isCreative)
    {
        inventorySize = _inventorySize;
        inventorySlots = new InventorySlot[inventorySize];

        for (int i = 0; i < inventorySlots.Length; i++)
            inventorySlots[i] = new InventorySlot();

        isCreative = _isCreative;
    }

    //public void AddItemToInventory<itemType>() where itemType : Item , new()
    //{
    //    for (int i = 0; i < inventorySlots.Length; i++)
    //    {
    //        if (inventorySlots[i].isEmpty)
    //        {
    //            //inventorySlots[i].itemType = typeof(itemType);
    //            inventorySlots[i].item = new itemType();
    //            inventorySlots[i].amount++;
    //            inventorySlots[i].isEmpty = false;
    //            return;
    //        }
    //        
    //        if (inventorySlots[i].item.GetType() == typeof(itemType))
    //        {
    //            inventorySlots[i].amount++;
    //            return;
    //        }
    //    }
    //}

    protected virtual void OnInventoryUpdate()
    {

    }

    public void AddItemToInventory(System.Type itemType)
    {
        if (!itemType.IsSubclassOf(typeof(Item)))
            return;

        for (int i = 0; i < inventorySlots.Length; i++)
        {
            if (inventorySlots[i].isEmpty)
            {
                inventorySlots[i].item = (Item)System.Activator.CreateInstance(itemType);
                inventorySlots[i].amount++;
                inventorySlots[i].isEmpty = false;
                OnInventoryUpdate();
                return;
            }
            if (inventorySlots[i].item.GetType() == itemType)
            {
                //If stack is full go to next slot
                if (inventorySlots[i].amount < inventorySlots[i].item.maxStackSize)
                {
                    inventorySlots[i].amount++;
                    OnInventoryUpdate();
                    return;
                }
            }
        }
        Debug.Log("Couldn't pick up item, inventory full");
    }

    public void RemoveItemFromInventory(int slotIndex)
    {
        if (inventorySlots[slotIndex].isEmpty)
            return;

        if (inventorySlots[slotIndex].amount == 1)
            inventorySlots[slotIndex].EmptyThisSlot();
        else
            inventorySlots[slotIndex].amount--;
        OnInventoryUpdate();
    }

    public bool MoveItemStackFromSlotToSlot(int slotFrom, int slotTo)
    {
        if (!inventorySlots[slotTo].isEmpty)
        {
            if (inventorySlots[slotTo].item.GetType() == inventorySlots[slotFrom].item.GetType() && (inventorySlots[slotTo].amount + inventorySlots[slotFrom].amount) <= inventorySlots[slotTo].item.maxStackSize)
            {
                inventorySlots[slotTo].amount += inventorySlots[slotFrom].amount;
                inventorySlots[slotFrom].EmptyThisSlot();
                OnInventoryUpdate();
                return true;
            }
            else
            {
                Debug.Log("Slot isnt empty");
                return false;
            }
        }
        else
        {
            inventorySlots[slotTo].CopyInventorySlot(inventorySlots[slotFrom]);
            inventorySlots[slotFrom].EmptyThisSlot();
            OnInventoryUpdate();

            return true;
        }
    }

    public static bool MoveItemStackFromInventoryToInventory(Inventory inventoryFrom, int slotFrom, Inventory inventoryTo, int slotTo)
    {
        if (!inventoryTo.inventorySlots[slotTo].isEmpty)
        {
            if (inventoryTo.inventorySlots[slotTo].item.GetType() == inventoryFrom.inventorySlots[slotFrom].item.GetType() && (inventoryTo.inventorySlots[slotTo].amount + inventoryFrom.inventorySlots[slotFrom].amount) <= inventoryTo.inventorySlots[slotTo].item.maxStackSize)
            {
                inventoryTo.inventorySlots[slotTo].amount += inventoryFrom.inventorySlots[slotFrom].amount;
                inventoryFrom.inventorySlots[slotFrom].EmptyThisSlot();
                inventoryTo.OnInventoryUpdate();
                inventoryFrom.OnInventoryUpdate();

                return true;
            }
            else
            {
                Debug.Log("Slot isnt empty");
                return false;
            }
        }
        else
        {
            inventoryTo.inventorySlots[slotTo].CopyInventorySlot(inventoryFrom.inventorySlots[slotFrom]);
            inventoryFrom.inventorySlots[slotFrom].EmptyThisSlot();
            inventoryTo.OnInventoryUpdate();
            inventoryFrom.OnInventoryUpdate();

            return true;
        }
    }

    public bool MoveOneItemFromSlotToSlot(Item item, int slotFrom, int slotTo)
    {
        if (!inventorySlots[slotTo].isEmpty)
        {
            if (inventorySlots[slotTo].item.GetType() == item.GetType() && inventorySlots[slotTo].amount < inventorySlots[slotTo].item.maxStackSize)
            {
                inventorySlots[slotTo].amount++;
                inventorySlots[slotFrom].amount--;
                if (inventorySlots[slotFrom].amount == 0)
                    inventorySlots[slotFrom].EmptyThisSlot();
                OnInventoryUpdate();
                return true;
            }
            else
            {
                Debug.Log("Slot isnt empty");
                return false;
            }
        }
        else
        {
            inventorySlots[slotTo].CopyInventorySlot(inventorySlots[slotFrom]);
            inventorySlots[slotTo].amount = 1;
            inventorySlots[slotFrom].amount--;
            if (inventorySlots[slotFrom].amount == 0)
                inventorySlots[slotFrom].EmptyThisSlot();
            OnInventoryUpdate();
            return true;
        }
    }

    public static bool MoveOneItemFromInventoryToInventory(Item item, Inventory inventoryFrom, int slotFrom, Inventory inventoryTo, int slotTo)
    {
        if (!inventoryTo.inventorySlots[slotTo].isEmpty)
        {
            if (inventoryTo.inventorySlots[slotTo].item.GetType() == item.GetType() && inventoryTo.inventorySlots[slotTo].amount < inventoryTo.inventorySlots[slotTo].item.maxStackSize)
            {
                inventoryTo.inventorySlots[slotTo].amount++;
                inventoryFrom.inventorySlots[slotFrom].amount--;
                if (inventoryFrom.inventorySlots[slotFrom].amount == 0)
                    inventoryFrom.inventorySlots[slotFrom].EmptyThisSlot();
                inventoryTo.OnInventoryUpdate();
                inventoryFrom.OnInventoryUpdate();
                return true;
            }
            else
            {
                Debug.Log("Slot isnt empty");
                return false;
            }
        }
        else
        {
            inventoryTo.inventorySlots[slotTo].CopyInventorySlot(inventoryFrom.inventorySlots[slotFrom]);
            inventoryTo.inventorySlots[slotTo].amount = 1;
            inventoryFrom.inventorySlots[slotFrom].amount--;
            if (inventoryFrom.inventorySlots[slotFrom].amount == 0)
                inventoryFrom.inventorySlots[slotFrom].EmptyThisSlot();
            inventoryTo.OnInventoryUpdate();
            inventoryFrom.OnInventoryUpdate();
            return true;
        }
    }
}