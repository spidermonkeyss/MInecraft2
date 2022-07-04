using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneShovel : Shovel
{
    public StoneShovel()
    {
        itemName = "Stone Shovel";
        miningSpeed = 5.0f;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/stone_shovel");
    }
}
