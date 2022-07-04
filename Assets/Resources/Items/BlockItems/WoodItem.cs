using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodItem : BlockItem
{
    public WoodItem()
    {
        itemName = "Wood";
        maxStackSize = 64;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/Wood");
    }

    public override void OnRightClick(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {
        base.OnRightClick(selectedBlock, selectedFaceNormal, selectedChunk, itemOwner, slotIndex);
        InventoryItemToBlock(selectedBlock, selectedFaceNormal, selectedChunk, BlockUtils.BlockType.Wood, itemOwner, slotIndex);
    }
}
