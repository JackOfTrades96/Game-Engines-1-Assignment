using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VertexData = System.Tuple<UnityEngine.Vector3, UnityEngine.Vector3, UnityEngine.Vector2>;

public static class MeshManager 
{
    public enum BlockFace
    {
        Top,
        Bottom,
        Front,
        Back,
        Left,
        Right
    }
    public enum BlockType
    {
        GrassTop,
        Dirt,
        Sand,
    };

    public static Vector2[,] blockUVs =
    {

        /*GrassTop*/
        {
            new Vector2(0.125f,0.375f),new Vector2(0.1875f,0.375f), new Vector2(0.125f,0.4375f), new Vector2(0.1875f,0.4375f)
        },

        /*Sand*/
        {
            new Vector2(0.125f,0.875f),new Vector2(0.1875f,0.875f), new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)
        },

         /*Sand*/
        {
            new Vector2(0.125f,0.875f),new Vector2(0.1875f,0.875f), new Vector2(0.125f,0.9375f), new Vector2(0.1875f,0.9375f)
        },

    };

    public static Mesh MergeMeshes(Mesh[] meshes)
    {
        Mesh Blockmesh = new Mesh();

        Dictionary<VertexData, int> vertexOrder = new Dictionary<VertexData, int>();
        HashSet<VertexData> vertexHash = new HashSet<VertexData>();
        List<int> triangles = new List<int>();

        int vertexIndex = 0;
        for(int i =0; i < meshes.Length; i++)
        {
            if (meshes[i] == null) continue;
            for (int j =  0; j < meshes[i].vertices.Length; j++)
            {
                Vector3 vertices = meshes[i].vertices[j];
                Vector3 normals = meshes[i].normals[j];
                Vector2 uvs = meshes[i].uv[j];
                VertexData vertex = new VertexData(vertices, normals, uvs);

                if(!vertexHash.Contains(vertex))
                {
                    vertexOrder.Add(vertex, vertexIndex);
                    vertexHash.Add(vertex);

                    vertexIndex++;
                }
            }

            for (int t = 0;  t<meshes[i].triangles.Length; t ++)
            {
                int trianglePoint = meshes[i].triangles[t];
                Vector3 vertices = meshes[i].vertices[trianglePoint];
                Vector3 normals = meshes[i].normals[trianglePoint];
                Vector2 uvs = meshes[i].uv[trianglePoint];
                VertexData vertex = new VertexData(vertices, normals, uvs);

                int index;
                vertexOrder.TryGetValue(vertex, out index);
                triangles.Add(index);
            }

            meshes[i] = null;
        }

        ExtractArrays(vertexOrder, Blockmesh);
        Blockmesh.triangles = triangles.ToArray();
        Blockmesh.RecalculateBounds();
        return Blockmesh;
    }

    public static void ExtractArrays(Dictionary<VertexData, int> list, Mesh mesh)
    {
        List<Vector3> verts = new List<Vector3>();
        List<Vector3> norms = new List<Vector3>();
        List<Vector2> uvs = new List<Vector2>();

        foreach(VertexData vertex in list.Keys)
        {
            verts.Add(vertex.Item1);
            norms.Add(vertex.Item2);
            uvs.Add(vertex.Item3);
        }

        mesh.vertices = verts.ToArray();
        mesh.normals = norms.ToArray();
        mesh.uv = uvs.ToArray();
    }
}
