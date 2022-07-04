using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class ChunkParentObject : MonoBehaviour
{
    public bool showDebugList;
    public List<Vector3> debug_verticesList = new List<Vector3>();
    public List<Vector3> debug_normalList = new List<Vector3>();
    public List<int> debug_indicesList = new List<int>();
    public List<Material> debug_materialsList = new List<Material>();
    public List<UnityEngine.Rendering.SubMeshDescriptor> debug_subMeshDescriptors = new List<UnityEngine.Rendering.SubMeshDescriptor>();

    //These are references to the lists in each chunk
    [SerializeField]
    public List<List<Vector3>> chunkVertices = new List<List<Vector3>>();
    [SerializeField]
    public List<List<Vector3>> chunkNormals = new List<List<Vector3>>();
    [SerializeField]
    public List<List<int>> chunkIndices = new List<List<int>>();
    [SerializeField]
    public List<List<Material>> chunkMaterials = new List<List<Material>>();
    [SerializeField]
    public List<List<UnityEngine.Rendering.SubMeshDescriptor>> chunkSubMeshDescriptors = new List<List<UnityEngine.Rendering.SubMeshDescriptor>>();

    Mesh mesh;

    private void Start()
    {
        mesh = new Mesh();
        gameObject.AddComponent<MeshFilter>();
        gameObject.AddComponent<MeshRenderer>();
        mesh.name = "Terrain Mesh";
        GetComponent<MeshFilter>().mesh = mesh;
    }

    //public void AddChunkMeshToTerrainMesh(Chunk chunk)
    //{
    //    for (int i = 0; i < chunk.verticesList.Count; i++)
    //        chunk.verticesList[i] += chunk.transform.position;
    //    for (int i = 0; i < chunk.subMeshDescriptors.Count; i++)
    //    {
    //        //chunk.subMeshDescriptors[i].indexStart = chunkIndices.SelectMany(x => x).ToArray().Length;
    //        //chunk.subMeshDescriptors[i].baseVertex = (8 * i) + chunkVertices.SelectMany(x => x).ToArray().Length;
    //    }
    //
    //    chunkVertices.Add(chunk.verticesList);
    //    chunkNormals.Add(chunk.normalList);
    //    chunkMaterials.Add(chunk.materialsList);
    //    
    //    chunkIndices.Add(chunk.indicesList);
    //    chunkSubMeshDescriptors.Add(chunk.subMeshDescriptors);
    //
    //    //chunk.GetComponent<MeshRenderer>().enabled = false;
    //
    //    UpdateTerrainMesh();
    //
    //    if (showDebugList)
    //        Debug_ShowDebugLists();
    //}
    //
    //public void RemoveChunkMeshFromTerrainMesh(Chunk chunk)
    //{
    //    chunkVertices.Remove(chunk.verticesList);
    //    chunkNormals.Remove(chunk.normalList);
    //    chunkIndices.Remove(chunk.indicesList);
    //    chunkMaterials.Remove(chunk.materialsList);
    //    chunkSubMeshDescriptors.Remove(chunk.subMeshDescriptors);
    //
    //    UpdateTerrainMesh();
    //
    //    if (showDebugList)
    //        Debug_ShowDebugLists();
    //}

    void Debug_ShowDebugLists()
    {
        debug_verticesList.Clear();
        debug_normalList.Clear();
        debug_indicesList.Clear();
        debug_materialsList.Clear();
        debug_subMeshDescriptors.Clear();
        for (int k = 0; k < chunkVertices.Count; k++)
        {
            for (int i = 0; i < chunkVertices[k].Count; i++)
            {
                debug_verticesList.Add(chunkVertices[k][i]);
                debug_normalList.Add(chunkNormals[k][i]);
            }
        }
        for (int k = 0; k < chunkIndices.Count; k++)
        {
            for (int i = 0; i < chunkIndices[k].Count; i++)
            {
                debug_indicesList.Add(chunkIndices[k][i]);
            }
        }
        for (int k = 0; k < chunkMaterials.Count; k++)
        {
            for (int i = 0; i < chunkMaterials[k].Count; i++)
            {
                debug_materialsList.Add(chunkMaterials[k][i]);
            }
        }
        for (int k = 0; k < chunkSubMeshDescriptors.Count; k++)
        {
            for (int i = 0; i < chunkSubMeshDescriptors[k].Count; i++)
            {
                debug_subMeshDescriptors.Add(chunkSubMeshDescriptors[k][i]);
            }
        }
    }

    void UpdateTerrainMesh()
    {
        mesh.vertices = chunkVertices.SelectMany(x => x).ToArray();
        mesh.normals = chunkNormals.SelectMany(x => x).ToArray();
        
        mesh.SetIndexBufferParams(chunkIndices.SelectMany(x => x).ToArray().Length, UnityEngine.Rendering.IndexFormat.UInt32);
        mesh.SetIndexBufferData(chunkIndices.SelectMany(x => x).ToArray(), 0, 0, chunkIndices.SelectMany(x => x).ToArray().Length);

        mesh.subMeshCount = chunkMaterials.SelectMany(x => x).ToArray().Length;
        
        //Set up mesh for each block
        for (int i = 0; i < mesh.subMeshCount; i++)
            mesh.SetSubMesh(i, chunkSubMeshDescriptors.SelectMany(x => x).ToArray()[i]);

        gameObject.GetComponent<MeshRenderer>().materials = chunkMaterials.SelectMany(x => x).ToArray();
        gameObject.GetComponent<MeshFilter>().mesh = mesh;
    }
}
