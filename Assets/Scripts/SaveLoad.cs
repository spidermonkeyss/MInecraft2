using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;

public static class SaveLoad
{
    public static WorldData worldData = new WorldData();

    public static void SaveWorldData(string worldName)
    {
        BinaryFormatter bf = new BinaryFormatter();
        string path = Application.persistentDataPath + "/" + worldName + ".world";
        FileStream stream = new FileStream(path, FileMode.Create);

        bf.Serialize(stream, worldData);
        stream.Close();

        Debug.Log("World: " + worldName + " saved");
    }

    public static void LoadWorldData(string worldName)
    {
        string path = Application.persistentDataPath + "/" + worldName + ".world";
        if (File.Exists(path))
        {
            BinaryFormatter bf = new BinaryFormatter();
            FileStream stream = new FileStream(path, FileMode.Open);

            if (stream.Length == 0)
                worldData = new WorldData();
            else
            {
                worldData = bf.Deserialize(stream) as WorldData;
                Debug.Log("World: " + worldName + " loaded");
            }

            stream.Close();
        }
        else
        {
            Debug.LogError("World File not found: " + path);
            worldData = new WorldData();
        }
    }

    public static void UpdateWorldDataWithChunk(Chunk chunk)
    {
        if (!IsChunkInWorldData(chunk, true))
            AddChunkToWorldData(chunk);
    }

    public static void AddChunkToWorldData(Chunk chunk)
    {
        ChunkData chunkData = new ChunkData();
        chunkData.chunkPositionX = chunk.chunkPosition.x;
        chunkData.chunkPositionZ = chunk.chunkPosition.y;

        //Changed blocks
        chunkData.blockIndexs = new int[chunk.changedBlockIndexes.Count];
        chunkData.blockTypes = new int[chunk.changedBlockIndexes.Count];
        for (int i = 0; i < chunk.changedBlockIndexes.Count; i++)
        {
            chunkData.blockIndexs[i] = chunk.changedBlockIndexes[i];
            chunkData.blockTypes[i] = (int)chunk.blocks[chunk.changedBlockIndexes[i]].blockType;
        }

        //Increase array size by one
        ChunkData[] newWorldChunks = new ChunkData[worldData.chunkData.Length + 1];
        for (int i = 0; i < worldData.chunkData.Length; i++)
        {
            newWorldChunks[i] = worldData.chunkData[i];
        }
        newWorldChunks[worldData.chunkData.Length] = chunkData;
        worldData.chunkData = newWorldChunks;

        Debug.Log("Chunk Added:" + chunk.chunkPosition);
    }

    public static bool IsChunkInWorldData(Chunk chunk, bool updateChunk)
    {
        for (int i = 0; i < worldData.chunkData.Length; i++)
        {
            if (worldData.chunkData[i].chunkPositionX == chunk.chunkPosition.x && worldData.chunkData[i].chunkPositionZ == chunk.chunkPosition.y)
            {
                if (updateChunk)
                {
                    //Changed blocks
                    worldData.chunkData[i].blockIndexs = new int[chunk.changedBlockIndexes.Count];
                    worldData.chunkData[i].blockTypes = new int[chunk.changedBlockIndexes.Count];
                    for (int k = 0; k < chunk.changedBlockIndexes.Count; k++)
                    {
                        worldData.chunkData[i].blockIndexs[k] = chunk.changedBlockIndexes[k];
                        worldData.chunkData[i].blockTypes[k] = (int)chunk.blocks[chunk.changedBlockIndexes[k]].blockType;
                    }
                }
                return true;
            }
        }
        return false;
    }

    public static ChunkData GetChunkData(float chunkPosX, float chunkPosZ)
    {
        for (int i = 0; i < worldData.chunkData.Length; i++)
        {
            if (worldData.chunkData[i].chunkPositionX == chunkPosX && worldData.chunkData[i].chunkPositionZ == chunkPosZ)
                return worldData.chunkData[i];
        }
        return null;
    }
}


[System.Serializable]
public class WorldData
{
    public int seed;
    public ChunkData[] chunkData;

    public WorldData()
    {
        seed = 10000;
        chunkData = new ChunkData[0];
    }

    public WorldData(ChunkData[] _chunkData)
    {
        seed = 10000;
        chunkData = _chunkData;
    }
}

[System.Serializable]
public class ChunkData
{
    public int chunkPositionX;
    public int chunkPositionZ;
    public int[] blockTypes = new int[0];
    public int[] blockIndexs = new int[0];
}

