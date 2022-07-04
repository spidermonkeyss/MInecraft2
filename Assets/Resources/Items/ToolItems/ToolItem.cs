using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolItem : Item
{
    public float miningSpeed;
    public enum ToolType { Null, Axe, Shovel, Pickaxe, Sword }
    public ToolType toolType;

    public ToolItem()
    {
        maxStackSize = 1;
    }

    protected virtual bool DoesToolBreakBlock(BlockUtils.BlockType blockType)
    {
        bool isToolRight = BlockUtils.GetBlockInstanceFromBlocktype(blockType).requiredDropTool == this.toolType;
        return isToolRight;
    }
}
