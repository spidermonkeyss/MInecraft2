using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrassBlock : Block
{
    public GrassBlock()
    {
        blockType = BlockUtils.BlockType.Grass;
        blockMaterial = (Material)Resources.Load("Blocks/Materials/Grass");
        isOpaque = true;
        requiredDropTool = ToolItem.ToolType.Null;
        fastBreakTool = ToolItem.ToolType.Shovel;
        durabilty = 1.5f;
        itemToDrop = typeof(GrassItem);
        itemDropAmount = 1;
    }
}
