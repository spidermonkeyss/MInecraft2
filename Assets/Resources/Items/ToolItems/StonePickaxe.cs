using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StonePickaxe : Pickaxe
{
    public StonePickaxe()
    {
        itemName = "Stone Pickaxe";
        miningSpeed = 5.0f;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/stone_pickaxe");
    }
}
