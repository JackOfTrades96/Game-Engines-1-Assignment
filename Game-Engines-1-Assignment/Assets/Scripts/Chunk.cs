using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Chunk : MonoBehaviour
{
    public Material Textures;

    public int chunkWidth = 16;
    public int chunkHeight = 256;
    public int chundDepth = 16;

    public Block[,,] blocks;

    // Start is called before the first frame update
    void Start()
    {
        MeshFilter mf = this.gameObject.AddComponent<MeshFilter>();
        MeshRenderer mr = this.gameObject.AddComponent<MeshRenderer>();
        mr.material = Textures;
        blocks = new Block[chunkWidth, chunkHeight, chundDepth];

        for (int z = 0; z< chundDepth; z++)
        {
            for(int y = 0; y < chunkHeight; y++)
            {
                for(int x = 0; x <chunkWidth; x++ )
                {
                    blocks[x, y, z] = new Block(new Vector3(x, y, z), MeshManager.BlockType.Dirt);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
