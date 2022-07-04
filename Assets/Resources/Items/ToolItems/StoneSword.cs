using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoneSword : Sword
{
    public StoneSword()
    {
        itemName = "Stone Sword";
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/stone_sword");
    }
}
