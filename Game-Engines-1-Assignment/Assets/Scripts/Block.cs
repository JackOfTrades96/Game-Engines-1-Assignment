using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{

    public Mesh mesh; // block mesh (includes all quad meshes)
    Chunk parentChunk;

    public Block(Vector3 offset, MeshManager.BlockType blocktype, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPosition = offset - chunk.chunkPosition;

        if (blocktype != MeshManager.BlockType.Air) // if blocktype is not equal to Air Block.
        {
            List<Quad> quads = new List<Quad>(); // creates list of quads from Quad class

            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z)) // if a Block has no neighbouring block at its x,y or z Coords.
            {
                if( blocktype == MeshManager.BlockType.GrassSide ) // if block type  is  textured as Grass Side blocks
                {
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, MeshManager.BlockType.GrassTop)); // Add quads form list and Replace texture them with Grass Top Texture
                }

                else
                {
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, blocktype));  
                }

                
            }

               

            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z))
            {
                if ( blocktype == MeshManager.BlockType.GrassSide)  // if Bottom blocks are textured as Grass Side blocks
                {
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, MeshManager.BlockType.Dirt)); // Replace them with Dirt Texture
                }

                else
                {
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, blocktype));
                }
            }
               

              

            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z + 1))
                quads.Add(new Quad(MeshManager.BlockFace.Front, offset, blocktype));

            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z - 1))
                quads.Add(new Quad(MeshManager.BlockFace.Back, offset, blocktype));

            if (!HasSolidNeighbour((int)blockLocalPosition.x - 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Left, offset, blocktype));

            if (!HasSolidNeighbour((int)blockLocalPosition.x + 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Right, offset, blocktype));
           
           

            if (quads.Count == 0) return;

            Mesh[] sideMeshes = new Mesh[quads.Count];
            int m = 0;
            foreach (Quad q in quads)
            {
                sideMeshes[m] = q.quadMesh;
                m++;
            }

            mesh = MeshManager.MergeMeshes(sideMeshes);
            mesh.name = "Cube_0_0_0";
        }
    }

    public bool HasSolidNeighbour(int x, int y, int z)
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
}
