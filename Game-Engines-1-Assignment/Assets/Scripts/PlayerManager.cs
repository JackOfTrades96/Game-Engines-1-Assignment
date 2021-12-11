using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerManager : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

        PlayerDig();

    }

    public void PlayerDig()
    {
        if (Input.GetMouseButtonDown(0))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit, 20))
            {
                Vector3 hitBlock = Vector3.zero;

                if (Input.GetMouseButtonDown(0))
                {
                    hitBlock = hit.point - hit.normal / 2.0f;
                }


                Debug.Log("Block " + hitBlock.x + "," + hitBlock.y + "," + hitBlock.z);
                Chunk diggingChunk = hit.collider.gameObject.GetComponent<Chunk>();

                int hitBlockX = (int)(Mathf.Round(hitBlock.x) - diggingChunk.chunkPosition.x); // Getting the block location
                int hitBlockY = (int)(Mathf.Round(hitBlock.y) - diggingChunk.chunkPosition.y); // within a chunk by using chunkposition of chunk 
                int hitBlockZ = (int)(Mathf.Round(hitBlock.z) - diggingChunk.chunkPosition.z); // rather than the gameobjects transform.

                int i = hitBlockX + World.chunkDimensions.x * (hitBlockY + World.chunkDimensions.z * hitBlockZ); //(x + chunkWidth * (y + chunkDepth * z))

                diggingChunk.chunkData[i] = MeshManager.BlockType.Air; // On Left Mouse Click set the block clicked on within its chunk to have the air texture.
                DestroyImmediate(diggingChunk.GetComponent<MeshFilter>());
                DestroyImmediate(diggingChunk.GetComponent<MeshRenderer>());
                DestroyImmediate(diggingChunk.GetComponent<Collider>());

                diggingChunk.CreateChunk(World.chunkDimensions, diggingChunk.chunkPosition, false);

            }
        }
    }

}

