using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sword : ToolItem
{
    public Sword()
    {
        itemName = "Sword";
        miningSpeed = 1;
        toolType = ToolType.Sword;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/default");
    }
}
