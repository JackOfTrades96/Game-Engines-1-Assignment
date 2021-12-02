using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{

    public static Vector3 worldDimensions = new Vector3(3,3,3); // world dimensions (How many chunks within the world)
    public static Vector3 chunkDimensions = new Vector3(10,10,10); // chunk dimensions (chunkWidth,ChunkHeight,ChunkDepth)
    public GameObject chunkPrefab;

    void Start()
    {
        StartCoroutine(BuildWorld());
    }

    // Corotuine is necessary to allow world to be built and render without any crashes
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int y = 0; y < worldDimensions.y; y++)
            {
                for (int x = 0; x < worldDimensions.x; x++)
                {
                    GameObject chunk = Instantiate(chunkPrefab);
                    Vector3 chunkPosition = new Vector3(x * chunkDimensions.x, y * chunkDimensions.y, z * chunkDimensions.z);
                    chunk.GetComponent<Chunk>().CreateChunk(chunkDimensions, chunkPosition);
                    yield return null;

                }
            }
        }

    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
