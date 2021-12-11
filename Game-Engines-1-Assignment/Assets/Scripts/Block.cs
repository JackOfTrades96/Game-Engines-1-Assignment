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
            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z)) // if a Block has no neighbouring block at its x,y or z Coords.
            {
                if (blocktype == MeshManager.BlockType.GrassSide)
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, MeshManager.BlockType.GrassTop)); // Set Top quad texture on surface Blocks to Grass.
                else
                    quads.Add(new Quad(MeshManager.BlockFace.Top, offset, blocktype));
            }

            if (!HasSolidNeighbour((int)blockLocalPosition.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z))
            {
                if (blocktype == MeshManager.BlockType.GrassSide)
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, MeshManager.BlockType.GrassSide));
                else
                    quads.Add(new Quad(MeshManager.BlockFace.Bottom, offset, blocktype));
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
