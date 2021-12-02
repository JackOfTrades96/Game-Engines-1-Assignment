using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block
{

    public Mesh mesh;
    Chunk parentChunk;

    public Block(Vector3 offset, MeshManager.BlockType blocktype, Chunk chunk)
    {
        parentChunk = chunk;
        Vector3 blockLocalPosition = offset - chunk.chunkPosition;

        if (blocktype != MeshManager.BlockType.Air)
        {
            List<Quad> quads = new List<Quad>();

            if (!HasSolidNeighbour((int)offset.x, (int)blockLocalPosition.y + 1, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Top, blockLocalPosition, blocktype));

            if (!HasSolidNeighbour((int)offset.x, (int)blockLocalPosition.y - 1, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Bottom, blockLocalPosition, blocktype));

            if (!HasSolidNeighbour((int)offset.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z + 1))
                quads.Add(new Quad(MeshManager.BlockFace.Front, blockLocalPosition, blocktype));

            if (!HasSolidNeighbour((int)offset.x, (int)blockLocalPosition.y, (int)blockLocalPosition.z - 1))
                quads.Add(new Quad(MeshManager.BlockFace.Back, blockLocalPosition, blocktype));

            if (!HasSolidNeighbour((int)offset.x - 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Left, blockLocalPosition, blocktype));

            if (!HasSolidNeighbour((int)offset.x + 1, (int)blockLocalPosition.y, (int)blockLocalPosition.z))
                quads.Add(new Quad(MeshManager.BlockFace.Right, blockLocalPosition, blocktype));
           
           

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
