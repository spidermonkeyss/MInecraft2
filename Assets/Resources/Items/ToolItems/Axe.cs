using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Axe : ToolItem
{
    public Axe()
    {
        itemName = "Axe";
        miningSpeed = 3.0f;
        toolType = ToolType.Axe;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/default");
    }
}
