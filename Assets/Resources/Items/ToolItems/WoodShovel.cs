using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodShovel : Shovel
{
    public WoodShovel()
    {
        itemName = "Wood Shovel";
        miningSpeed = 3.0f;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/wood_shovel");
    }
}
