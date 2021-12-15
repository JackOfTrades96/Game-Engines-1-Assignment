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

    // Chunk(width,height,depth)(16x16x16)
    public int chunkWidth = 16;
    public int chunkHeight = 16;
    public int chunkDepth = 16;

    // chunk position within world space
    public Vector3 chunkPosition;

    public Block[,,] blocks;

    //Flat[x + chunkwidth * (y + chunkHeight * z)] = [x, y, z]
    public MeshManager.BlockType[] chunkData;
    public MeshRenderer meshRenderer;

    CalculateBlockTypes calculateBlockTypes;
    JobHandle ProcessBlocks;

    struct CalculateBlockTypes : IJobParallelFor
    {
        public NativeArray<MeshManager.BlockType> chunkData;
        public int chunkWidth;
        public int chunkHeight;
        public Vector3 chunkPosition;
        public Unity.Mathematics.Random random;

        public void Execute(int i)
        {
            int x = i % chunkWidth + (int)chunkPosition.x;
            int y = (i / chunkWidth) % chunkHeight + (int)chunkPosition.y;
            int z = i / (chunkWidth * chunkHeight) + (int)chunkPosition.z;

            random = new Unity.Mathematics.Random(1);

            int mountainLayerHeight =  (int)FractalBrownianMotion.fBm(x, z, World.mountainLayerSettings.octaves, World.mountainLayerSettings.Scale, World.mountainLayerSettings.heightScale,
            World.mountainLayerSettings.heightOffset);

            int dirtLayerHeight = (int)FractalBrownianMotion.fBm(x, z, World.dirtLayerSettings.octaves, World.dirtLayerSettings.Scale, World.dirtLayerSettings.heightScale,
            World.dirtLayerSettings.heightOffset);


            int stoneLayerHeight = (int)FractalBrownianMotion.fBm(x, z, World.stoneLayerSettings.octaves, World.stoneLayerSettings.Scale, World.stoneLayerSettings.heightScale,
            World.stoneLayerSettings.heightOffset);

          int ironTopLayerHeight = (int)(int)FractalBrownianMotion.fBm(x, z, World.ironTopLayerSettings.octaves, World.ironTopLayerSettings.Scale, World.ironTopLayerSettings.heightScale,
            World.ironTopLayerSettings.heightOffset);

            int ironBottomLayerHeight = (int)(int)FractalBrownianMotion.fBm(x, z, World.ironBottomLayerSettings.octaves, World.ironBottomLayerSettings.Scale, World.ironBottomLayerSettings.heightScale,
            World.ironBottomLayerSettings.heightOffset);



            int CaveLayerHeight = (int)FractalBrownianMotion3D.fBm3D(x, y, z, World.caveLayerSettings.octaves,
                           World.caveLayerSettings.Scale, World.caveLayerSettings.heightScale,
                           World.caveLayerSettings.heightOffset);

           

            if (y < 3 )
            {
                chunkData[i] = MeshManager.BlockType.BedRock;
                return;
            }

            if (CaveLayerHeight < World.caveLayerSettings.probability)
            {
                chunkData[i] = MeshManager.BlockType.Air;
                return;
            }



            if (dirtLayerHeight == y)
            {
                chunkData[i] = MeshManager.BlockType.GrassOnSide;
            }


            else if (y < mountainLayerHeight && y > dirtLayerHeight)
            {
                chunkData[i] = MeshManager.BlockType.Stone;
            }


            else if (y < ironTopLayerHeight && y > ironBottomLayerHeight && random.NextFloat(1) <= World.ironTopLayerSettings.probability)
            {
                chunkData[i] = MeshManager.BlockType.Iron;
            }
               

            else if (y < stoneLayerHeight)
            {
                chunkData[i] = MeshManager.BlockType.Stone;

            }

            else if (y < dirtLayerHeight )
            {
                chunkData[i] = MeshManager.BlockType.Dirt;
            }
               

            else
                chunkData[i] = MeshManager.BlockType.Air;
        }
    }

    void BuildChunk()
    {
        int blockCount = chunkWidth * chunkDepth * chunkHeight;
        chunkData = new MeshManager.BlockType[blockCount];
        NativeArray<MeshManager.BlockType> blockTypes = new NativeArray<MeshManager.BlockType>(chunkData, Allocator.Persistent);
        calculateBlockTypes = new CalculateBlockTypes()
        {
            chunkData = blockTypes,
            chunkWidth = chunkWidth,
            chunkHeight = chunkHeight,
            chunkPosition = chunkPosition
        };

        ProcessBlocks = calculateBlockTypes.Schedule(chunkData.Length, 64);
        ProcessBlocks.Complete();
        calculateBlockTypes.chunkData.CopyTo(chunkData);
        blockTypes.Dispose();
    }

   
    public void CreateChunk(Vector3 chunkDimensions, Vector3 chunkPosition , bool rebuildChunkBlocks = true)
    {
        this.chunkPosition = chunkPosition;
        chunkWidth = (int)chunkDimensions.x;
        chunkHeight = (int)chunkDimensions.y;
        chunkDepth = (int)chunkDimensions.z;

        MeshFilter chunkMeshFilter = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer chunkMeshRenderer = this.gameObject.AddComponent<MeshRenderer>();
        meshRenderer = chunkMeshRenderer;
        chunkMeshRenderer.material = Textures;
        blocks = new Block[chunkWidth, chunkHeight, chunkDepth];
        if(rebuildChunkBlocks)
        {
            BuildChunk();
        }
       

        var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triangleStart = 0;
        int meshCount = chunkWidth * chunkHeight * chunkDepth;
        int m = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triangleStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);


        for (int z = 0; z < chunkDepth; z++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z) + this.chunkPosition, chunkData[x + chunkWidth * (y + chunkDepth * z)], this);
                    if (blocks[x, y, z].blockMesh != null)
                    {
                        inputMeshes.Add(blocks[x, y, z].blockMesh);
                        var vertexCount = blocks[x, y, z].blockMesh.vertexCount;
                        var icount = (int)blocks[x, y, z].blockMesh.GetIndexCount(0);
                        jobs.vertexStart[m] = vertexStart;
                        jobs.triangleStart[m] = triangleStart;
                        vertexStart += vertexCount;
                        triangleStart += icount;
                        m++;
                    }
                }
            }
        }

        jobs.meshData = Mesh.AcquireReadOnlyMeshData(inputMeshes);
        var outputMeshData = Mesh.AllocateWritableMeshData(1);
        jobs.outputMesh = outputMeshData[0];
        jobs.outputMesh.SetIndexBufferParams(triangleStart, IndexFormat.UInt32);
        jobs.outputMesh.SetVertexBufferParams(vertexStart,
            new VertexAttributeDescriptor(VertexAttribute.Position),
            new VertexAttributeDescriptor(VertexAttribute.Normal, stream: 1),
            new VertexAttributeDescriptor(VertexAttribute.TexCoord0, stream: 2));

        var handle = jobs.Schedule(inputMeshes.Count, 4);
        var newMesh = new Mesh();
        newMesh.name = "Chunk " + chunkPosition.x + "_" + chunkPosition.y + "_" + chunkPosition.z;
        var submesh = new SubMeshDescriptor(0, triangleStart, MeshTopology.Triangles);
        submesh.firstVertex = 0;
        submesh.vertexCount = vertexStart;

        handle.Complete();

        jobs.outputMesh.subMeshCount = 1;
        jobs.outputMesh.SetSubMesh(0, submesh);
        Mesh.ApplyAndDisposeWritableMeshData(outputMeshData, new[] { newMesh });
        jobs.meshData.Dispose();
        jobs.vertexStart.Dispose();
        jobs.triangleStart.Dispose();
        newMesh.RecalculateBounds();

        chunkMeshFilter.mesh = newMesh;
        MeshCollider collider = this.gameObject.AddComponent<MeshCollider>();
        collider.sharedMesh = chunkMeshFilter.mesh;
    }

    [BurstCompile]
    struct ProcessMeshDataJob : IJobParallelFor
    {
        [ReadOnly] public Mesh.MeshDataArray meshData;
        public Mesh.MeshData outputMesh;
        public NativeArray<int> vertexStart;
        public NativeArray<int> triangleStart;

        public void Execute(int index)
        {
            var data = meshData[index];
            var vertexCount = data.vertexCount;
            var vertex_Start = vertexStart[index];

            var vertices = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetVertices(vertices.Reinterpret<Vector3>());

            var normals = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetNormals(normals.Reinterpret<Vector3>());

            var uvs = new NativeArray<float3>(vertexCount, Allocator.Temp, NativeArrayOptions.UninitializedMemory);
            data.GetUVs(0, uvs.Reinterpret<Vector3>());

            var outputVerts = outputMesh.GetVertexData<Vector3>();
            var outputNormals = outputMesh.GetVertexData<Vector3>(stream: 1);
            var outputUVs = outputMesh.GetVertexData<Vector3>(stream: 2);

            for (int i = 0; i < vertexCount; i++)
            {
                outputVerts[i + vertex_Start] = vertices[i];
                outputNormals[i + vertex_Start] = normals[i];
                outputUVs[i + vertex_Start] = uvs[i];
            }

            vertices.Dispose();
            normals.Dispose();
            uvs.Dispose();

            var trianglesStart = triangleStart[index];
            var trianglesCount = data.GetSubMesh(0).indexCount;
            var outputTriangles = outputMesh.GetIndexData<int>();
            if (data.indexFormat == IndexFormat.UInt16)
            {
                var tris = data.GetIndexData<ushort>();
                for (int i = 0; i < trianglesCount; ++i)
                {
                    int idx = tris[i];
                    outputTriangles[i + trianglesStart] = vertex_Start + idx;
                }
            }
            else
            {
                var tris = data.GetIndexData<int>();
                for (int i = 0; i < trianglesCount; ++i)
                {
                    int idx = tris[i];
                    outputTriangles[i + trianglesStart] = vertex_Start + idx;
                }
            }

        }
    }

    
    
}

