using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stick : Item
{
    public Stick()
    {
        itemName = "Stick";
        maxStackSize = 64;
        ItemTexture = (Texture2D)Resources.Load("Items/Textures/Stick");
    }
    
}
