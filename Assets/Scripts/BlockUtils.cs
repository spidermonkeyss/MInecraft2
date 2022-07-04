using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BlockUtils
{
    //Raw block data that is stored in chunks
    [System.Serializable]
    public struct BlockData
    {
        public BlockType blockType;
        public Vector3 localChunkBlockPosition;
    }

    //Make all opaque blocks greater than cube, and all transpanent less then cube
    public enum BlockType {
        Air,
        Null,
        Grass,
        Cobblestone,
        Wood,
        Wood_Plank,
        Crafting_Table
    };
    public const int MINIMUM_OPAQUE_BLOCK_NUMBER = (int)BlockType.Null;

    static public void BreakBlock(Chunk chunk, int blockIndex, BlockType blockType, Item itemBrokenWith)
    {
        //If tool is right, drop items
        if (GetBlockInstanceFromBlocktype(chunk.blocks[blockIndex].blockType).requiredDropTool == ToolItem.ToolType.Null)
            GetBlockInstanceFromBlocktype(chunk.blocks[blockIndex].blockType).DropItems(chunk, blockIndex);
        else if (itemBrokenWith != null && itemBrokenWith.GetType().IsSubclassOf(typeof(ToolItem)))
        {
            if (GetBlockInstanceFromBlocktype(chunk.blocks[blockIndex].blockType).requiredDropTool == ((ToolItem)itemBrokenWith).toolType)
                GetBlockInstanceFromBlocktype(chunk.blocks[blockIndex].blockType).DropItems(chunk, blockIndex);
        }

        ChangeBlock(chunk, blockIndex, blockType);
    }

    static public void ChangeBlock(Chunk chunk, int blockIndex, BlockType blockType)
    {
        chunk.blocks[blockIndex].blockType = blockType;
        if (!chunk.changedBlockIndexes.Contains(blockIndex))
            chunk.changedBlockIndexes.Add(blockIndex);

        chunk.UpdateChunkFull();

        if (chunk.blocks[blockIndex].localChunkBlockPosition.z == Chunk.sizeOfChunk - 1)
            chunk.neighborChunks[0].UpdateChunkFull();
        if (chunk.blocks[blockIndex].localChunkBlockPosition.x == Chunk.sizeOfChunk - 1)
            chunk.neighborChunks[1].UpdateChunkFull(); 
        if (chunk.blocks[blockIndex].localChunkBlockPosition.z == 0)
            chunk.neighborChunks[2].UpdateChunkFull(); 
        if (chunk.blocks[blockIndex].localChunkBlockPosition.x == 0)
            chunk.neighborChunks[3].UpdateChunkFull();
    }
  
    static public Vector3 Get_LocalChunkBlockPosition(Vector3 worldBlockPosition)
    {
        int x = (int)worldBlockPosition.x % Chunk.sizeOfChunk;
        if (x < 0)
            x = 16 - Mathf.Abs(x);
        int z = (int)worldBlockPosition.z % Chunk.sizeOfChunk;
        if (z < 0)
            z = 16 - Mathf.Abs(z);
        Vector3 local = new Vector3(x, worldBlockPosition.y, z);

        return local;
    }

    static public Vector3 Get_WorldBlockPosition(Vector3 localChunkBlockPosition, Vector2Int chunkPosition)
    {
        float x = localChunkBlockPosition.x + (Chunk.sizeOfChunk * chunkPosition.x);
        float z = localChunkBlockPosition.z + (Chunk.sizeOfChunk * chunkPosition.y);

        Vector3 blockPos = new Vector3(x, localChunkBlockPosition.y, z);
        return blockPos;
    }

    static public Vector3 GetBlockPosition(Vector3 worldPosition)
    {
        float x = Mathf.Round(worldPosition.x);
        float y = Mathf.Round(worldPosition.y);
        float z = Mathf.Round(worldPosition.z);

        Vector3 blockPos = new Vector3(x, y, z);
        return blockPos;
    }

    //static public Material GetBlockMaterial(BlockType blockType)
    //{
    //    return LoadResources.blockMaterialsList[(int)blockType];
    //}

    public static Block GetBlockInstanceFromBlocktype(BlockType blockType)
    {
        for (int i = 0; i < Block.GetBlockInstances().Count; i++)
        {
            if (Block.GetBlockInstances()[i].blockType == blockType)
                return Block.GetBlockInstances()[i];
        }
        return null;
    }
}