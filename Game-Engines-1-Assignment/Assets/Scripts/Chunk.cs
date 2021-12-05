using System.Collections.Generic;
using Unity.Burst;
using Unity.Mathematics;
using Unity.Collections;
using Unity.Jobs;
using UnityEngine;
using UnityEngine.Rendering;

public class Chunk : MonoBehaviour
{
    public Material Textures;

    // Chunk(width,height,depth)(16x256x16)
    public int chunkWidth = 16;
    public int chunkHeight = 256;
    public int chunkDepth = 2;

    

    // chunk position within world space
     public Vector3 chunkPosition;


    public Block[,,] blocks;
    //Flat[x + chunkwidth * (y + chunkHeight * z)] = [x, y, z]
    public MeshManager.BlockType[] chunkData;

    void BuildChunk()
    {
        int blockCount = chunkWidth * chunkDepth * chunkHeight;
        chunkData = new MeshManager.BlockType[blockCount];
        for (int i = 0; i < blockCount; i++)
        {
            int x = i % chunkWidth + (int) chunkPosition.x;
            int y = i / chunkWidth % chunkHeight + (int) chunkPosition.y;
            int z = i / (chunkWidth * chunkHeight) + (int) chunkPosition.z;

            int surfaceHeight = (int) MeshManager.fBm(x, z, World.surfaceLayerSettings.octaves, World.surfaceLayerSettings.Scale,
                World.surfaceLayerSettings.heightScale, World.surfaceLayerSettings.heightOffset); 

            int stoneHeight = (int)MeshManager.fBm(x, z, World.stoneLayerSettings.octaves, World.stoneLayerSettings.Scale,
                World.stoneLayerSettings.heightScale, World.stoneLayerSettings.heightOffset);

            int coalTopHeight = (int)MeshManager.fBm(x, z, World.coalTopLayerSettings.octaves, World.coalTopLayerSettings.Scale,
                World.coalTopLayerSettings.heightScale, World.coalTopLayerSettings.heightOffset);

            int coalBottomHeight = (int)MeshManager.fBm(x, z, World.coalBottomLayerSettings.octaves, World.coalBottomLayerSettings.Scale,
                World.coalBottomLayerSettings.heightScale, World.coalBottomLayerSettings.heightOffset);

            if (surfaceHeight == y)
            {
                chunkData[i] = MeshManager.BlockType.GrassSide; // replaceing Dirt blocks on top layer with grass side blocks.
            }

            else if (y < coalTopHeight && y > coalBottomHeight && UnityEngine.Random.Range(0.0f, 1.0f) <= World.coalTopLayerSettings.probability)
                chunkData[i] = MeshManager.BlockType.Diamond;
            else if (y < stoneHeight && UnityEngine.Random.Range(0.0f, 1.0f) <= World.stoneLayerSettings.probability)
                chunkData[i] = MeshManager.BlockType.Stone;

            else if (y < surfaceHeight)
                chunkData[i] = MeshManager.BlockType.Dirt;
            else
                chunkData[i] = MeshManager.BlockType.Air;
        }
    }

     void Start()
    {
        
    }

      public void CreateChunk(Vector3 dimensions, Vector3 position)
    {
        chunkPosition = position;

        chunkWidth = (int) dimensions.x;
        chunkHeight = (int) dimensions.y;
        chunkDepth = (int) dimensions.z;
        

        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        MeshCollider mc = this.gameObject.AddComponent<MeshCollider>();
        
        mr.material = Textures;
        blocks = new Block [chunkWidth, chunkHeight, chunkDepth];
        BuildChunk();

        var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triStart = 0;
        int meshCount = chunkWidth * chunkHeight * chunkDepth;
        int m = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);


        for (int z = 0; z < chunkDepth; z++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z) + chunkPosition, chunkData[x +  chunkWidth * (y + chunkDepth * z)], this);
                    if (blocks[x, y, z].mesh != null)
                    {
                        inputMeshes.Add(blocks[x, y, z].mesh);
                        var vcount = blocks[x, y, z].mesh.vertexCount;
                        var icount = (int)blocks[x, y, z].mesh.GetIndexCount(0);
                        jobs.vertexStart[m] = vertexStart;
                        jobs.triStart[m] = triStart;
                        vertexStart += vcount;
                        triStart += icount;
                        m++;
                    }
                }
            }
        }

        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        jobs.outputMesh = outputMeshData[0];
        jobs.outputMesh.SetIndexBufferParams(triStart, IndexFormat.UInt32);
        jobs.outputMesh.SetVertexBufferParams(vertexStart,
            new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2));

        var handle = jobs.Schedule(inputMeshes.Count, 4);
        var newMesh = new Mesh();
        newMesh.name = "Chunk " + chunkPosition.x + "_" + chunkPosition.y + "_" + chunkPosition.z;
        var sm = new SubMeshDescriptor(0, triStart, MeshTopology.Triangles);
        sm.firstVertex = 0;
        sm.vertexCount = vertexStart;

        handle.Complete();

        jobs.outputMesh.subMeshCount = 1;
        jobs.outputMesh.SetSubMesh(0, sm);
        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new[] { newMesh });
        jobs.meshData.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triStart.Dispose();
        newMesh.RecalculateBounds();

        mf.mesh = newMesh;
        mc.sharedMesh = mf.mesh;

    }

    [BurstCompile]
    struct ProcessMeshDataJob : IJobParallelFor
    {
        [ReadOnly] public Mesh.MeshDataArray meshData;
        public Mesh.MeshData outputMesh;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triStart;

        public void Execute(int index)
        {
            var data = meshData[index];
            var vCount = data.vertexCount;
            var vStart = vertexStart[index];

            var verts = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(verts.Reinterpret<Vector3>());

            var normals = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());

            var uvs = new NativeArray<float3>(vCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>();
            var outputNormals = outputMesh.GetVertexData<Vector3>(stream: 1);
            var outputUVs = outputMesh.GetVertexData<Vector3>(stream: 2);

            for (int i = 0; i < vCount; i++)
            {
                outputVerts[i + vStart] = verts[i];
                outputNormals[i + vStart] = normals[i];
                outputUVs[i + vStart] = uvs[i];
            }

            verts.Dispose();
            normals.Dispose();
            uvs.Dispose();

            var tStart = triStart[index];
            var tCount = data.GetSubMesh(0).indexCount;
            var outputTris = outputMesh.GetIndexData<int>();
            if (data.indexFormat == IndexFormat.UInt16)
            {
                var tris = data.GetIndexData<ushort>();
                for (int i = 0; i < tCount; ++i)
                {
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }
            else
            {
                var tris = data.GetIndexData<int>();
                for (int i = 0; i < tCount; ++i)
                {
                    int idx = tris[i];
                    outputTris[i + tStart] = vStart + idx;
                }
            }

        }
    }

    // Update is called once per frame
    void Update()
    {

    }
}
