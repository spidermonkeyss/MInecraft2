using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodAxe : Axe
{
    public WoodAxe()
    {
        itemName = "Wood Axe";
        miningSpeed = 3.0f;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/wood_axe");
    }
}
