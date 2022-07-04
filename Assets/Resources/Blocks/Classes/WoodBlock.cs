using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodBlock : Block
{
    public WoodBlock()
    {
        blockType = BlockUtils.BlockType.Wood;
        blockMaterial = (Material)Resources.Load("Blocks/Materials/Wood");
        isOpaque = true;
        requiredDropTool = ToolItem.ToolType.Null;
        fastBreakTool = ToolItem.ToolType.Axe;
        durabilty = 4;
        itemToDrop = typeof(WoodItem);
        itemDropAmount = 1;
    }
}
