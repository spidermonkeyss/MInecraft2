using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Cuboid : MonoBehaviour
{
    public Vector3Int startingPosition;
    public Vector3Int endingPosition;
    public List<BlockUtils> blocks = new List<BlockUtils>();
}
