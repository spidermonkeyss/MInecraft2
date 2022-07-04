using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTableItem : BlockItem
{
    public CraftingTableItem()
    {
        itemName = "Crafting Table";
        maxStackSize = 64;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/crafting_table");
    }

    public override void OnRightClick(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {
        base.OnRightClick(selectedBlock, selectedFaceNormal, selectedChunk, itemOwner, slotIndex);
        InventoryItemToBlock(selectedBlock, selectedFaceNormal, selectedChunk, BlockUtils.BlockType.Crafting_Table, itemOwner, slotIndex);
    }
}
