using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// This script defines and creates  the  6 quads that will make up each block unit that is used in the world.


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
		

		Vector2 uv00 = MeshManager.blockUVs[(int)blockType, 0];
		Vector2 uv10 = MeshManager.blockUVs[(int)blockType, 1];
		Vector2 uv01 = MeshManager.blockUVs[(int)blockType, 2];
		Vector2 uv11 = MeshManager.blockUVs[(int)blockType, 3];

		Vector3 v0 = new Vector3(-0.5f, -0.5f, 0.5f) + vertexOffset;
		Vector3 v1 = new Vector3(0.5f, -0.5f, 0.5f) + vertexOffset;
		Vector3 v2 = new Vector3(0.5f, -0.5f, -0.5f) + vertexOffset;
		Vector3 v3 = new Vector3(-0.5f, -0.5f, -0.5f) + vertexOffset;
		Vector3 v4 = new Vector3(-0.5f, 0.5f, 0.5f) + vertexOffset;
		Vector3 v5 = new Vector3(0.5f, 0.5f, 0.5f) + vertexOffset;
		Vector3 v6 = new Vector3(0.5f, 0.5f, -0.5f) + vertexOffset;
		Vector3 v7 = new Vector3(-0.5f, 0.5f, -0.5f) + vertexOffset;

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

		//setting each qaud vertices, normals ,uvs and triangles to the vaules in the arrays I have created.

		quadMesh.vertices = vertices;
		quadMesh.normals = normals;
		quadMesh.uv = uvs;
		quadMesh.triangles = triangles;

		//Recaluate the bounding volume of the quadmesh 
		quadMesh.RecalculateBounds();
	}
}




