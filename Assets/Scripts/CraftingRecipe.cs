using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingRecipe
{
    private static List<CraftingRecipe> recipes = new List<CraftingRecipe>();

    private System.Type craftedItem;
    private int craftedAmount;
    public int CraftedAmount { get { return craftedAmount; } }
    private System.Type[] recipe;
    private bool _isOrdered;
    public bool isOrdered { get { return _isOrdered; } }

    private CraftingRecipe()
    {
        recipe = new System.Type[9];
    }

    public static void InitRecipes()
    {
        CraftingRecipe woodSword = CreateCraftingRecipe<WoodSword>(true, 1);
        woodSword.AddItemToRecipe<Stick>(1);
        woodSword.AddItemToRecipe<WoodPlankItem>(4);
        woodSword.AddItemToRecipe<WoodPlankItem>(7);
        AddRecipeToList(woodSword);

        CraftingRecipe woodPickaxe = CreateCraftingRecipe<WoodPickaxe>(true, 1);
        woodPickaxe.AddItemToRecipe<Stick>(1);
        woodPickaxe.AddItemToRecipe<Stick>(4);
        woodPickaxe.AddItemToRecipe<WoodPlankItem>(6);
        woodPickaxe.AddItemToRecipe<WoodPlankItem>(7);
        woodPickaxe.AddItemToRecipe<WoodPlankItem>(8);
        AddRecipeToList(woodPickaxe);

        CraftingRecipe woodAxe = CreateCraftingRecipe<WoodAxe>(true, 1);
        woodAxe.AddItemToRecipe<Stick>(1);
        woodAxe.AddItemToRecipe<Stick>(4);
        woodAxe.AddItemToRecipe<WoodPlankItem>(5);
        woodAxe.AddItemToRecipe<WoodPlankItem>(7);
        woodAxe.AddItemToRecipe<WoodPlankItem>(8);
        AddRecipeToList(woodAxe);

        CraftingRecipe woodShovel = CreateCraftingRecipe<WoodShovel>(true, 1);
        woodShovel.AddItemToRecipe<Stick>(1);
        woodShovel.AddItemToRecipe<Stick>(4);
        woodShovel.AddItemToRecipe<WoodPlankItem>(7);
        AddRecipeToList(woodShovel);

        CraftingRecipe woodPlank = CreateCraftingRecipe<WoodPlankItem>(true, 4);
        woodPlank.AddItemToRecipe<WoodItem>(0);
        AddRecipeToList(woodPlank);

        CraftingRecipe stick = CreateCraftingRecipe<Stick>(true, 4);
        stick.AddItemToRecipe<WoodPlankItem>(0);
        stick.AddItemToRecipe<WoodPlankItem>(2);
        AddRecipeToList(stick);

        CraftingRecipe craftingTable = CreateCraftingRecipe<CraftingTableItem>(true, 1);
        craftingTable.AddItemToRecipe<WoodPlankItem>(0);
        craftingTable.AddItemToRecipe<WoodPlankItem>(1);
        craftingTable.AddItemToRecipe<WoodPlankItem>(2);
        craftingTable.AddItemToRecipe<WoodPlankItem>(3);
        AddRecipeToList(craftingTable);

        CraftingRecipe stoneSword = CreateCraftingRecipe<StoneSword>(true, 1);
        stoneSword.AddItemToRecipe<Stick>(1);
        stoneSword.AddItemToRecipe<CobblestoneItem>(4);
        stoneSword.AddItemToRecipe<CobblestoneItem>(7);
        AddRecipeToList(stoneSword);

        CraftingRecipe stonePickaxe = CreateCraftingRecipe<StonePickaxe>(true, 1);
        stonePickaxe.AddItemToRecipe<Stick>(1);
        stonePickaxe.AddItemToRecipe<Stick>(4);
        stonePickaxe.AddItemToRecipe<CobblestoneItem>(6);
        stonePickaxe.AddItemToRecipe<CobblestoneItem>(7);
        stonePickaxe.AddItemToRecipe<CobblestoneItem>(8);
        AddRecipeToList(stonePickaxe);

        CraftingRecipe stoneAxe = CreateCraftingRecipe<StoneAxe>(true, 1);
        stoneAxe.AddItemToRecipe<Stick>(1);
        stoneAxe.AddItemToRecipe<Stick>(4);
        stoneAxe.AddItemToRecipe<CobblestoneItem>(5);
        stoneAxe.AddItemToRecipe<CobblestoneItem>(7);
        stoneAxe.AddItemToRecipe<CobblestoneItem>(8);
        AddRecipeToList(stoneAxe);

        CraftingRecipe stoneShovel = CreateCraftingRecipe<StoneShovel>(true, 1);
        stoneShovel.AddItemToRecipe<Stick>(1);
        stoneShovel.AddItemToRecipe<Stick>(4);
        stoneShovel.AddItemToRecipe<CobblestoneItem>(7);
        AddRecipeToList(stoneShovel);
    }

    void AddItemToRecipe<ItemType>(int slotIndex) where ItemType : Item
    {
        recipe[slotIndex] = System.Activator.CreateInstance<ItemType>().GetType();
    }

    static CraftingRecipe CreateCraftingRecipe<ItemType>(bool _isOrdered, int _craftedAmount) where ItemType : Item
    {
        CraftingRecipe recipe = new CraftingRecipe();
        recipe.craftedItem = System.Activator.CreateInstance<ItemType>().GetType();
        recipe._isOrdered = _isOrdered;
        recipe.craftedAmount = _craftedAmount;
        return recipe;
    }

    static void AddRecipeToList(CraftingRecipe recipe)
    {
        recipes.Add(recipe);
    }

    public static InventorySlot CheckRecipes(CraftingInventory craftingInventory)
    {
        for (int i = 0; i < recipes.Count; i++)
        {
            bool vaildRecipe = true;
            if (recipes[i].isOrdered)
            {
                for (int k = 0; k < craftingInventory.inventorySlots.Length - 1; k++)
                {
                    if (craftingInventory.inventorySlots[k].isEmpty)
                    {
                        if (recipes[i].recipe[k] != null)
                            vaildRecipe = false;
                    }
                    else
                    {
                        if (recipes[i].recipe[k] != craftingInventory.inventorySlots[k].item.GetType())
                            vaildRecipe = false;
                    }
                }
            }
            if (vaildRecipe)
            {
                Item item = (Item)System.Activator.CreateInstance(recipes[i].craftedItem);
                InventorySlot inventorySlot = new InventorySlot();
                inventorySlot.item = item;
                inventorySlot.amount = recipes[i].craftedAmount;
                inventorySlot.isEmpty = false;
                return inventorySlot;
            }
        }
        return null;
    }
}