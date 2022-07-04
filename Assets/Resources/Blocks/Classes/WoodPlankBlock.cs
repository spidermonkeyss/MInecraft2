using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPlankBlock : Block
{
    public WoodPlankBlock()
    {
        blockType = BlockUtils.BlockType.Wood_Plank;
        blockMaterial = (Material)Resources.Load("Blocks/Materials/Wood_Plank");
        isOpaque = true;
        requiredDropTool = ToolItem.ToolType.Null;
        fastBreakTool = ToolItem.ToolType.Axe;
        durabilty = 4;
        itemToDrop = typeof(WoodPlankBlock);
        itemDropAmount = 1;
    }
}
