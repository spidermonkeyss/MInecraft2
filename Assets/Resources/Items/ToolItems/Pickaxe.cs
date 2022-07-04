using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickaxe : ToolItem
{
    public Pickaxe()
    {
        itemName = "Pickaxe";
        miningSpeed = 3.0f;
        toolType = ToolType.Pickaxe;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/default");
    }
}