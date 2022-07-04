using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Hotbar : MonoBehaviour
{
    public Inventory playerInventory;
    public int hotbarSize = 4;
    public int currentSlotIndex;

    public GameObject currentSlotUI;
    public GameObject hotbarUI;

    private void Start()
    {
        currentSlotIndex = 0;

        for (int i = 0; i < hotbarUI.transform.childCount - 1; i++)
        {
            hotbarUI.transform.GetChild(i + 1).GetChild(0).GetComponent<Image>().raycastTarget = false;
        }
    }

    private void Update()
    {
        //Hotbar ui
        for (int i = 0; i < hotbarUI.transform.childCount - 1; i++)
        {
            if (playerInventory.inventorySlots[i].isEmpty)
                hotbarUI.transform.GetChild(i + 1).GetChild(1).gameObject.SetActive(false);
            else
            {
                if (playerInventory.inventorySlots[i].item.GetItemSprite() != null)
                {
                    hotbarUI.transform.GetChild(i + 1).GetChild(1).gameObject.SetActive(true);
                    hotbarUI.transform.GetChild(i + 1).GetChild(1).GetChild(0).GetComponent<Image>().sprite = playerInventory.inventorySlots[i].item.GetItemSprite();
                    if (playerInventory.inventorySlots[i].amount > 1)
                        hotbarUI.transform.GetChild(i + 1).GetChild(1).GetChild(1).GetComponent<Text>().text = playerInventory.inventorySlots[i].amount.ToString();
                    else
                        hotbarUI.transform.GetChild(i + 1).GetChild(1).GetChild(1).GetComponent<Text>().text = "";
                }
                else
                    hotbarUI.transform.GetChild(i + 1).GetChild(1).gameObject.SetActive(false);
            }
        }

        currentSlotUI.GetComponent<Text>().text = (GetComponent<Hotbar>().currentSlotIndex + 1).ToString();
    }

    public void RemoveItemFromCurrentSelection()
    {
        if (playerInventory.inventorySlots[currentSlotIndex].isEmpty)
            return;

        if (playerInventory.inventorySlots[currentSlotIndex].amount == 1)
            playerInventory.inventorySlots[currentSlotIndex].EmptyThisSlot();
        else
            playerInventory.inventorySlots[currentSlotIndex].amount--;
    }

    public void MoveCurrentSelectionUp()
    {
        currentSlotIndex--;
        if (currentSlotIndex == -1)
            currentSlotIndex = hotbarSize - 1;
    }

    public void MoveCurrentSelectionDown()
    {
        currentSlotIndex++;
        if (currentSlotIndex == hotbarSize)
            currentSlotIndex = 0;
    }

    public void SetInventorySlot(int slotIndex)
    {
        currentSlotIndex = slotIndex;
        if (currentSlotIndex >= hotbarSize)
            currentSlotIndex = 0;
    }

    public InventorySlot GetCurrentSlot()
    {
        return playerInventory.inventorySlots[currentSlotIndex];
    }
}
