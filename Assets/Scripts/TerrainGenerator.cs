using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGenerator
{
    public static int seed;
    static int minimumYHieght = 40;
    static int maximumYHeight = 70;
    static float noiseScale = 150;

    static int dirtLayerHeight = 3;
    static int treeHeight = 4;
    static int treeSpawnChance = 90;

    static float[,] surfaceBlock = new float[Chunk.sizeOfChunk, Chunk.sizeOfChunk];
    static List<Vector3> treeSpawnBlock = new List<Vector3>();

    public static void PopulateChunkBlocks(Chunk chunk, ChunkData chunkData)
    {
        GenerateChunkTerrain(chunk, chunkData);
        GenerateChunkTrees(chunk, chunkData);
        LoadChunkChangedBlocks(chunk, chunkData);

        ChunkHandler.chunkIsLoading = false;
        chunk.isBlocksLoaded = true;
    }

    static void GenerateChunkTerrain(Chunk chunk, ChunkData chunkData)
    {
        seed = SaveLoad.worldData.seed;
        System.Random random = new System.Random(chunk.chunkPosition.x + chunk.chunkPosition.y + seed);
        treeSpawnBlock.Clear();

        for (int z = 0; z < Chunk.sizeOfChunk; z++)
        {
            for (int x = 0; x < Chunk.sizeOfChunk; x++)
            {
                float yHeight = GenerateNoiseAtCoordinate(x + chunk.transform.position.x, z + chunk.transform.position.z) * (maximumYHeight - minimumYHieght) + minimumYHieght;
                yHeight = Mathf.RoundToInt(yHeight);
                surfaceBlock[x, z] = yHeight;
            }
        }

        Vector3 localChunkBlockPos;
        int index = 0;
        //float startTime = Time.realtimeSinceStartup;
        //float endTime;
        for (int y = 0; y < Chunk.chunkHeight; y++)
        {
            for (int z = 0; z < Chunk.sizeOfChunk; z++)
            {
                for (int x = 0; x < Chunk.sizeOfChunk; x++)
                {
                    localChunkBlockPos = new Vector3(x, y, z);

                    if (y > surfaceBlock[x, z])
                        chunk.blocks[index].blockType = BlockUtils.BlockType.Air;
                    else
                    {
                        if (y > surfaceBlock[x, z] - dirtLayerHeight)
                            chunk.blocks[index].blockType = BlockUtils.BlockType.Grass;
                        else
                            chunk.blocks[index].blockType = BlockUtils.BlockType.Cobblestone;
                    }
                    chunk.blocks[index].localChunkBlockPosition = localChunkBlockPos;

                    //Check for tree
                    if (y == surfaceBlock[x,z])
                    {
                        int treeValue = random.Next(0, treeSpawnChance);
                        if (treeValue == 0)
                            treeSpawnBlock.Add(localChunkBlockPos);
                    }

                    index++;
                }
            }
            //endTime = Time.realtimeSinceStartup;
            //if ((endTime - startTime) * 1000 >= 6.0f)
            //{
            //    //yield return null;
            //    startTime = Time.realtimeSinceStartup;
            //}
        }
        //yield return null;
    }

    static void GenerateChunkTrees(Chunk chunk, ChunkData chunkData)
    {
        for (int i = 0; i < treeSpawnBlock.Count; i++)
        {
            for (int k = 0; k < treeHeight; k++)
            {
                int index = Chunk.GetIndexOfBlockUsingLocalPosition((int)treeSpawnBlock[i].x, (int)treeSpawnBlock[i].y + k + 1, (int)treeSpawnBlock[i].z);
                chunk.blocks[index].blockType = BlockUtils.BlockType.Wood;
            }
        }
    }

    static void LoadChunkChangedBlocks(Chunk chunk, ChunkData chunkData)
    {
        for (int i = 0; i < chunk.changedBlockIndexes.Count; i++)
        {
            chunk.blocks[chunk.changedBlockIndexes[i]].blockType = (BlockUtils.BlockType)chunkData.blockTypes[i];
        }
    }

    static float GenerateNoiseAtCoordinate(float x, float z)
    {
        float noiseValue = Mathf.PerlinNoise((x / noiseScale) + seed, (z / noiseScale) + seed);
        return noiseValue;
    }
}
