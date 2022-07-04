using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChunkHandler : MonoBehaviour
{
    public GameObject chunkParentObject;

    [SerializeField]
    Chunk sourceChunk;
   
    public int chunkLoadDistance;
    public int maxActiveChunks;
    public int maxActiveChunksBuffer;

    public static Chunk[] activeChunks;
    private static List<Chunk> chunkMeshsToLoad = new List<Chunk>();
    [SerializeField]
    private static int activeChunksCount = 0;

    public static bool chunkIsLoading;

    public bool renderChunksOneByOne;

    void Start()
    {
        if (chunkParentObject == null)
            chunkParentObject = GameObject.Find("Chunk Parent Object");

        PreAllocateChunks();

        SaveLoad.LoadWorldData("worldName");

        Vector2Int chunkPos = GetChunkPosFromWorldPos(0, 0);
        sourceChunk = LoadChunk(chunkPos.x, chunkPos.y);
        LoadChunksByDistance();
    }

    private void Update()
    {
        CheckSourceChunk();

        if (renderChunksOneByOne && chunkMeshsToLoad.Count > 0 && !chunkIsLoading)
            LoadNextChunkMesh();

        LoadChunksByDistance();
    }

    void PreAllocateChunks()
    {
        //Pre allocate chunk memory and reuse it
        maxActiveChunks = (chunkLoadDistance + chunkLoadDistance) * (chunkLoadDistance + chunkLoadDistance) + maxActiveChunksBuffer;
        activeChunks = new Chunk[maxActiveChunks];
        activeChunksCount = 0;

        for (int i = 0; i < maxActiveChunks; i++)
        {
            GameObject chunkObject = new GameObject();
            chunkObject.transform.SetParent(chunkParentObject.transform);
            chunkObject.name = "Chunk";
            chunkObject.AddComponent<Chunk>();
            chunkObject.GetComponent<Chunk>().isActive = false;
            activeChunks[i] = chunkObject.GetComponent<Chunk>();
        }
    }

    bool CheckSourceChunk()
    {
        Chunk chunk = GetChunkUsingWorldPos(transform.position.x, transform.position.z);

        if (chunk == null)
        {
            Debug.LogError("source chunk lost: " + Time.time);
            return false;
        }

        if (chunk == sourceChunk)
            return false;
        else
        {
            sourceChunk = chunk;
            return true;
        }
    }

    void LoadNextChunkMesh()
    {
        chunkIsLoading = true;
        chunkMeshsToLoad[0].GetComponent<Chunk>().OnChunkLoad();
        chunkMeshsToLoad.RemoveAt(0);
    }

    void LoadChunksByDistance()
    {
        Vector2 playerWorldPos = new Vector2(transform.position.x, transform.position.z);

        //All this is in chunkPos
        int rightBound = sourceChunk.chunkPosition.x + chunkLoadDistance;
        int leftBound = sourceChunk.chunkPosition.x - chunkLoadDistance;
        int topBound = sourceChunk.chunkPosition.y + chunkLoadDistance;
        int botBound = sourceChunk.chunkPosition.y - chunkLoadDistance;

        for (int x = leftBound; x <= rightBound; x++)
        {
            for (int z = botBound; z <= topBound; z++)
            {
                Vector2 chunkWorldPos = GetWorldPosFromChunkPos(x, z);
                float chunkWorldDistance = Vector2.Distance(chunkWorldPos, playerWorldPos);
                if (chunkWorldDistance <= chunkLoadDistance * Chunk.sizeOfChunk)
                {
                    //Check if chunk isnt alreadly loaded
                    Chunk chunk = GetChunkUsingChunkPos(x, z);
                    if (chunk == null)
                        LoadChunk(x, z);
                }
            }
        }

        //Unload chunks
        for (int i = activeChunksCount - 1; i >= 0; i--)
        {
            Chunk chunk = activeChunks[i];
            if (chunk.isActive)
            {
                Vector2 chunkWorldPos = new Vector2(chunk.transform.position.x, chunk.transform.position.z);
                float chunkWorldDistance = Vector2.Distance(chunkWorldPos, playerWorldPos);
                if (chunkWorldDistance > chunkLoadDistance * Chunk.sizeOfChunk)
                    UnloadChunk(i);
            }
        }
    }

    Chunk LoadChunk(int chunkX, int chunkY)
    {
        Chunk chunk = activeChunks[activeChunksCount];
        chunk.gameObject.layer = LayerMask.NameToLayer("Chunk");
        chunk.transform.position = new Vector3(chunkX * Chunk.sizeOfChunk + 7.5f, 0, chunkY * Chunk.sizeOfChunk + 7.5f);
        chunk.GetComponent<Chunk>().chunkPosition = new Vector2Int(chunkX, chunkY);
        chunk.isActive = true;

        if (renderChunksOneByOne)
            chunkMeshsToLoad.Add(chunk);
        else
            chunk.OnChunkLoad();

        activeChunksCount++;

        return chunk;
    }

    void UnloadChunk(int chunkIndex)
    {
        if (activeChunksCount == 0)
        {
            Debug.LogError("No active chunks");
            return;
        }

        chunkMeshsToLoad.Remove(activeChunks[chunkIndex]);

        Chunk tempChunk = activeChunks[chunkIndex];

        for (int i = chunkIndex; i < activeChunksCount - 1; i++)
            activeChunks[i] = activeChunks[i+1];

        activeChunks[activeChunksCount - 1] = tempChunk;

        activeChunksCount--;
        tempChunk.OnChunkUnload();
    }

    //Only returns loaded chunks
    //Parameters are in chunk position not world position
    public static Chunk GetChunkUsingChunkPos(int x, int z)
    {
        for (int i = 0; i < activeChunks.Length; i++)
        {
            if (activeChunks[i].isActive && activeChunks[i].chunkPosition.x == x && activeChunks[i].chunkPosition.y == z)
                return activeChunks[i];
        }
        return null;
    }
    public static Chunk GetChunkUsingChunkPos(Vector2Int chunkPosition)
    {
        for (int i = 0; i < activeChunks.Length; i++)
        {
            if (activeChunks[i].isActive && activeChunks[i].chunkPosition == chunkPosition)
                return activeChunks[i];
        }
        return null;
    }

    //Only returns loaded chunks
    //Parameters are in world position
    public static Chunk GetChunkUsingWorldPos(float x, float z)
    {
        Vector2Int chunkPos = GetChunkPosFromWorldPos(x, z);
        Chunk chunk = GetChunkUsingChunkPos(chunkPos.x, chunkPos.y);
        return chunk;
    }

    //Input parameters in chunk pos
    public static Vector2 GetWorldPosFromChunkPos(int x, int z)
    {
        float worldX = x * Chunk.sizeOfChunk + 7.5f;
        float worldZ = z * Chunk.sizeOfChunk + 7.5f;

        return new Vector2(worldX, worldZ);
    }

    //Input parameters in world pos
    public static Vector2Int GetChunkPosFromWorldPos(float x, float z)
    {
        //int posOrNegX = (int)(x / Mathf.Abs(x));
        //int posOrNegZ = (int)(z / Mathf.Abs(z));
        //
        //int chunkX = (int)((Mathf.Abs(x) + Chunk.sizeOfChunk / 2) / Chunk.sizeOfChunk);
        //int chunkZ = (int)((Mathf.Abs(z) + Chunk.sizeOfChunk / 2) / Chunk.sizeOfChunk);
        //
        //chunkX *= posOrNegX;
        //chunkZ *= posOrNegZ;

        float chunkX = (x - 8) / 16;
        float chunkZ = (z - 8) / 16;

        return new Vector2Int(Mathf.RoundToInt(chunkX), Mathf.RoundToInt(chunkZ));
    }

    //returns loaded neighboring chunks
    public static List<Chunk> GetNeighborChunks(Vector2Int chunkPosition)
    {
        List<Chunk> chunks = new List<Chunk>(4);
        //North chunk
        chunks.Add(GetChunkUsingChunkPos(new Vector2Int(chunkPosition.x, chunkPosition.y + 1)));
        //East chunk
        chunks.Add(GetChunkUsingChunkPos(new Vector2Int(chunkPosition.x + 1, chunkPosition.y)));
        //South chunk
        chunks.Add(GetChunkUsingChunkPos(new Vector2Int(chunkPosition.x, chunkPosition.y - 1)));
        //West chunk
        chunks.Add(GetChunkUsingChunkPos(new Vector2Int(chunkPosition.x - 1, chunkPosition.y)));

        return chunks;
    }
}
