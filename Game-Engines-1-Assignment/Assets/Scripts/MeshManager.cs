using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;

public static class MeshManager
{
    public enum BlockType
    {
        GrassTop, GrassSide, Dirt, Water, Stone, Sand, Diamond , Air
    };

    public enum BlockFace { Top, Bottom, Front, Back, Left, Right };


    // Texture Being Used is 1280 x 1280. 16x16 textures. 1/16 = 0.0625
    //First Vector is Bottom Right of Texture, Second Vector is Bottom Left of Texture
    //Third Vector is Top Right of Texture, Fourth Vector is Top Left of Texture.
    public static Vector2[,] blockUVs = {
        /*GRASSTOP*/ {  new Vector2(0.0625f,0.9375f), new Vector2(0.125f,0.9375f),
                        new Vector2(0.0625f, 1f), new Vector2(0.125f,1f) },
        /*GRASSSIDE*/ { new Vector2( 0f, 0.9375f ), new Vector2( 0.0625f, 0.9375f),
                        new Vector2( 0f, 1.0f ),new Vector2( 0.0625f, 1.0f )},
        /*DIRT*/	  { new Vector2( 0.125f, 0.9375f ), new Vector2( 0.1875f, 0.9375f),
                        new Vector2( 0.125f, 1.0f ),new Vector2( 0.1875f, 1.0f )},
        /*WATER*/	  { new Vector2(0.875f,0.125f),  new Vector2(0.9375f,0.125f),
                        new Vector2(0.875f,0.1875f), new Vector2(0.9375f,0.1875f)},
        /*STONE*/	  { new Vector2( 0.1875f, 0.9375f ), new Vector2( 0.25f, 0.9375f),
                        new Vector2( 0.1875f, 1f ),new Vector2( 0.25f, 1f )},
        /*SAND*/	  { new Vector2(0.125f,0.875f),  new Vector2(0.1875f,0.875f),
                        new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)},
        /*Diamond*/ { new Vector2( 0.125f, 0.9375f ), new Vector2(0.1875f, 0.9375f),
                      new Vector2(0.125f, 1.0f), new Vector2( 0.1875f, 1.0f )} 

    };

    // FractallBrowningMethod(x,y,octaves,Scale,HeightScale,HeightOffset) >y)
    public static float fBm(float x, float z, int octaves, float Scale, float heightScale, float heightoffset)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * Scale * frequency, z * Scale * frequency) * heightScale;
            frequency *= 2;
        }
        return total + heightoffset;
    }


    public static float fBm3D(float x,float y, float z, int octaves, float Scale, float heightScale, float heightoffset)
    {
        float xy = fBm(x, y, octaves, Scale, heightScale, heightoffset);
        float yx = fBm(y, x, octaves, Scale, heightScale, heightoffset);
        float yz = fBm(y, z, octaves, Scale, heightScale, heightoffset);
        float zy = fBm(z, y, octaves, Scale, heightScale, heightoffset);
        float xz = fBm(x, y, octaves, Scale, heightScale, heightoffset);
        float zx = fBm(z, x, octaves, Scale, heightScale, heightoffset);

        return (xy + yx + yz + zy + xz + zx) /6.0f;

    }


    public static Mesh MergeMeshes(Mesh[] meshes)
    {
        Mesh mesh = new Mesh();

        Dictionary<VertexData, int> pointsOrder = new Dictionary<VertexData, int>();
        HashSet<VertexData> pointsHash = new HashSet<VertexData>();
        List<int> tris = new List<int>();

        int pIndex = 0;
        for (int i = 0; i < meshes.Length; i++) //loop through each mesh
        {
            if (meshes[i] == null) continue;
            for (int j = 0; j < meshes[i].vertices.Length; j++) //loop through each vertex of the current mesh
            {
                Vector3 v = meshes[i].vertices[j];
                Vector3 n = meshes[i].normals[j];
                Vector2 u = meshes[i].uv[j];
                VertexData p = new VertexData(v, n, u);
                if (!pointsHash.Contains(p))
                {
                    pointsOrder.Add(p, pIndex);
                    pointsHash.Add(p);

                    pIndex++;
                }

            }

            for (int t = 0; t < meshes[i].triangles.Length; t++)
            {
                int triPoint = meshes[i].triangles[t];
                Vector3 v = meshes[i].vertices[triPoint];
                Vector3 n = meshes[i].normals[triPoint];
                Vector2 u = meshes[i].uv[triPoint];
                VertexData p = new VertexData(v, n, u);

                int index;
                pointsOrder.TryGetValue(p, out index);
                tris.Add(index);
            }
            meshes[i] = null;
        }

        ExtractArrays(pointsOrder, mesh);
        mesh.triangles = tris.ToArray();
        mesh.RecalculateBounds();
        return mesh;
    }

    public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach (VertexData v in list.Keys)
        {
            verts.Add(v.Item1);
            norms.Add(v.Item2);
            uvs.Add(v.Item3);
        }
        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
    }

}