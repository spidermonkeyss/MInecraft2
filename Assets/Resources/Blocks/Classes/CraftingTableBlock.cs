using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingTableBlock : Block
{
    public CraftingInventory craftingInventory;

    public CraftingTableBlock()
    {
        blockType = BlockUtils.BlockType.Crafting_Table;
        blockMaterial = (Material)Resources.Load("Blocks/Materials/Crafting_Table");
        isOpaque = true;
        requiredDropTool = ToolItem.ToolType.Null;
        fastBreakTool = ToolItem.ToolType.Axe;
        durabilty = 4;
        itemToDrop = typeof(CraftingTableItem);
        itemDropAmount = 1;
        craftingInventory = new CraftingInventory(9, false);
    }

    public override void OnRightClick(GameObject player)
    {
        base.OnRightClick(player);
        Debug.Log("Right Click On:" + this.GetType().ToString());
        player.GetComponent<PlayerController>().ToggleInventory(craftingInventory);
    }
}
