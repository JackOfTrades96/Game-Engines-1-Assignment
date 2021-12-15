using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Data = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;
// Using a Tuple to contain the vertice, normal and uv data.

//The MeshManager Script sets out each block texture  
public static class MeshManager
{

     public enum BlockType // Using a enum for each block texture which is used in the chunk script to texture each block to its correct block type.
    {
        GrassOnSide, GrassOnTop, Dirt, Stone, Iron, BedRock, Air
    };

    public enum BlockFace { Top, Bottom, Front, Back, Left, Right}; // Using a enum which is used in the quad scripts switch statement to create each quad face.

    // Texture Being Used is 512 x 512. 8x8 textures. 1/ = 0.125
    //First UV is Bottom Right of Texture, Second UV is Bottom Left of Texture
    //Third UV is Top Right of Texture, Fourth UV is Top Left of Texture.

    public static Vector2[,] blockUVs = {

        
        /*GrassOnSide*/ { new Vector2( 0f, 0.9375f ), new Vector2( 0.0625f, 0.9375f),
                        new Vector2( 0f, 1.0f ),new Vector2( 0.0625f, 1.0f )},

        /*GrassOnTop*/ {  new Vector2(0.0625f,0.9375f), new Vector2(0.125f,0.9375f),
                        new Vector2(0.0625f, 1f), new Vector2(0.125f,1f) },

        /*Dirt*/	  { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                        new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},

        /*Stone*/	  { new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                        new Vector2( 0.1875f, 1f ),new Vector2( 0.25f, 1f )},       
      
        /*Iron*/      { new Vector2( 0.25f, 0.9375f ), new Vector2(0.3125f, 0.9375f),
                      new Vector2(0.25f, 1.0f), new Vector2( 0.3125f, 1.0f )},

        /*BedRock*/   { new Vector2( 0.3125f, 0.9375f ), new Vector2( 0.375f, 0.9375f),
                      new Vector2( 0.3125f, 1.0f ),new Vector2( 0.375f, 1.0f )},

        /*Air */      { new Vector2(0f,0f), new Vector2(0f,0f),
                      new Vector2(0f, 0f), new Vector2(0,0f) },

    };

  

   


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

        ExtractArrays(verticesDictionary, blockMesh);
        blockMesh.triangles = triangles.ToArray();
        blockMesh.RecalculateBounds();
        return blockMesh;
    }

    public static void ExtractArrays(Dictionary<Data, int> list, Mesh blockMesh) {
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

}