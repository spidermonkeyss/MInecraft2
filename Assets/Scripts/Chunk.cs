using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public static PlayerController playerController;

    public static ComputeShader computeShader;
    public static int sizeOfChunk = 16;
    public static int chunkHeight = 256;
    public static int totalChunkSize = sizeOfChunk * sizeOfChunk * chunkHeight;
    static bool keepBufferLists = false;

    static int verticesPerBlock = 8;
    static int normalsPerBlock = 8;
    static int indiesPerBlock = 36;

    static int verticesSize = sizeof(float) * 3 * verticesPerBlock;
    static int normalsSize = sizeof(float) * 3 * normalsPerBlock;
    static int indicesSize = sizeof(float) * indiesPerBlock;
    static int subMeshIndexCountSize = sizeof(int);
    static int blockDataSize = sizeof(int) + (sizeof(float) * 3);

    //Have to set arrays to max possible sizes
    //Then cut out any useless data and resize
    static Vector3[] vertices = new Vector3[totalChunkSize * verticesPerBlock];
    static Vector3[] normals = new Vector3[totalChunkSize * normalsPerBlock];
    static int[] indices = new int[totalChunkSize * indiesPerBlock];
    static int[] subMeshIndexCounts = new int[totalChunkSize];

    static List<Vector3> verticesList = new List<Vector3>(totalChunkSize * verticesPerBlock);
    static List<Vector3> normalList = new List<Vector3>(totalChunkSize * normalsPerBlock);
    static List<int> indicesList = new List<int>(totalChunkSize * indiesPerBlock);
    static List<Material> materialsList = new List<Material>(totalChunkSize);
    static List<UnityEngine.Rendering.SubMeshDescriptor> subMeshDescriptors = new List<UnityEngine.Rendering.SubMeshDescriptor>(totalChunkSize);
    static List<int> activeBlockIndexes = new List<int>(totalChunkSize);

    public Vector2Int chunkPosition;
    public bool isBlocksLoaded;
    public bool isActive;
    public List<Chunk> neighborChunks = new List<Chunk>(4);

    public BlockUtils.BlockData[] blocks = new BlockUtils.BlockData[totalChunkSize];

    //Indexes of changed blocks
    public List<int> changedBlockIndexes = new List<int>();
    
    //These are blocks from neighboring chunks
    public BlockUtils.BlockData[] borderBlocks = new BlockUtils.BlockData[sizeOfChunk * chunkHeight * 4];

    public void OnChunkLoad()
    {
        isActive = true;
        isBlocksLoaded = false;
        neighborChunks = ChunkHandler.GetNeighborChunks(chunkPosition);

        ChunkData chunkData = SaveLoad.GetChunkData(chunkPosition.x, chunkPosition.y);
        if (chunkData != null)
        {
            for (int i = 0; i < chunkData.blockIndexs.Length; i++)
                changedBlockIndexes.Add(chunkData.blockIndexs[i]);
        }

        for (int i = 0; i < borderBlocks.Length; i++)
            borderBlocks[i].blockType = BlockUtils.BlockType.Null;
        
        TerrainGenerator.PopulateChunkBlocks(this, chunkData);
        GenerateChunkMesh();
        UpdateChunkNeighborsFull();
    }

    public void OnChunkUnload()
    {
        isActive = false;
        isBlocksLoaded = false;
        GetComponent<MeshFilter>().mesh.Clear();
        GetComponent<MeshRenderer>().enabled = false;
        GetComponent<MeshCollider>().enabled = false;
        transform.position = new Vector3();
        chunkPosition = new Vector2Int();
        UpdateChunkNeighborsHalf();
        neighborChunks.Clear();
        changedBlockIndexes.Clear();
        borderBlocks = new BlockUtils.BlockData[sizeOfChunk * chunkHeight * 4];
    }

    public void UpdateChunkHalf()
    {
        neighborChunks = ChunkHandler.GetNeighborChunks(chunkPosition);
    }

    public void UpdateChunkFull()
    {
        neighborChunks = ChunkHandler.GetNeighborChunks(chunkPosition);
        if (changedBlockIndexes.Count > 0)
            SaveLoad.UpdateWorldDataWithChunk(this);
        GenerateChunkMesh();
    }

    public void UpdateChunkNeighborsHalf()
    {
        for (int i = 0; i < neighborChunks.Capacity; i++)
        {
            if (neighborChunks[i] != null)
                neighborChunks[i].UpdateChunkHalf();
        }
    }

    public void UpdateChunkNeighborsFull()
    {
        for (int i = 0; i < neighborChunks.Capacity; i++)
        {
            if (neighborChunks[i] != null)
            {
                neighborChunks[i].UpdateChunkFull();
                //yield return null;
            }
        }
    }

    public void GenerateChunkMesh()
    {
        float startTime = Time.realtimeSinceStartup;

        if (!gameObject.GetComponent<MeshFilter>())
            gameObject.AddComponent<MeshFilter>();
        if (!gameObject.GetComponent<MeshRenderer>())
            gameObject.AddComponent<MeshRenderer>();

        Mesh mesh = new Mesh();
        mesh.name = "Chunk Mesh";

        GetComponent<MeshRenderer>().enabled = true;

        //Compute shader here
        //Return list of blocks indexs to render
        if (computeShader == null)
            computeShader = (ComputeShader)Resources.Load("ComputeShaders/GenerateChunkMesh", typeof(ComputeShader));

        ComputeBuffer verticesBuffer = new ComputeBuffer(blocks.Length, verticesSize);
        ComputeBuffer normalsBuffer = new ComputeBuffer(blocks.Length, normalsSize);
        ComputeBuffer indicesBuffer = new ComputeBuffer(blocks.Length, indicesSize);
        ComputeBuffer subMeshIndexCountBuffer = new ComputeBuffer(blocks.Length, subMeshIndexCountSize);

        ComputeBuffer blockInfoBuffer = new ComputeBuffer(blocks.Length, blockDataSize);
        blockInfoBuffer.SetData(blocks);

        //Setup border block buffers
        PopulateBlockBorderLists();
        ComputeBuffer borderBlocksBuffer = new ComputeBuffer(borderBlocks.Length, blockDataSize);
        borderBlocksBuffer.SetData(borderBlocks);

        int kernelIndex = computeShader.FindKernel("CSMain");
        computeShader.SetBuffer(kernelIndex, "vertices", verticesBuffer);
        computeShader.SetBuffer(kernelIndex, "normals", normalsBuffer);
        computeShader.SetBuffer(kernelIndex, "indices", indicesBuffer);
        computeShader.SetBuffer(kernelIndex, "subMeshIndexCount", subMeshIndexCountBuffer);

        computeShader.SetBuffer(kernelIndex, "blockInfo", blockInfoBuffer);
        computeShader.SetBuffer(kernelIndex, "borderBlockInfo", borderBlocksBuffer);

        computeShader.SetInt(Shader.PropertyToID("minimum_opaque_block_number"), BlockUtils.MINIMUM_OPAQUE_BLOCK_NUMBER);
        computeShader.SetInt(Shader.PropertyToID("sizeOfChunk"), sizeOfChunk);
        computeShader.SetInt(Shader.PropertyToID("chunkHeight"), chunkHeight);
        computeShader.SetInt(Shader.PropertyToID("blocksCount"), blocks.Length);

        computeShader.Dispatch(0, blocks.Length / 64, 1, 1);

        verticesBuffer.GetData(vertices);
        normalsBuffer.GetData(normals);
        indicesBuffer.GetData(indices);
        subMeshIndexCountBuffer.GetData(subMeshIndexCounts);

        float endTime = Time.realtimeSinceStartup;

        int blockRenderCount = 0;
        int totalIndexs = 0;
        activeBlockIndexes.Clear();
        for (int i = 0; i < totalChunkSize; i++)
        {
            if (subMeshIndexCounts[i] > 0)
            {
                for (int k = 0; k < 8; k++)
                {
                    verticesList.Add(vertices[i * 8 + k]);
                    normalList.Add(normals[i * 8 + k]);
                }
                for (int k = 0; k < subMeshIndexCounts[i]; k++)
                    indicesList.Add(indices[i * 36 + k]);

                blockRenderCount++;

                UnityEngine.Rendering.SubMeshDescriptor subMeshDescriptor = new UnityEngine.Rendering.SubMeshDescriptor();
                subMeshDescriptor.indexStart = totalIndexs;
                totalIndexs += subMeshIndexCounts[i];
                subMeshDescriptor.indexCount = subMeshIndexCounts[i];
                subMeshDescriptor.topology = MeshTopology.Triangles;
                int vertexOffset = (blockRenderCount - 1) * verticesPerBlock;
                subMeshDescriptor.baseVertex = vertexOffset;
                //subMeshDescriptor.firstVertex = 0;
                //subMeshDescriptor.vertexCount = 8;
                //Bounds bounds = new Bounds();
                //bounds.center = blocks[i].blockPosition;
                //bounds.extents = new Vector3(0.5f, 0.5f, 0.5f);
                //subMeshDescriptor.bounds = bounds;

                subMeshDescriptors.Add(subMeshDescriptor);

                activeBlockIndexes.Add(i);
            }
        }

        mesh.vertices = verticesList.ToArray();
        mesh.normals = normalList.ToArray();

        mesh.SetIndexBufferParams(indicesList.Count, UnityEngine.Rendering.IndexFormat.UInt32);
        mesh.SetIndexBufferData(indicesList.ToArray(), 0, 0, indicesList.Count);
        mesh.subMeshCount = blockRenderCount;

        //Set up mesh and material for each block
        for (int i = 0; i < mesh.subMeshCount; i++)
        {
            mesh.SetSubMesh(i, subMeshDescriptors[i]);
            int blockIndex = activeBlockIndexes[i];
            //materialsList.Add(BlockUtils.GetBlockMaterial(blocks[blockIndex].blockType));
            materialsList.Add(BlockUtils.GetBlockInstanceFromBlocktype(blocks[blockIndex].blockType).blockMaterial);
        }

        gameObject.GetComponent<MeshRenderer>().materials = materialsList.ToArray();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;

        verticesBuffer.Dispose();
        normalsBuffer.Dispose();
        indicesBuffer.Dispose();
        blockInfoBuffer.Dispose();
        borderBlocksBuffer.Dispose();
        
        subMeshIndexCountBuffer.Dispose();

        if (!keepBufferLists)
        {
            verticesList.Clear();
            normalList.Clear();
            indicesList.Clear();
            materialsList.Clear();
            subMeshDescriptors.Clear();
        }

        //Debug.Log("ChunkMeshGen: " + (endTime - startTime) * 1000);

        if (playerController == null)
            playerController = FindObjectOfType<PlayerController>();
        playerController.lastmeshGenText.text = ((endTime - startTime) * 1000).ToString();

        GenerateCollider();
    }

    void GenerateCollider()
    {
        if (gameObject.GetComponent<MeshCollider>())
            DestroyImmediate(GetComponent<MeshCollider>());
        gameObject.AddComponent<MeshCollider>();
        GetComponent<MeshCollider>().material = LoadResources.blockPhysicsMaterial;
        GetComponent<MeshCollider>().enabled = true;
    }

    void PopulateBlockBorderLists()
    {
        int x;
        int y;
        int z;
        int offset;

        //North neighbor
        z = 0;
        offset = 0;
        if (neighborChunks[0] != null)
        {
            for (y = 0; y < chunkHeight; y++)
            {
                for (x = 0; x < sizeOfChunk; x++)
                    borderBlocks[x + (y * sizeOfChunk) + offset] = neighborChunks[0].blocks[GetIndexOfBlockUsingLocalPosition(new Vector3(x, y, z))];
            }
        }

        //East
        x = 0;
        offset = chunkHeight * sizeOfChunk;
        if (neighborChunks[1] != null)
        {
            for (y = 0; y < chunkHeight; y++)
            {
                for (z = 0; z < sizeOfChunk; z++)
                    borderBlocks[z + (y * sizeOfChunk) + offset] = neighborChunks[1].blocks[GetIndexOfBlockUsingLocalPosition(new Vector3(x, y, z))];
            }
        }

        //South
        z = 15;
        offset = chunkHeight * sizeOfChunk * 2;
        if (neighborChunks[2] != null)
        {
            for (y = 0; y < chunkHeight; y++)
            {
                for (x = 0; x < sizeOfChunk; x++)
                    borderBlocks[x + (y * sizeOfChunk) + offset] = neighborChunks[2].blocks[GetIndexOfBlockUsingLocalPosition(new Vector3(x, y, z))];
            }
        }

        //West
        x = 15;
        offset = chunkHeight * sizeOfChunk * 3;
        if (neighborChunks[3] != null)
        {
            for (y = 0; y < chunkHeight; y++)
            {
                for (z = 0; z < sizeOfChunk; z++)
                    borderBlocks[z + (y * sizeOfChunk) + offset] = neighborChunks[3].blocks[GetIndexOfBlockUsingLocalPosition(new Vector3(x, y, z))];
            }
        }
    }

    public static int GetIndexOfBlockUsingLocalPosition(Vector3 localPosition)
    {
        if (localPosition.x < 0 || localPosition.x >= sizeOfChunk || localPosition.y < 0 || localPosition.y >= chunkHeight || localPosition.z < 0 || localPosition.z >= sizeOfChunk)
            return -1;

        int index = (int)(localPosition.x) + ((int)localPosition.z * sizeOfChunk) + ((int)localPosition.y * (sizeOfChunk * sizeOfChunk));

        return index;
    }

    public static int GetIndexOfBlockUsingLocalPosition(int x, int y, int z)
    {
        if (x < 0 || x >= sizeOfChunk || y < 0 || y >= chunkHeight || z < 0 || z >= sizeOfChunk)
            return -1;

        int index = x + (z * sizeOfChunk) + (y * (sizeOfChunk * sizeOfChunk));

        return index;
    }

    public static Vector3 GetLocalPositionUsingIndex(int index)
    {
        return new Vector3();
    }
}
