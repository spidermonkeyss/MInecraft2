using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadResources : MonoBehaviour
{
    [SerializeField]
    public static PhysicMaterial blockPhysicsMaterial;
   
    void Awake()
    {
        CraftingRecipe.InitRecipes();
        Block.InitBlocks();
        LoadPhysicMaterial();
    }
    
    void LoadPhysicMaterial()
    {
        blockPhysicsMaterial = (PhysicMaterial)Resources.Load("PhysicsMaterial/Block", typeof(PhysicMaterial));
    }
}
