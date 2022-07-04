using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CobblestoneBlock : Block
{
    public CobblestoneBlock()
    {
        blockType = BlockUtils.BlockType.Cobblestone;
        blockMaterial = (Material)Resources.Load("Blocks/Materials/Cobblestone");
        isOpaque = true;
        requiredDropTool = ToolItem.ToolType.Pickaxe;
        fastBreakTool = ToolItem.ToolType.Pickaxe;
        durabilty = 6;
        itemToDrop = typeof(CobblestoneItem);
        itemDropAmount = 1;
    }
}
