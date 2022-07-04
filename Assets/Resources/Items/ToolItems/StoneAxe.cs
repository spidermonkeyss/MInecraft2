using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneAxe : Axe
{
    public StoneAxe()
    {
        itemName = "Stone Axe";
        miningSpeed = 5.0f;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/stone_axe");
    }
}
