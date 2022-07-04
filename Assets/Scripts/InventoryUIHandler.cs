using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InventoryUIHandler : MonoBehaviour, IPointerClickHandler
{
    public GameObject inventoryUI;
    public GameObject inventoryBackDropUI;
    public GameObject craftingGridUI;
    public GameObject slotPrefab;

    public int selectedSlotIndex;
    public bool isInventorySlotSelected;
    public bool isCraftingGridSlotSelected;
    public int maxRowWidth;
    private int numberOfRows;
    public float slotSpacing;

    public Inventory inventoryToShow;
    public CraftingInventory craftingInventory;
    public InventorySlot pointerSlot;

    void Update()
    {
        if (inventoryUI.activeSelf)
            ShowInventory();

        if (craftingGridUI.activeSelf)
            ShowCraftingGrid();
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Left)
            LeftClick(eventData);
        else if (eventData.button == PointerEventData.InputButton.Right)
            RightClick(eventData);
    }

    void LeftClick(PointerEventData eventData)
    {
        //Place Stack
        if (isInventorySlotSelected)
        {
            //Place stack from inventory slot to new inventory slot
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "InventoryUI")
            {
                inventoryToShow.MoveItemStackFromSlotToSlot(selectedSlotIndex, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isInventorySlotSelected = false;
            }
            //Place stack from inventory slot to crafting grid slot
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "CraftingGrid")
            {
                //Crafted slot
                if (GetSlotIndex(eventData.pointerCurrentRaycast.gameObject) == craftingInventory.inventorySize - 1)
                    return;
                Inventory.MoveItemStackFromInventoryToInventory(inventoryToShow, selectedSlotIndex, craftingInventory, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isInventorySlotSelected = false;
            }
        }
        else if (isCraftingGridSlotSelected)
        {
            //Place stack from grid to inventory
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "InventoryUI")
            {
                Inventory.MoveItemStackFromInventoryToInventory(craftingInventory, selectedSlotIndex, inventoryToShow, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isCraftingGridSlotSelected = false;
            }
            //Place stack from grid slot to grid slot
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "CraftingGrid")
            {
                //Crafted slot
                if (GetSlotIndex(eventData.pointerCurrentRaycast.gameObject) == craftingInventory.inventorySize - 1)
                    return;
                craftingInventory.MoveItemStackFromSlotToSlot(selectedSlotIndex, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isCraftingGridSlotSelected = false;
            }
        }
        //Pickup Stack
        else
        {
            //Pickup stack from inventory slot
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "InventoryUI")
            {
                if (!inventoryToShow.inventorySlots[GetSlotIndex(eventData.pointerCurrentRaycast.gameObject)].isEmpty)
                {
                    isInventorySlotSelected = true;
                    selectedSlotIndex = GetSlotIndex(eventData.pointerCurrentRaycast.gameObject);
                }
            }
            //Pickup stack from crafting grid slot
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "CraftingGrid")
            {
                if (!craftingInventory.inventorySlots[GetSlotIndex(eventData.pointerCurrentRaycast.gameObject)].isEmpty)
                {
                    isCraftingGridSlotSelected = true;
                    selectedSlotIndex = GetSlotIndex(eventData.pointerCurrentRaycast.gameObject);
                    
                    //Pickup stack from crafted slot
                    if (selectedSlotIndex == craftingInventory.inventorySize - 1)
                        craftingInventory.ConfirmCraft();
                }
            }
        }
    }

    void RightClick(PointerEventData eventData)
    {
        //Place one item
        if (isInventorySlotSelected)
        {
            //Place item from inventory slot to new inventory slot
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "InventoryUI")
            {
                inventoryToShow.MoveOneItemFromSlotToSlot(inventoryToShow.inventorySlots[selectedSlotIndex].item, selectedSlotIndex, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isInventorySlotSelected = false;
            }
            //Place item from inventory slot to crafting grid slot
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "CraftingGrid")
            {
                //Crafted slot
                if (GetSlotIndex(eventData.pointerCurrentRaycast.gameObject) == craftingInventory.inventorySize - 1)
                    return;
                Inventory.MoveOneItemFromInventoryToInventory(inventoryToShow.inventorySlots[selectedSlotIndex].item, inventoryToShow, selectedSlotIndex, craftingInventory, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isInventorySlotSelected = false;
            }
        }
        else if (isCraftingGridSlotSelected)
        {
            //Place item from grid to inventory
            if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "InventoryUI")
            {
                Inventory.MoveOneItemFromInventoryToInventory(craftingInventory.inventorySlots[selectedSlotIndex].item, craftingInventory, selectedSlotIndex, inventoryToShow, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isCraftingGridSlotSelected = false;
            }
            //Place item from grid slot to grid slot
            else if (eventData.pointerCurrentRaycast.gameObject.transform.parent.parent.tag == "CraftingGrid")
            {
                //Crafted slot
                if (GetSlotIndex(eventData.pointerCurrentRaycast.gameObject) == craftingInventory.inventorySize - 1)
                    return;
                craftingInventory.MoveOneItemFromSlotToSlot(craftingInventory.inventorySlots[selectedSlotIndex].item, selectedSlotIndex, GetSlotIndex(eventData.pointerCurrentRaycast.gameObject));
                isCraftingGridSlotSelected = false;
            }
        }
    }

    void ShowInventory()
    {
        for (int i = 0; i < inventoryToShow.inventorySlots.Length; i++)
        {
            if (inventoryToShow.inventorySlots[i].isEmpty)
                inventoryUI.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            else
            {
                if (inventoryToShow.inventorySlots[i].item.GetItemSprite() != null)
                {
                    inventoryUI.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    inventoryUI.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>().sprite = inventoryToShow.inventorySlots[i].item.GetItemSprite();
                    if (inventoryToShow.inventorySlots[i].amount > 1)
                        inventoryUI.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Text>().text = inventoryToShow.inventorySlots[i].amount.ToString();
                    else
                        inventoryUI.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Text>().text = "";
                }
                else
                    inventoryUI.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    void ShowCraftingGrid()
    {
        for (int i = 0; i < craftingInventory.inventorySlots.Length; i++)
        {
            if (craftingInventory.inventorySlots[i].isEmpty)
                craftingGridUI.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            else
            {
                if (craftingInventory.inventorySlots[i].item.GetItemSprite() != null)
                {
                    craftingGridUI.transform.GetChild(i).GetChild(1).gameObject.SetActive(true);
                    craftingGridUI.transform.GetChild(i).GetChild(1).GetChild(0).GetComponent<Image>().sprite = craftingInventory.inventorySlots[i].item.GetItemSprite();
                    if (craftingInventory.inventorySlots[i].amount > 1)
                        craftingGridUI.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Text>().text = craftingInventory.inventorySlots[i].amount.ToString();
                    else
                        craftingGridUI.transform.GetChild(i).GetChild(1).GetChild(1).GetComponent<Text>().text = "";
                }
                else
                    craftingGridUI.transform.GetChild(i).GetChild(1).gameObject.SetActive(false);
            }
        }
    }

    public void ShowInventory(Inventory inventory, CraftingInventory _craftingInventory)
    {
        //Inventory grid
        if (inventory != null)
        {
            inventoryUI.SetActive(true);
            inventoryBackDropUI.SetActive(true);
            inventoryToShow = inventory;
            numberOfRows = Mathf.CeilToInt(inventory.inventorySize / maxRowWidth);

            float rowHalf = maxRowWidth / 2;
            float rowPixelWidth = maxRowWidth * slotSpacing;
            float rowStartingPosition = -(rowPixelWidth / 2) - (slotSpacing / 2);
            float colStartingPosition = rowStartingPosition;

            int rowCount = 1;
            int colCount = 1;
            for (int i = 0; i < inventoryToShow.inventorySlots.Length; i++)
            {
                GameObject slot = Instantiate(slotPrefab, inventoryUI.transform);

                slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(rowStartingPosition + (slotSpacing * colCount), colStartingPosition + (slotSpacing * rowCount));

                if (colCount == maxRowWidth)
                {
                    rowCount++;
                    colCount = 0;
                }

                colCount++;
            }
        }

        //Crafting grid
        if (_craftingInventory != null)
        {
            craftingGridUI.SetActive(true);
            craftingInventory = _craftingInventory;

            float rowHalf = craftingInventory.inventorySize / 2;
            float rowPixelWidth = craftingInventory.gridSize * slotSpacing;
            float rowStartingPosition = -(rowPixelWidth / 2) - (slotSpacing / 2);
            float colStartingPosition = rowStartingPosition;

            int rowCount = 1;
            int colCount = 1;
            for (int i = 0; i < craftingInventory.inventorySlots.Length; i++)
            {
                GameObject slot = Instantiate(slotPrefab, craftingGridUI.transform);

                slot.GetComponent<RectTransform>().anchoredPosition = new Vector2(rowStartingPosition + (slotSpacing * colCount), colStartingPosition + (slotSpacing * rowCount));

                if (colCount == craftingInventory.gridSize)
                {
                    rowCount++;
                    colCount = 0;
                }

                colCount++;
            }
        }
        else
        {
            craftingGridUI.SetActive(false);
        }
    }

    public void TurnOffInventory()
    {
        inventoryUI.SetActive(false);
        inventoryBackDropUI.SetActive(false);
        inventoryToShow = null;
        for (int i = 0; i < inventoryUI.transform.childCount; i++)
            Destroy(inventoryUI.transform.GetChild(i).gameObject);

        craftingGridUI.SetActive(false);
        craftingInventory = null;
        for (int i = 0; i < craftingGridUI.transform.childCount; i++)
            Destroy(craftingGridUI.transform.GetChild(i).gameObject);
    }

    int GetSlotIndex(GameObject uiItemObject)
    {
        for (int i = 0; i < uiItemObject.transform.parent.parent.childCount; i++)
        {
            //Debug.Log(uiItemObject + "," + inventoryUI.transform.GetChild(i).GetChild(0).gameObject);
            if (uiItemObject.transform.parent.parent.GetChild(i).GetChild(0).gameObject == uiItemObject)
                return i;
        }

        return -1;
    }
}
