using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineOfCubes : MonoBehaviour
{
    public List<BlockUtils> blockList = new List<BlockUtils>();
    public List<Cuboid> cuboids = new List<Cuboid>();
    public List<BlockUtils> airBlockList = new List<BlockUtils>();

    public Vector3Int deminsions;
    public int rngCutoff;
    public bool useRandomGen;
    public Material cubeMaterial;

    public List<BlockUtils> blockPrefabs = new List<BlockUtils>();

    void Start()
    {
        int i = 0;
        for (int y = 0; y < deminsions.y; y++)
        {
            for (int z = 0; z < deminsions.z; z++)
            {
                for (int x = 0; x < deminsions.x; x++)
                {
                    Vector3 blockPos = new Vector3(x, y, z);

                    //random block
                    //if (useRandomGen)
                    //{
                    //    int rng = Random.Range(0, 100);
                    //    if (rng <= rngCutoff)
                    //        blockList.Add(Block.SpawnBlock(Block.BlockType.Cube, blockPos));
                    //    else
                    //        blockList.Add(Block.SpawnBlock(Block.BlockType.Air, blockPos));
                    //}
                    //else
                        //blockList.Add(Block.SpawnBlock(blockPrefabs[i].name, blockPos).GetComponent<Block>());

                    //if (blockList[blockList.Count - 1].name == "Air")
                        //airBlockList.Add(blockList[blockList.Count - 1]);
                    i++;
                }
            }
        }

        GenerateCuboids();
        GenerateCuboidParentObjects();
        GenerateAirParentObject();
        GenerateMesh();
    }

    void GenerateCuboidsOLD()
    {
        /*
        cuboids = new List<Cuboid>();
        bool startNewCuboid = true;
        for (int i = 0; i < blockList.Count; i++)
        {
            //Check if air block
            if (blockList[i].isOpaque)
                startNewCuboid = true;
            else
            {
                //Create cuboid
                if (startNewCuboid)
                {
                    startNewCuboid = false;
                    GameObject cuboidGO = new GameObject();
                    cuboidGO.name = "Cuboid";
                    cuboidGO.AddComponent<Cuboid>();
                    cuboids.Add(cuboidGO.GetComponent<Cuboid>());
                }

                //Add block to cuboid
                int currentCuboidIndex = cuboids.Count - 1;
                cuboids[currentCuboidIndex].blocks.Add(blockList[i]);

                //Check if end of row
                if ((i + 1) % 16 == 0)
                {
                    Debug.Log(blockList[i].blockPosition);
                    //if the cuboid didnt start at the first block in the row, it needs to finish at the end of the row
                    //Check if cuboid started at beginning of row
                    //if not then new cuboid ends
                    if ((cuboids[currentCuboidIndex].blocks[0].blockPosition.x) % 16 != 0)
                        startNewCuboid = true;
                }
            }
        }
        */
    }

    void GenerateCuboids()
    {
        /*
        cuboids = new List<Cuboid>();
        //Reset blocks in cuboid
        for (int i = 0; i < blockList.Count; i++)
            blockList[i].inCuboid = false;

        for (int i = 0; i < blockList.Count; i++)
        {
            //Check if already in cuboid
            if (blockList[i].inCuboid)
                continue;

            //Check if air block
            if (blockList[i].isAir)
                continue;

            //Create cuboid
            GameObject cuboidGO = new GameObject();
            cuboidGO.name = "Cuboid";
            cuboidGO.AddComponent<Cuboid>();
            cuboids.Add(cuboidGO.GetComponent<Cuboid>());
            Cuboid cuboid = cuboidGO.GetComponent<Cuboid>();

            cuboid.startingPosition = blockList[i].localChunkBlockPosition;

            //Find cuboid dimensions
            Vector3Int bounds = deminsions - blockList[i].localChunkBlockPosition;
            for (int y = 0; y < bounds.y; y++)
            {
                for (int z = 0; z < bounds.z; z++)
                {
                    for (int x = 0; x < bounds.x; x++)
                    {
                        //Get index of block to check
                        int xa = blockList[i].localChunkBlockPosition.x + x;
                        int ya = blockList[i].localChunkBlockPosition.y + y;
                        int za = blockList[i].localChunkBlockPosition.z + z;
                        Vector3Int blockToCheck = new Vector3Int(xa, ya, za);
                        int index = GetIndexOfBlockListUsingLocalChunkBlockPosition(blockToCheck);

                        //If block is outside of chunk no index exists
                        if (index == -1)
                            break;

                        if (index >= blockList.Count)
                            Debug.Log("Index is bigger than blockList count");

                        //This is the starting block
                        if (index == i)
                            continue;

                        //Check for end of axis and set new bounds
                        //If block is in a cuboid already
                        if (blockList[index].inCuboid)
                        {
                            if (x == 0)
                            {
                                bounds.z = z;
                                x = bounds.x;
                            }
                            else
                                bounds.x = x;
                        }
                        //If block is air
                        else if (blockList[index].isAir)
                        {
                            if (x == 0)
                            {
                                bounds.z = z;
                                x = bounds.x;
                            }
                            else
                                bounds.x = x;
                        }
                        else
                        {
                            //If block is end of chunk
                            if (blockToCheck.x == deminsions.x - 1)
                                bounds.x = x + 1;
                            if (blockToCheck.z == deminsions.z - 1)
                                bounds.z = z + 1;
                            if (blockToCheck.y == deminsions.y - 1)
                                bounds.y = y + 1;
                        }
                    }
                }
            }

            cuboid.endingPosition = new Vector3Int(cuboid.startingPosition.x + bounds.x - 1, cuboid.startingPosition.y + bounds.y - 1, cuboid.startingPosition.z + bounds.z - 1);
            //Populate cuboid
            for (int y = cuboid.startingPosition.y; y <= cuboid.endingPosition.y; y++)
            {
                for (int z = cuboid.startingPosition.z; z <= cuboid.endingPosition.z; z++)
                {
                    for (int x = cuboid.startingPosition.x; x <= cuboid.endingPosition.x; x++)
                    {
                        Vector3Int blockLocalPosition = new Vector3Int(x, y, z);
                        int index = GetIndexOfBlockListUsingLocalChunkBlockPosition(blockLocalPosition);
                        cuboidGO.GetComponent<Cuboid>().blocks.Add(blockList[index]);
                        blockList[index].inCuboid = true;
                    }
                }
            }
        }*/
    }

    void GenerateCuboidParentObjects()
    {
        /*
        for (int i = 0; i < cuboids.Count; i++)
        {
            for (int k = 0; k < cuboids[i].blocks.Count; k++)
                cuboids[i].blocks[k].transform.SetParent(cuboids[i].transform);
        }
        */
    }

    void GenerateAirParentObject()
    {
        //GameObject airBlocksParentObject = new GameObject();
        //airBlocksParentObject.name = "Air Block Parent Object";
        //foreach(Block airBlock in airBlockList)
            //airBlock.transform.SetParent(airBlocksParentObject.transform);
    }

    void GenerateMesh()
    {
        for (int i = 0; i < cuboids.Count; i++)
            GenerateCuboidMesh(cuboids[i]);
    }

    void GenerateCuboidMesh(Cuboid cuboid)
    {
        Vector3 bottomLeftVertices = new Vector3(cuboid.GetComponent<Cuboid>().startingPosition.x - 0.5f, cuboid.GetComponent<Cuboid>().startingPosition.y - 0.5f, cuboid.GetComponent<Cuboid>().startingPosition.x - 0.5f);
        Vector3 topRightVertices = new Vector3(cuboid.GetComponent<Cuboid>().endingPosition.x + 0.5f, cuboid.GetComponent<Cuboid>().endingPosition.y + 0.5f, cuboid.GetComponent<Cuboid>().endingPosition.z + 0.5f);
        
        List<Vector3> verticesList = new List<Vector3>();
        //Front face vertices
        for (float y = bottomLeftVertices.y; y < topRightVertices.y; y++)
        {
            for (float x = bottomLeftVertices.x; x < topRightVertices.x; x++)
                verticesList.Add(new Vector3(x, y, bottomLeftVertices.z));
        }
        //Right face vertices
        for (float y = bottomLeftVertices.y; y < topRightVertices.y; y++)
        {
            for (float z = bottomLeftVertices.z; z < topRightVertices.z; z++)
                verticesList.Add(new Vector3(bottomLeftVertices.x, y, z));
        }
        //Back face vertices
        for (float y = bottomLeftVertices.y; y < topRightVertices.y; y++)
        {
            for (float x = topRightVertices.x -1 ; x >= bottomLeftVertices.x; x--)
                verticesList.Add(new Vector3(x, y, topRightVertices.z));
        }
        //Left face vertices
        for (float y = bottomLeftVertices.y; y < topRightVertices.y; y++)
        {
            for (float z = topRightVertices.z - 1; z >= bottomLeftVertices.z; z--)
                verticesList.Add(new Vector3(topRightVertices.x, y, z));
        }
        //top face vertices
        for (float z = bottomLeftVertices.z; z < topRightVertices.z; z++)
        {
            for (float x = bottomLeftVertices.x; x < topRightVertices.x; x++)
                verticesList.Add(new Vector3(x, topRightVertices.y, z));
        }
        //bot face vertices
        for (float z = topRightVertices.z - 1; z >= bottomLeftVertices.z; z--)
        {
            for (float x = bottomLeftVertices.x; x < topRightVertices.x; x++)
                verticesList.Add(new Vector3(x, bottomLeftVertices.y, z));
        }

        List<int> trianglesList = new List<int>();
        for (int i = 0; i < verticesList.Count; i++)
        {
            trianglesList.Add(i);
            trianglesList.Add(i + 1);
            trianglesList.Add(i);
        }
        

        GameObject go = new GameObject();
        go.transform.position = new Vector3(0,5,0);
        go.AddComponent<MeshFilter>();
        go.AddComponent<MeshRenderer>();
        go.GetComponent<MeshRenderer>().sharedMaterial = cubeMaterial;

        Mesh mesh = new Mesh();
        mesh.name = "Meshss";
        mesh.vertices = verticesList.ToArray();
        mesh.triangles = trianglesList.ToArray();
        
        mesh.RecalculateNormals();

        go.GetComponent<MeshFilter>().mesh = mesh;
    }

    int GetIndexOfBlockListUsingLocalChunkBlockPosition(Vector3Int localChunkBlockPosition)
    {
        if (localChunkBlockPosition.x > deminsions.x - 1 || localChunkBlockPosition.z > deminsions.z - 1)
            return -1;

        int x = localChunkBlockPosition.x;
        int z = localChunkBlockPosition.z;
        int index = x + (z * deminsions.z);

        return index;
    }
}