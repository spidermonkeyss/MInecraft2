using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WoodSword : Sword
{
    public WoodSword()
    {
        itemName = "Wood Sword";
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/wood_sword");
    }
}
