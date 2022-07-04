using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodPickaxe : Pickaxe
{
    public WoodPickaxe()
    {
        itemName = "Wood Pickaxe";
        miningSpeed = 3.0f;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/wood_pickaxe");
    }
}
