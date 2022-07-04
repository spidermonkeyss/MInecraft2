using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shovel : ToolItem
{
    public Shovel()
    {
        itemName = "Shovel";
        miningSpeed = 3.0f;
        toolType = ToolType.Shovel;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/default");
    }
}
