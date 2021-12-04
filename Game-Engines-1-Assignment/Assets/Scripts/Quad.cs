using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Quad
{
    public Mesh quadMesh; //  quad mesh that is used to create the voxel blocks.

    public Quad(MeshManager.BlockFace face, Vector3 offset, MeshManager.BlockType blockType)
    {

        quadMesh = new Mesh(); 
        
        // Quads are made up of  vertex arrays,  normals  texture Coords(uvs) &  triangles.

        Vector3[] vertices = new Vector3[4]; // Vecrtices Array
        Vector3[] normals = new Vector3[4]; // Normals Array
        Vector2[] uvs = new Vector2[4]; // Uvs Array
        int[] triangles = new int[6]; // Triangle Array
        triangles = new int[] { 3, 1, 0, 3, 2, 1 };

        // UV Array
        Vector2 uv00 = MeshManager.blockUVs[(int)blockType, 0];
        Vector2 uv10 = MeshManager.blockUVs[(int)blockType, 1];
        Vector2 uv01 = MeshManager.blockUVs[(int)blockType, 2];
        Vector2 uv11 = MeshManager.blockUVs[(int)blockType, 3];

        //Vertices Array
        Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f) + offset;
        Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f) + offset;
        Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f) + offset;
        Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f) + offset;
        Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f) + offset;
        Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f) + offset;
        Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f) + offset;
        Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f) + offset;

        switch (face) // switch statemnet creates  the 6 quads that form each voxel Block (Top,Bottom, Front, Back, Left, Right)
        {

            case MeshManager.BlockFace.Top:
                {
                    vertices = new Vector3[] { v7, v6, v5, v4 }; // setting the  vertices for  the Top Quad
                    normals = new Vector3[] { Vector3.up, Vector3.up, Vector3.up, Vector3.up }; // setting the  normals for  the Top Quad
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; // setting the uvs  for the Top Quad
                    break;

                }

            case MeshManager.BlockFace.Bottom:
                {
                    vertices = new Vector3[] { v0, v1, v2, v3 }; // setting the  vertices for the Bottom Quad
                    normals = new Vector3[] { Vector3.down, Vector3.down, Vector3.down, Vector3.down }; // setting the   normals for  the Bottom Quad
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; // setting the uvs for the Bottom Quad
                    break;

                }


            case MeshManager.BlockFace.Front:
                {
                    vertices = new Vector3[] { v4, v5, v1, v0 }; // setting the  vertices for  the Front Quad
                    normals = new Vector3[] { Vector3.forward, Vector3.forward, Vector3.forward, Vector3.forward };  // setting the  normals for  the Front Quad
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; // setting the uvs for the Front Quad
                    break;

                }

            case MeshManager.BlockFace.Back:
                {
                    vertices = new Vector3[] { v6, v7, v3, v2 }; // setting the  vertices for  the Back Quad
                    normals = new Vector3[] { Vector3.back, Vector3.back, Vector3.back, Vector3.back }; // setting the  normals for  the Back Quad
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; // setting the uvs for the Back Quad
                    break;

                }

            case MeshManager.BlockFace.Left:
                {
                    vertices = new Vector3[] { v7, v4, v0, v3 }; // setting the  vertices for  the Left Quad
                    normals = new Vector3[] { Vector3.left, Vector3.left, Vector3.left, Vector3.left }; // setting the  normals for  the Left Quad
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; // setting the  uvs for the Left Quad
                    break;

                }

            case MeshManager.BlockFace.Right:
                {
                    vertices = new Vector3[] { v5, v6, v2, v1 };  // setting the  vertices for  the Right Quad
                    normals = new Vector3[] { Vector3.right, Vector3.right, Vector3.right, Vector3.right };  // setting the  normals for  the Right Quad
                    uvs = new Vector2[] { uv11, uv01, uv00, uv10 }; // setting the uvs for the Right Quad
                    break;

                }

        }


        quadMesh.vertices = vertices; // setting the quad vertices to the array that have been created
        quadMesh.normals = normals; // setting the quad normals to the array that have been created
        quadMesh.uv = uvs;  // setting the quad uvs to the array that have been created
        quadMesh.triangles = triangles;  // setting the quad triangles to the array that have been created

        quadMesh.RecalculateBounds(); //  finding the new limits of each quad mesh created.




    }

}

