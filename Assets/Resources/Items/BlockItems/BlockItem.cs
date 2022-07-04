using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockItem : Item
{
    protected void InventoryItemToBlock(BlockUtils.BlockData selectedBlock, Vector3 selectedFaceNormal, Chunk selectedChunk, BlockUtils.BlockType blockType, Inventory itemOwner, int slotIndex)
    {
        Vector3 worldBlockPos = BlockUtils.Get_WorldBlockPosition(selectedBlock.localChunkBlockPosition, selectedChunk.chunkPosition) + selectedFaceNormal;
        Vector3 localBlockPos = BlockUtils.Get_LocalChunkBlockPosition(worldBlockPos);
        int index = Chunk.GetIndexOfBlockUsingLocalPosition(localBlockPos);

        Chunk chunk = ChunkHandler.GetChunkUsingWorldPos(worldBlockPos.x, worldBlockPos.z);

        BlockUtils.ChangeBlock(chunk, index, blockType);

        if (itemOwner != null)
        {
            if (!itemOwner.isCreative)
                itemOwner.RemoveItemFromInventory(slotIndex);
        }
    }
}
