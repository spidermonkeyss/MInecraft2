using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobblestoneItem : BlockItem
{
    public CobblestoneItem()
    {
        itemName = "Cobblestone";
        maxStackSize = 64;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/Cobblestone");
    }

    public override void OnRightClick(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {
        base.OnRightClick(selectedBlock, selectedFaceNormal, selectedChunk, itemOwner, slotIndex);
        InventoryItemToBlock(selectedBlock, selectedFaceNormal, selectedChunk, BlockUtils.BlockType.Cobblestone, itemOwner, slotIndex);
    }
}

