using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block 
{

    public Mesh blockMesh;

   
   

    // Start is called before the first frame update
    public Block()
    {
        

        Quad[] quads = new Quad[6]; 
        quads[0] = new Quad(MeshManager.BlockFace.Top, new Vector3(0, 0, 0), MeshManager.BlockType.Dirt);
        quads[1] = new Quad(MeshManager.BlockFace.Bottom, new Vector3(0, 0, 0), MeshManager.BlockType.Sand);
        quads[2] = new Quad(MeshManager.BlockFace.Front, new Vector3(0, 0, 0), MeshManager.BlockType.Sand);
        quads[3] = new Quad(MeshManager.BlockFace.Back, new Vector3(0, 0, 0), MeshManager.BlockType.Sand);
        quads[4] = new Quad(MeshManager.BlockFace.Left, new Vector3(0, 0, 0), MeshManager.BlockType.Sand);
        quads[5] = new Quad(MeshManager.BlockFace.Right, new Vector3(0, 0, 0), MeshManager.BlockType.Sand);

        Mesh[] faceMeshes = new Mesh[6];
        faceMeshes[0] = quads[0].quadMesh;
        faceMeshes[1] = quads[1].quadMesh;
        faceMeshes[2] = quads[2].quadMesh;
        faceMeshes[3] = quads[3].quadMesh;
        faceMeshes[4] = quads[4].quadMesh;
        faceMeshes[5] = quads[5].quadMesh;

        blockMesh = MeshManager.MergeMeshes(faceMeshes);
        blockMesh.name = "Block_0_0_0)";

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
