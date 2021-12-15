# Game-Engines-1-Assignment
Voxel Procedural Generation
# Project Title
Name: Jack McKenna 

Student Number: C19744931
 
Class Group: Game Design

# Description Of Project
My Project is based on creating a Voxel based Procedural Generation.

Every Voxel will be generated within  16x16x16 chunks.

I will use both  Fractal Brownian Motion, which is a form of Prelin  Noise  to generate each layer of terrain within the game world

For the textures I will be using 2d art assets for the voxels textures.

As the voxels are generated there block type will determine what textures are rendered on each block.

The Player will be able to move around the terrain as well as remove blocks from both the surface of the world and underground.

### Visual Aspects
- Voxel generated Terrain with differnt block textures for  each block type 

- New Voxel terrain being generated as the player moves from its spawn location.

- Ores of different types being randomly  generated within the world under the surface layer. 

-  Stone Mountains 


### Interactive  Aspects
- Player Movement & Jumping

- Player Camera Controlled by Mouse

- Digging Mechanic 


# Instructions For Use
When Start is pressed the world chunks will  load. Once the loading Bar has reached 100% the player prebaf will be spawned within the world.
Once spawned the player can move around the world. Reaching the edge of a chunk with no chunk will either genrate a new chunk if none has been created previously  or the world script "unhide" that chunk.
if the player left clicks they will remove the block they clicked on, allowing the player to explore beneath the surface of world.

# How it Works

- The Voxel Terrain system  works  by dividing  the generation of the game world into 4 parts the quads, blocks chunks and world itself.
  
- The Quad script is used to generate  the 6 quads that will make up each block in the game world.

- In order to create the blocks needed. The vertices, normals ,uvs and triangles of each quad are created and set. 

```
  public class Quad 
{

	public Mesh quadMesh;

	public Quad(MeshManager.BlockFace face, Vector3 vertexOffset, MeshManager.BlockType blockType) //(each quad face, Offset = the position of each face, what texture it will have )
	{

		quadMesh = new Mesh();

		Vector3[] vertices = new Vector3[4]; // Block Vertices Array
		Vector3[] normals = new Vector3[4]; // Block Normal Array
		Vector2[] uvs = new Vector2[4]; // Block Uvs Array
		int[] triangles = new int[6]; // Block Traingle Array 

		Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f) + vertexOffset;
		Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f) + vertexOffset;
		Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f) + vertexOffset;
		Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f) + vertexOffset;
		Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f) + vertexOffset;
		Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f) + vertexOffset;
		Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f) + vertexOffset;
		Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f) + vertexOffset;
  
  ```
  
  - Once the vertice, normals, uvs and triangles are created, a switch statement is used to set up each block face. 
  
  ```

		switch (face) // Using a  switch statement to set the vertices, normals and uvs of each Quad Face that will make up each Block.
		{
			case MeshManager.BlockFace.Top:
				vertices = new Vector3[] { v7, v6, v5, v4 };
				normals = new Vector3[] {Vector3.up, Vector3.up,
											Vector3.up, Vector3.up};
				uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };

				break;

			case MeshManager.BlockFace.Bottom:
				vertices = new Vector3[] { v0, v1, v2, v3 };
				normals = new Vector3[] {Vector3.down, Vector3.down,
											Vector3.down, Vector3.down};
				uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };

				break;


			case MeshManager.BlockFace.Front:
				vertices = new Vector3[] { v4, v5, v1, v0 };
				normals = new Vector3[] {Vector3.forward, Vector3.forward,
											Vector3.forward, Vector3.forward};
				uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };

				break;
			case MeshManager.BlockFace.Back:
				vertices = new Vector3[] { v6, v7, v3, v2 };
				normals = new Vector3[] {Vector3.back, Vector3.back,
											Vector3.back, Vector3.back};
				uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };

				break;


			case MeshManager.BlockFace.Left:
				vertices = new Vector3[] { v7, v4, v0, v3 };
				normals = new Vector3[] {Vector3.left, Vector3.left,
											Vector3.left, Vector3.left};
				uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };

				break;
			case MeshManager.BlockFace.Right:
				vertices = new Vector3[] { v5, v6, v2, v1 };
				normals = new Vector3[] {Vector3.right, Vector3.right,
											Vector3.right, Vector3.right};
				uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
				triangles = new int[] { 3, 1, 0, 3, 2, 1 };

				break;
			
		}
  
  ```
 
- Once each block face  of each quad is set, the vertices, normals , uvs and triangles of the quad mesh are set to use the vertices, normals, uvs and triangles that have been created from the arrays list's.

- The Volume of each quadmesh is then recalculated.
  
  ```

		//setting each qaud vertices, normals ,uvs and trianlge to the ones  in the vaules in the arrays.

		quadMesh.vertices = vertices;
		quadMesh.normals = normals;
		quadMesh.uv = uvs;
		quadMesh.triangles = triangles;

		//Recaluate the bounding volume of the mesh from the veritices.
		quadMesh.RecalculateBounds();
  


- The Block Script  is then used  to define each block within its chunk.


```

 public Mesh blockMesh; // block mesh (includes all quad meshes)
    Chunk parentChunk;

    public Block(Vector3 offset, MeshManager.BlockType blocktype, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPosition = offset - chunk.chunkPosition;
    }
    
        
```

- The block scipt deals with two parts 

-  Firstly if a block is connected in any way to any other block within the game world, the quads that are hidden will not have their meshes rendered.

- This is done for perfomance reasons as having every quad mesh rendered all at once would lead to poor performance

- Once the blocks are checked their quads are generated and rendered.

```

        if (blocktype != MeshManager.BlockType.Air) // if blocktype is not equal to Air Block.
        {
            List<Quad> quads = new List<Quad>(); // creates list of quads from Quad class
            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z)) // if a Block has no neighbouring block at its x,y or z Coords.
            {
                if (blocktype == MeshManager.BlockType.GrassSide)
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, MeshManager.BlockType.GrassTop)); // Set Top quad texture on surface Blocks to Grass.
                else
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, blocktype));
            }

            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z))
            {
                if (blocktype == MeshManager.BlockType.GrassSide)
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, MeshManager.BlockType.GrassSide));
                else
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, blocktype));
            }

            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z + 1))
                quads.Add(new Quad(MeshManager.BlockFace.Front, offset, blocktype));

            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z - 1))
                quads.Add(new Quad(MeshManager.BlockFace.Back, offset, blocktype));

            if (!BlockHasNeigbour((int)blockLocalPosition.x - 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Left, offset, blocktype));

            if (!BlockHasNeigbour((int)blockLocalPosition.x + 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Right, offset, blocktype));
           
           

            if (quads.Count == 0) return;

            Mesh[] faceMeshes = new Mesh[quads.Count];
            int blockMeshAmount = 0;
            foreach (Quad q in quads)
            {
                faceMeshes[blockMeshAmount] = q.quadMesh;
                blockMeshAmount++;
            }

            blockMesh = MeshManager.MergeMeshes(faceMeshes);
            blockMesh.name = "Cube_0_0_0";
        }
    }

    public bool BlockHasNeigbour(int x, int y, int z)
    {
        if (x < 0 || x >= parentChunk.chunkWidth ||
            y < 0 || y >= parentChunk.chunkHeight ||
            z < 0 || z >= parentChunk.chunkDepth)
        {
            return false;
        }
        if (parentChunk.chunkData[x + parentChunk.chunkWidth * (y + parentChunk.chunkDepth * z)] == MeshManager.BlockType.Air
            || parentChunk.chunkData[x + parentChunk.chunkWidth * (y + parentChunk.chunkDepth * z)] == MeshManager.BlockType.Water)
            return false;
        return true;
    }
    
```


- The Mesh Manager Then takes the 6 quads Meshes and merges them togther to make a single block mesh.
- The Tuple system is used to create a data of vertices, normlas and uvs, while getting rid of any duplicte values form different quad meshes.

```
using Data = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;
```

- The Block Type enum defines what type each block within their chunks will be.

```
 public enum BlockType // Using a enum for each block texture which is used in the chunk script to texture each block to its correct block type.
    {
        GrassTop, GrassSide, Dirt, Stone, Water, Coal, Iron, Gold, Diamond, BedRock, Air
    };
```

- The Block Face enum defines which part each blokc quad will be.

```
 public enum BlockFace { Top, Bottom, Front, Back, Left, Right}; // Using a enum which is used in the quad scripts switch statement to create each quad face.

```

The Static Vector 2 blockUvs Take the texture from the Texture Sprite, with each of the 4 Vectors acting as the 4 vertices of that texure.

```
public static Vector2[,] blockUVs = {

        /*GrassOnTop*/ {  new Vector2(0.0625f,0.9375f), new Vector2(0.125f,0.9375f),
                        new Vector2(0.0625f, 1f), new Vector2(0.125f,1f) },

        /*GrassOnSide*/ { new Vector2( 0f, 0.9375f ), new Vector2( 0.0625f, 0.9375f),
                        new Vector2( 0f, 1.0f ),new Vector2( 0.0625f, 1.0f )},

        /*Dirt*/	  { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                        new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},

        /*Stone*/	  { new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                        new Vector2( 0.1875f, 1f ),new Vector2( 0.25f, 1f )},

        /*Water*/	  { new Vector2(0.875f,0.125f),  new Vector2(0.9375f,0.125f),
                        new Vector2(0.875f,0.1875f), new Vector2(0.9375f,0.1875f)},
       
      
        /*Coal*/      { new Vector2( 0.125f, 0.9375f ), new Vector2(0.1875f, 0.9375f),
                      new Vector2(0.125f, 1.0f), new Vector2( 0.1875f, 1.0f )},

      
        /*BedRock*/   { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                      new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},

        /*Air */      { new Vector2(0f,0f), new Vector2(0f,0f),
                      new Vector2(0f, 0f), new Vector2(0,0f) },

    };
```

The Quad Script then uses these Vector2 values to texture each quad in the block.

```

		Vector2 uv00 = MeshManager.blockUVs[(int)blockType, 0];
		Vector2 uv10 = MeshManager.blockUVs[(int)blockType, 1];
		Vector2 uv01 = MeshManager.blockUVs[(int)blockType, 2];
		Vector2 uv11 = MeshManager.blockUVs[(int)blockType, 3];
  
```

- The MergeMeshes static function Then use a nested for loop to pass the values of the  vertices, normals, uvs and trianglesthrough and if not present add them to the Dictionary and HashSet.
- 

```
 public static Mesh MergeMeshes(Mesh[] meshes) {
        Mesh blockMesh = new Mesh();

        Dictionary<Data, int> verticesDictionary = new Dictionary<Data, int>(); // holds the names and values of the mesh  vertices 
        HashSet<Data> verticesHash = new HashSet<Data>(); // similar to the Dictionary but no values are held by it. Only being used to decide wheter the a vertices is in the data structure
        List<int> triangles = new List<int>(); //mesh triangles

        int verticesIndex = 0; //  current vertex

        for (int i = 0; i < meshes.Length; i++) //loop through each mesh being passed through.
        {
            if (meshes[i] == null) continue; // when no refernce to a mesh occurs.

            for (int j = 0; j < meshes[i].vertices.Length; j++) //loops through each of the vertex in the  current mesh
            {
                Vector3 Vertices = meshes[i].vertices[j];
                Vector3 Normals = meshes[i].normals[j];
                Vector2 Uvs = meshes[i].uv[j];

                Data vertexdata = new Data(Vertices, Normals, Uvs); // Creating new Data Structure and passing through the vertices, normlas and Uvs
                if (!verticesHash.Contains(vertexdata)) 
                {
                    verticesDictionary.Add(vertexdata,verticesIndex);
                    verticesHash.Add(vertexdata);

                    verticesIndex++;
                }

            }

            for (int t = 0; t < meshes[i].triangles.Length; t++)
            {
                int triPoint = meshes[i].triangles[t];
                Vector3 Vertices = meshes[i].vertices[triPoint];
                Vector3 Normals = meshes[i].normals[triPoint];
                Vector2 Uvs = meshes[i].uv[triPoint];
                Data vertexdata = new Data(Vertices, Normals, Uvs);

                int index;
                verticesDictionary.TryGetValue(vertexdata, out index);
                triangles.Add(index);
            }
            meshes[i] = null;
        }

```
- Once the meshes are Processed the ExtractArrays function  extracts the  vertices, normals and uv  and triangle values from the dictionary and are then pass into the new block mesh.
- The volume of the block mesh is recalculated and the blockmesh is returned.

```
public static void ExtractArrays(Dictionary<Data, int> list, Mesh blockMesh)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (Data vertices in list.Keys) {
            verts.Add(vertices.Item1);
            norms.Add(vertices.Item2);
            uvs.Add(vertices.Item3);
        }
        blockMesh.vertices = verts.ToArray();
        blockMesh.normals = norms.ToArray();
        blockMesh.uv = uvs.ToArray();
    }
    
     ExtractArrays(verticesDictionary, blockMesh);
        blockMesh.triangles = triangles.ToArray();
        blockMesh.RecalculateBounds();
        return blockMesh;

```


- In the Chunk Script the blocks are generated within the game world in chunks.
- In order to first create the chunks within the game world the CreateChunk Function is used.
- The CreateChunk function creates the chunk by using a nested for loop using the chunkdepth,chunkheight and chunkwidth.
```
 for (int z = 0; z < chunkDepth; z++)
        {
            for (int y = 0; y < chunkHeight; y++)
            {
                for (int x = 0; x < chunkWidth; x++)
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z) + this.chunkPosition, chunkData[x + chunkWidth * (y + chunkDepth * z)], this);
                    if (blocks[x, y, z].blockMesh != null)
                    {
                        
                    }
                }
            }
        }
```

- In order to render the chunk the unitys jobs system is used for optimal performace.
- To use the jobs system I imported the unity Packages Burst Complier and Mathematics.


```
using Unity.Jobs;
using Unity.Burst;
using Unity.Mathematics;
```



- The vertex and triangles position values of the block mesh is obtained and the link to the  job is created and the job  is then passed into the nested for loop.
- two Native Array are used for the vertices and the triangles  for the job.
- Inside the Nested for loop  the vertices and triangle values are taken for the job.
-  The values within the blocks array are taken and placed within a flattened array.


```
var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triangleStart = 0;
        int meshCount = chunkWidth * chunkHeight * chunkDepth;
        int m = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triangleStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        
        
        inputMeshes.Add(blocks[x, y, z].blockMesh);
                        var vertexCount = blocks[x, y, z].blockMesh.vertexCount;
                        var icount = (int)blocks[x, y, z].blockMesh.GetIndexCount(0);
                        jobs.vertexStart[m] = vertexStart;
                        jobs.triangleStart[m] = triangleStart;
                        vertexStart += vertexCount;
                        triangleStart += icount;
                        m++;
        



- To run the job within the nested for loop a struct is used.
- Within the Execute function the vertices, normals and uvs data are taken from the meshdata index and are placed within each of the Native Arrays.
- They are then reinterpreting the Vector3 values as float3 values by using NativeArrays.
- The outputed values are then looped.
- In order to prevent a memory leak the Native Arrays that were used to convert the values are disposed.
 

```


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
    

                                                   
                                                   
```
                                                   
                                                   
- The vertices and triangle outputmeshs from  job is then passed though the  CreateChunk function.
- The outputVerts,OutputNormals and OutputUVs are taken from the job struct and passed into the CreateChunk Script


```
var inputMeshes = new List<Mesh>();
        int vertexStart = 0;
        int triangleStart = 0;
        int meshCount = chunkWidth * chunkHeight * chunkDepth;
        int m = 0;
        var jobs = new ProcessMeshDataJob();
        jobs.vertexStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
        jobs.triangleStart = new NativeArray<int>(meshCount, Allocator.TempJob, NativeArrayOptions.UninitializedMemory);
```
- The  4 jobs needed are created within the CreateChunk function with the new mesh for the chunk.

- The job mesh data is then disposed of and the var newMesh volume is recalculated.

- The meshFilter and meshCollider are then set for the chunkmesh.


```
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
```

- To determine  each block type within the chunk a matrix array is used.

```
 public NativeArray<MeshManager.BlockType> chunkData;
        public int chunkWidth;
        public int chunkHeight;
        public Vector3 chunkPosition;
        public Unity.Mathematics.Random random;

```
 
- The BuildChunk  function use the array to determine each blocktype.

```
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
```

- The Execute function is used to determine how the chunk meshs are rendered between each Perlin Noise Graph
```
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
```



- The World script creates the the quads, blocks and chunks that make up the game and determines there position relative to the game world space.
- The World dimensions are set to (8,8,8) chunks within the world
- The chunk dimensions are set to (16,16,16) blocks within each chunk.
- To generate the terrain layers each layer uses a Pelin Graph and its own individual Perlin settings.

```
 public static Vector3Int worldDimensions = new Vector3Int(8, 8, 8); // the world Dimensions realtive to chunks (8 chunks on x axis, 8 chunks on y axis, 8 chunks on the z axis)
    public static Vector3Int chunkDimensions = new Vector3Int(16, 16, 16); //the chunk Dimensions relactive to the blocks (16 blocks on x axis in chunk, 16 blocks on the y axis, 16 blocks on the z axis)
    public GameObject chunkPrefab; // The chunk prefab being called when the chunks are being created.
    public GameObject IntroCamera; // first camera used while world is loading
    public GameObject Player; 
    public Slider loadingBar; // shows the process of the worldchunks being loaded

    // Perlin Noise Graphs  and the settings for each layer of terrain within the world.

    public static PerlinSettings mountainLayerSettings;
    public PerlinGrapher mountainLayer;

    public static PerlinSettings dirtLayerSettings;
    public PerlinGrapher dirtLayer;

    public static PerlinSettings stoneLayerSettings;
    public PerlinGrapher stoneLayer;
    
    public static PerlinSettings ironTopLayerSettings;
    public PerlinGrapher ironTopLayer;

    public static PerlinSettings ironBottomLayerSettings;
    public PerlinGrapher ironBottomLayer;

   
    public static PerlinSettings caveLayerSettings;
    public PerlinGrapher3D caveLayer;
```

- On start the loading is set to the world Dimensions values and each of the perlin Noise Settings are created.

```
 // Loading bar total value is equal to amount to blocks in the world in the x axis multple by amount to blocks in the world in the z axis

        loadingBar.maxValue = worldDimensions.x * worldDimensions.z;

        //Setting each Perlin Setting to its correct values

        mountainLayerSettings = new PerlinSettings(mountainLayer.heightScale, mountainLayer.Scale,
                                     mountainLayer.octaves, mountainLayer.heightOffset, mountainLayer.probability);

        dirtLayerSettings = new PerlinSettings(dirtLayer.heightScale, dirtLayer.Scale,
                                     dirtLayer.octaves, dirtLayer.heightOffset, dirtLayer.probability);

        stoneLayerSettings = new PerlinSettings(stoneLayer.heightScale, stoneLayer.Scale,
                             stoneLayer.octaves, stoneLayer.heightOffset, stoneLayer.probability);

        

        caveLayerSettings = new PerlinSettings(caveLayer.heightScale, caveLayer.Scale,
             caveLayer.octaves, caveLayer.heightOffset, caveLayer.Probability);

      // BeginCorotuine to generate world.
        StartCoroutine(BuildWorld());
```

- To build the actual world a coroutine BuildWorld is used with a nested for loop using the world dimensions.
- Within the nested for loop BuildChunk is called to build each of the chunks with a for loop that use the world dimensions Vector3Int y value.
- When each chunk is built the loadbar value is increased to show the world build progress.
- In order to spawn the player precisly within the game world we set the x, and z postion of the player to world dimensions divide by 2.
- For the y value we  set a value higher than our surface layer.
- The player will now spawn and drop into the game world.
- At the End of the Coroutine both the ChunkBuildCoordinator and UpdateWorld are called to update the game world with new chunks.


```
 IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++)
            {
                BuildChunkPrefabs(x * chunkDimensions.x, z * chunkDimensions.z);
                loadingBar.value++;
                yield return null;
            }

        }

        int xpos = (worldDimensions.x * chunkDimensions.x) / 2;
        int zpos = (worldDimensions.z * chunkDimensions.z) / 2;

        loadingBar.gameObject.SetActive(false); //Set Loadingbar false
        IntroCamera.SetActive(false); // Camera is switched to player view once world has been loaded
        Player.SetActive(true); // Set player Prefab Active.
        Player.transform.position = new Vector3(xpos, 100, zpos); //Spawn Player
     
        lastPlayerPosition = Vector3Int.CeilToInt(Player.transform.position); // Player Position Vector3Int that is used to enable/disalbe the chunk mesh renderers.
        StartCoroutine(ChunkBuildCoordinator()); // Build Chunks in world using Chunk Build Coordinator 
        StartCoroutine(UpdateWorld()); // Update the world chunks.
       
    }
    
```
- In order to control the overall performance of the world the Hide_Chunks_Prefabs function and Hide_Cunk_Prefab coroutine are used set each chunks meshRenderer to false when the player has moved out of its range by using the chunkCheck to determine its position.

```
public void HideChunkPrefabs(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int pos = new Vector3Int(x, y * chunkDimensions.y, z);
            if (chunkCheck.Contains(pos))
            {
                chunks[pos].meshRenderer.enabled = false;
            }
        }
    }

    IEnumerator Hide_Chunk_Prefabs(int x, int z)
    {
        Vector2Int playerPosition = new Vector2Int(x, z);
        foreach (Vector2Int cc in chunkPrefabs)
        {
            if ((cc - playerPosition).magnitude >= distanceFromChunk * chunkDimensions.x)
            {
                HideChunkPrefabs(cc.x, cc.y);
            }
        }
        yield return null;
    }

```
- TheBuildHiddenWorld Coroutine is used to create the new chunks that are needed when the player moves around the game world.
~~~

 IEnumerator BuildHiddenWorld(int x, int z, int rad)
    {
        int nextrad = rad - 1;
        if (rad <= 0) yield break;

        BuildChunkPrefabs(x, z + chunkDimensions.z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x, z + chunkDimensions.z, nextrad));
        yield return null;

        BuildChunkPrefabs(x, z - chunkDimensions.z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x, z - chunkDimensions.z, nextrad));
        yield return null;

        BuildChunkPrefabs(x + chunkDimensions.x, z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x + chunkDimensions.x, z, nextrad));
        yield return null;

        BuildChunkPrefabs(x - chunkDimensions.x, z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x - chunkDimensions.x, z, nextrad));
        yield return null;
    }
~~~


 - The Update World Coroutine is then used to determine which chunk prefabs are needed to be hidden by using a Vector3Int value set from the players position.
 - The ChunkPrefabBuildQueue are the two coroutine Queue used when handling the BuildHiddenWorld and Hide_Chunk_Prefabs cooroutines.
 - Every Time the UpdateWorldPause is called, the world will update itself and decide which chunk renderers are set to true or false or if a chunk is needed to be created.

```
  IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastPlayerPosition - Player.transform.position).magnitude > (chunkDimensions.x))
            {
                lastPlayerPosition = Vector3Int.CeilToInt(Player.transform.position);
                int posx = (int)(Player.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int posz = (int)(Player.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(posx, posz, distanceFromChunk));
                chunkPrefabBuildQueue.Enqueue(Hide_Chunk_Prefabs(posx, posz));
            }
            yield return UpdateWorldPause;
        }
    }
```


- To generate the game world Fractal Brownian Motion , which is a form of Perlin Noise is used.
- A float fBm is created to represent the height value that will be used by the other classes.
- As the float loops around the Perlin octaves, the total float will increase by Mathf.PerlinNoise with the x and z values being multiped by both the Noise scale and the Noise frequency.
- The total value is then returned with the hieghtoffset added.

```
public static float fBm(float x, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * scale * frequency, z * scale * frequency) * heightScale;
            frequency *= 2;
        }
        return total + heightOffset; // in order for the return value to be used wherever the Perlin grpah is drawn.
```


- The Perlin Graph work by displaying the Perlin noise from the FractalBrowninanMotion Class that is being used, onto the linerender itself.
Perlin Graph uses the Perlin settings of the fractal Brownian Method in its class to generate the Perlin Noise.

```
public LineRenderer lineRenderer;
    public float heightScale = 2; // Adjust height of the Perlin Graph.
    public float Scale = 0.5f; // The Perlin Noise Scale.
    public int octaves = 1;
    public float heightOffset = 1; 
    public float probability = 1; // The probality of the block texture being rendered.
   
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        Graph();
    }

    
    // Graph Function draws the linerender to show the Perlin Settings 
    void Graph()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        int z = 0;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        for (int x = 0; x < lineRenderer.positionCount; x++)
        {
            float y = FractalBrownianMotion.fBm(x, z, octaves, Scale, heightScale, heightOffset);
            positions[x] = new Vector3(x, y, z);
        }
        lineRenderer.SetPositions(positions);
    }
    void OnValidate()
    {
        Graph();
    }
                                                       
    
```



- To generate the caves beneath the surface layer, three dimensional Perlin Noise is needed to be used.
- In order to calculate perlin noise in a three dimensional area the fBM3D class uses the two dimensional perlin noise fBm value six times for the x, y and z axis
- To get the perlin noise of that area you take the six perlin noise that are in that area and divide by 6 in order to get the average value. 

                                                       
```                                                       
public static float fBm3D(float x, float y, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        // To create 3D perlin noise use the Perlin noise values of each axis.

        float xy = FractalBrownianMotion.fBm(x, y, octaves, scale, heightScale, heightOffset); 
        float xz = FractalBrownianMotion.fBm(x, z, octaves, scale, heightScale, heightOffset);
        float yx = FractalBrownianMotion.fBm(y, x, octaves, scale, heightScale, heightOffset);
        float yz = FractalBrownianMotion.fBm(y, z, octaves, scale, heightScale, heightOffset);
        float zx = FractalBrownianMotion.fBm(z, x, octaves, scale, heightScale, heightOffset);
        float zy = FractalBrownianMotion.fBm(z, y, octaves, scale, heightScale, heightOffset);

        return (xy + yz + xz + yx + zy + zx) / 6.0f; // combine the 6 perlin noises values and divide by 6 to get the Perlin noise average of the area.
    }
}
```



- In order to creat the caves using the 3D perlin noise a Perlin graph 3D is used.
- Similar to the perlin graph 2D, this class first creates the cave blocks by using a nesteeed for loop with the caveDimenions z, y and x values.
- The cave itslef is then created with the Perlin Graph function by taking the caveblocks that were created within the first function and looped again.



- The player script is used so that the player can explore underneath the surface layer.
- Using raycasting when the player left clicks a raycast hits a block within a chunk. 
- The collider for the chunk in which the block resides is called.
- The block locstion is obtained by using the chunk position as transforms cannot be as all gameobjects chunks reside at (0,0,0)
- With the block within the chunk we are digging now obtained instead of removing that block its blocktype is instead switched to a air block
- The chunks mesh filter, mesh renderer and collider are destroyed and news one are generated.
- Because the new block is an air block no collider will be applied to it allowing the player to pass through the new air block.
 
 
                                                       
                                                       
                                                       
```
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20))
            {
                Vector3 hitBlock = Vector3.zero;

                if (Input.GetMouseButtonDown(0))
                {
                    hitBlock = hit.point - hit.normal / 2.0f;
                }


                Debug.Log("Block " + hitBlock.x + "," + hitBlock.y + "," + hitBlock.z);
                Chunk diggingChunk = hit.collider.gameObject.GetComponent<Chunk>();

                int hitBlockX = (int)(Mathf.Round(hitBlock.x) - diggingChunk.chunkPosition.x); // Getting the block location
                int hitBlockY = (int)(Mathf.Round(hitBlock.y) - diggingChunk.chunkPosition.y); // within a chunk by using chunkposition of chunk 
                int hitBlockZ = (int)(Mathf.Round(hitBlock.z) - diggingChunk.chunkPosition.z); // rather than the gameobjects transform.

                int i = hitBlockX + World.chunkDimensions.x * (hitBlockY + World.chunkDimensions.z * hitBlockZ); //(x + chunkWidth * (y + chunkDepth * z))

                diggingChunk.chunkData[i] = MeshManager.BlockType.Air; // On Left Mouse Click  the block clicked on within its chunk si retextured as an Air Block.
                DestroyImmediate(diggingChunk.GetComponent<MeshFilter>());
                DestroyImmediate(diggingChunk.GetComponent<MeshRenderer>());
                DestroyImmediate(diggingChunk.GetComponent<Collider>());

                diggingChunk.CreateChunk(World.chunkDimensions, diggingChunk.chunkPosition, false);

            }
        }
```
 

 







# List of Classes/Asset With Sources

|Class|Source|
|:---:|:---:|
|Quad.cs|Self Written|
|Block.cs|Self Written|
|MeshManager.cs|Self Written|
|Chunk.cs|Self Written|
|World.cs|Self Written|
|PerlinGraph.cs|Self Written|
|PerlinGraph3D.cs|Self Written|
|FractalBrownianMotion.cs|Self Written|
|FractalBrownianMotion3D.cs|Self Written|
|PlayerManager.cs|Self Written|

|Asset|Source|
|:---:|:---:|
|Block Textures|Self made but with reference to [here](https://minecraft.fandom.com/wiki/List_of_block_textures)|
|Modular First Person Controller  by JeCase|Taken from [here](https://assetstore.unity.com/packages/3d/characters/modular-first-person-controller-189884)|


# What I am most proud of in the assignment 
What I am  most proud from this assignment is learning Perlin Noise, espically Brownnian Motion and learning how to apply to terrain generation. I have always wanted to be able to genrate a voxel cube based game world. I have aslo learned that games that use these voxel terrain systems are far more complex codeing wise than I initally believed. 

