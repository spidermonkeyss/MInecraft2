using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    private static List<Block> blockInstanceList = new List<Block>();

    public BlockUtils.BlockType blockType;
    public Material blockMaterial;
    public bool isOpaque;
    public ToolItem.ToolType requiredDropTool;
    public ToolItem.ToolType fastBreakTool;
    public float durabilty;
    public System.Type itemToDrop;
    public int itemDropAmount;

    public static void InitBlocks()
    {
        AddInstance<WoodBlock>();
        AddInstance<GrassBlock>();
        AddInstance<CobblestoneBlock>();
        AddInstance<CraftingTableBlock>();
        AddInstance<WoodPlankBlock>();
    }

    static void AddInstance<BlockType>() where BlockType : Block
    {
        blockInstanceList.Add(System.Activator.CreateInstance<BlockType>());
    }

    public virtual void DropItems(Chunk chunk, int blockIndex)
    {
        for (int i = 0; i < itemDropAmount; i++)
           Item.SpawnItem(BlockUtils.Get_WorldBlockPosition(chunk.blocks[blockIndex].localChunkBlockPosition, chunk.chunkPosition), itemToDrop);
    }

    public virtual void OnRightClick(GameObject player)
    {

    }

    public static List<Block> GetBlockInstances()
    {
        return blockInstanceList;
    }
}