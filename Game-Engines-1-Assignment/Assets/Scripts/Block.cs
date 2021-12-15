using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{
    public Mesh blockMesh; // block mesh (includes all quad meshes)
    Chunk parentChunk;

    public Block(Vector3 offset, MeshManager.BlockType blocktype, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPosition = offset - chunk.chunkPosition;

        if (blocktype != MeshManager.BlockType.Air) // if blocktype is not equal to Air Block.
        {
            List<Quad> quads = new List<Quad>(); // creates list of quads from Quad class
            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z)) // if a Block has no neighbouring block above it on the y axis.
            {
                if (blocktype == MeshManager.BlockType.GrassOnSide) // if block type is set to GrassOnSide Texture
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, MeshManager.BlockType.GrassOnTop)); // Set Top quad texture on surface Block to GrassOnTop.
                else
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, blocktype)); // else add  quad to blokc and  set the top quad texture to whatever its blocktype states.
            }

            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z)) // if a Block has no neighbouring block below it on the y axis.
            {
                if (blocktype == MeshManager.BlockType.GrassOnSide) // if block type is set to GrassOnSide Texture
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, MeshManager.BlockType.GrassOnSide)); // Set Top quad texture on surface Block to GrassOnSide.
                else
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, blocktype)); // else set the top quad texture to whatever its blocktype states.
            }

            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z + 1)) // if a Block has no neighbouring block in front of it on the z axis.
                quads.Add(new Quad(MeshManager.BlockFace.Front, offset, blocktype));  //  Add  quad to block and  set the Front quad texture to whatever its blocktype states.

            if (!BlockHasNeigbour((int)blockLocalPosition.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z - 1))  // if a Block has no neighbouring block  behind  it on the z axis.
                quads.Add(new Quad(MeshManager.BlockFace.Back, offset, blocktype)); //  Add  quad to block and  set the Back quad texture to whatever its blocktype states.

            if (!BlockHasNeigbour((int)blockLocalPosition.x - 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z)) // if a Block has no neighbouring block to the left of it on the x axis.
                quads.Add(new Quad(MeshManager.BlockFace.Left, offset, blocktype)); //  Add  quad to block and  set the Left quad texture to whatever its blocktype states.

            if (!BlockHasNeigbour((int)blockLocalPosition.x + 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))  // if a Block has no neighbouring block to the right of it on the x axis.
                quads.Add(new Quad(MeshManager.BlockFace.Right, offset, blocktype)); //  Add quad to block and set the Right quad texture to whatever its blocktype states.



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
        if (parentChunk.chunkData[x + parentChunk.chunkWidth * (y + parentChunk.chunkDepth * z)] == MeshManager.BlockType.Air) 
           
            return false;
            return true;
    }
}
