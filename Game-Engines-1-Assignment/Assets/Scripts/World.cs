using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PerlinSettings
{
    public float heightScale; //fBm heighScale
    public float Scale; //  fBm noise Scale 
    public int octaves; // fBm octaves
    public float heightOffset; // fbm heighOffset
    public float probability; // fbm probability

    public PerlinSettings(float HS, float S, int O, float HO, float P)
    {
        heightScale = HS;
        Scale = S;
        octaves = O;
        heightOffset = HO;
        probability = P;
    }
}


public class World : MonoBehaviour
{
    public static Vector3Int worldDimensions = new Vector3Int(8, 8, 8); // the world Dimensions realtive to chunks (8 chunks on x axis, 8 chunks on y axis, 8 chunks on the z axis)
    public static Vector3Int chunkDimensions = new Vector3Int(16, 16, 16); //the chunk Dimensions relactive to the blocks (16 blocks on x axis in chunk, 16 blocks on the y axis, 16 blocks on the z axis)
    public GameObject chunkPrefab; // The chunk prebaf being called when the chunks are being created.
    public GameObject IntroCamera; // first camera used while world is loading
    public GameObject Player; 
    public Slider loadingBar; // shows the prohress of the worldchunks being loaded

    // Perlin Noise Graphs  and the settings for each layer of terrain within the world.

    public static PerlinSettings mountainLayerSettings;
    public PerlinGrapher mountainLayer;

    public static PerlinSettings dirtLayerSettings;
    public PerlinGrapher dirtLayer;

    public static PerlinSettings stoneLayerSettings;
    public PerlinGrapher stoneLayer;

    public static PerlinSettings ironTopLayerSettings;
    public PerlinGrapher ironTopLayer;

    public static PerlinSettings ironBottomLayerSettings;
    public PerlinGrapher ironBottomLayer;

    public static PerlinSettings caveLayerSettings;
    public PerlinGrapher3D caveLayer;


    HashSet<Vector3Int> chunkCheck = new HashSet<Vector3Int>();
    HashSet<Vector2Int> chunkPrefabs = new HashSet<Vector2Int>();
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    Vector3Int lastPlayerPosition;
    int distanceFromChunk = 3;

    Queue<IEnumerator> chunkPrefabBuildQueue = new Queue<IEnumerator>();

    IEnumerator ChunkBuildCoordinator()
    {
        while (true)
        {
            while (chunkPrefabBuildQueue.Count > 0)
                yield return StartCoroutine(chunkPrefabBuildQueue.Dequeue());
            yield return null;
        }
    }


  
    void Start()
    {
        // Loading bar total value is equal to amount to blocks in the world in the x axis multple by amount to blocks in the world in the z axis

        loadingBar.maxValue = worldDimensions.x * worldDimensions.z;

        //Setting each Perlin Setting to its correct values

        mountainLayerSettings = new PerlinSettings(mountainLayer.heightScale, mountainLayer.Scale,
                                     mountainLayer.octaves, mountainLayer.heightOffset, mountainLayer.probability);

        dirtLayerSettings = new PerlinSettings(dirtLayer.heightScale, dirtLayer.Scale,
                                     dirtLayer.octaves, dirtLayer.heightOffset, dirtLayer.probability);

        stoneLayerSettings = new PerlinSettings(stoneLayer.heightScale, stoneLayer.Scale,
                             stoneLayer.octaves, stoneLayer.heightOffset, stoneLayer.probability);

        ironTopLayerSettings = new PerlinSettings(ironBottomLayer.heightScale, ironBottomLayer.Scale,
                               ironBottomLayer.octaves, ironBottomLayer.heightOffset, ironBottomLayer.probability);

        ironBottomLayerSettings = new PerlinSettings(ironBottomLayer.heightScale, ironBottomLayer.Scale,
                               ironBottomLayer.octaves, ironBottomLayer.heightOffset, ironBottomLayer.probability);

        caveLayerSettings = new PerlinSettings(caveLayer.heightScale, caveLayer.Scale,
             caveLayer.octaves, caveLayer.heightOffset, caveLayer.Probability);

      // BeginCorotuine to generate world.
        StartCoroutine(BuildWorld());
    }

 
    // Function that controls how the  chunk prefabs are  built. Use for  loop  with the worlds y values to genreat each chunk on top of each other
    void BuildChunkPrefabs(int x, int z, bool meshEnabled = true)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int position = new Vector3Int(x, y * chunkDimensions.y, z);
            if (!chunkCheck.Contains(position))
            {
                GameObject chunk = Instantiate(chunkPrefab);
                chunk.name = "Chunk_" + position.x + "_" + position.y + "_" + position.z;
                Chunk c = chunk.GetComponent<Chunk>();
                c.CreateChunk(chunkDimensions, position);
                chunkCheck.Add(position);
                chunks.Add(position, c);
            }
            chunks[position].meshRenderer.enabled = meshEnabled;


        }
        chunkPrefabs.Add(new Vector2Int(x, z));
    }

 



    //Coroutine that controls building the game world. Uses a nested for loop with the x and z world values to generate each chunk within the world. 
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++)
            {
                BuildChunkPrefabs(x * chunkDimensions.x, z * chunkDimensions.z);
                loadingBar.value++;
                yield return null;
            }

        }

        int xpos = (worldDimensions.x * chunkDimensions.x) / 2;
        int zpos = (worldDimensions.z * chunkDimensions.z) / 2;

        loadingBar.gameObject.SetActive(false); //Set Loadingbar false
        IntroCamera.SetActive(false); // Camera is switched to player view once world has been loaded
        Player.SetActive(true); // Set player Prefab Active.
        Player.transform.position = new Vector3(xpos, 100, zpos); //Spawn Player
     
        lastPlayerPosition = Vector3Int.CeilToInt(Player.transform.position); // Player Position Vector3Int that is used to enable/disalbe the chunk mesh renderers.
        StartCoroutine(ChunkBuildCoordinator()); // Build Chunks in world
        StartCoroutine(UpdateWorld()); // Update the world chunks.
       
    }

    WaitForSeconds UpdateWorldPause = new WaitForSeconds(0.5f);

    // Coroutine that Updates the world every (0.5 seconds)
    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastPlayerPosition - Player.transform.position).magnitude > (chunkDimensions.x))
            {
                lastPlayerPosition = Vector3Int.CeilToInt(Player.transform.position);
                int posx = (int)(Player.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int posz = (int)(Player.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(posx, posz, distanceFromChunk));
                chunkPrefabBuildQueue.Enqueue(Hide_Chunk_Prefabs(posx, posz));
            }
            yield return UpdateWorldPause;
        }
    }

    //Meshrenderer of a chunk i set to false when player distance from chunk is greater than the distance form the chunk.

    public void HideChunkPrefabs(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int pos = new Vector3Int(x, y * chunkDimensions.y, z);
            if (chunkCheck.Contains(pos))
            {
                chunks[pos].meshRenderer.enabled = false;
            }
        }
    }

    IEnumerator Hide_Chunk_Prefabs(int x, int z)
    {
        Vector2Int playerPosition = new Vector2Int(x, z);
        foreach (Vector2Int cc in chunkPrefabs)
        {
            if ((cc - playerPosition).magnitude >= distanceFromChunk * chunkDimensions.x)
            {
                HideChunkPrefabs(cc.x, cc.y);
            }
        }
        yield return null;
    }


    IEnumerator BuildHiddenWorld(int x, int z, int rad)
    {
        int nextrad = rad - 1;
        if (rad <= 0) yield break;

        BuildChunkPrefabs(x, z + chunkDimensions.z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x, z + chunkDimensions.z, nextrad));
        yield return null;

        BuildChunkPrefabs(x, z - chunkDimensions.z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x, z - chunkDimensions.z, nextrad));
        yield return null;

        BuildChunkPrefabs(x + chunkDimensions.x, z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x + chunkDimensions.x, z, nextrad));
        yield return null;

        BuildChunkPrefabs(x - chunkDimensions.x, z);
        chunkPrefabBuildQueue.Enqueue(BuildHiddenWorld(x - chunkDimensions.x, z, nextrad));
        yield return null;
    }

}


