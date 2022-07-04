using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPlankItem : BlockItem
{
    public WoodPlankItem()
    {
        itemName = "Wood Plank";
        maxStackSize = 64;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/Wood_Plank");
    }

    public override void OnRightClick(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, Inventory itemOwner, int slotIndex)
    {
        base.OnRightClick(selectedBlock, selectedFaceNormal, selectedChunk, itemOwner, slotIndex);
        InventoryItemToBlock(selectedBlock, selectedFaceNormal, selectedChunk, BlockUtils.BlockType.Wood_Plank, itemOwner, slotIndex);
    }
}
