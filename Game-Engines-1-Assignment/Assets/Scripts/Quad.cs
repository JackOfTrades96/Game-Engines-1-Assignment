using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad
{
    public Mesh quadMesh;

    public Quad(MeshManager.BlockFace face, Vector3 offset, MeshManager.BlockType blockType)
    {

        quadMesh = new Mesh();
        quadMesh.name = "ScriptableQuad";

        Vector3[] vertices = new Vector3[5];
        Vector3[] normals = new Vector3[4];
        Vector2[] uvs = new Vector2[4];
        int[] triangles = new int[6];
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };

        Vector2 uv00 = MeshManager.blockUVs[(int)blockType, 0];
        Vector2 uv10 = MeshManager.blockUVs[(int)blockType, 1];
        Vector2 uv01 = MeshManager.blockUVs[(int)blockType, 2];
        Vector2 uv11 = MeshManager.blockUVs[(int)blockType, 3];

        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
        Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
        Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
        Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
        Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
        Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;

        switch (face)
        {

            case MeshManager.BlockFace.Top:
                {
                    vertices = new Vector3[] { v7, v6, v5, v4 };
                    normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    break;

                }

            case MeshManager.BlockFace.Bottom:
                {
                    vertices = new Vector3[] { v0, v1, v2, v3 };
                    normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    break;

                }


            case MeshManager.BlockFace.Front:
                {
                    vertices = new Vector3[] { v4, v5, v1, v0 };
                    normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    break;

                }

            case MeshManager.BlockFace.Back:
                {
                    vertices = new Vector3[] { v6, v7, v3, v2 };
                    normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    break;

                }

            case MeshManager.BlockFace.Left:
                {
                    vertices = new Vector3[] { v7, v4, v0, v3 };
                    normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    break;

                }

            case MeshManager.BlockFace.Right:
                {
                    vertices = new Vector3[] { v5, v6, v2, v1 };
                    normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 };
                    break;

                }

        }


        quadMesh.vertices = vertices;
        quadMesh.normals = normals;
        quadMesh.uv = uvs;
        quadMesh.triangles = triangles;

        quadMesh.RecalculateBounds();




    }

}

