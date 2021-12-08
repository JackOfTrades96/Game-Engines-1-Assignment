using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public struct PerlinSettings
{
    public float heightScale;
    public float Scale;
    public int octaves;
    public float heightOffset;
    public float probability;

   


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

    public static Vector3Int worldDimensions = new Vector3Int(4,4,4); // world dimensions (How many chunks within the world)
    public static Vector3Int chunkDimensions = new Vector3Int(16,16,16); // chunk dimensions (chunkWidth,ChunkHeight,ChunkDepth)
    public GameObject chunkPrefab;

    HashSet<Vector3Int> chunkCheck = new HashSet<Vector3Int>();
    HashSet<Vector2Int> chunkColumns = new HashSet<Vector2Int>();
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    Vector3Int lastPlayerPosition;
    int drawRadius = 3;

    Queue<IEnumerator> chunkColumnBuildQueue = new Queue<IEnumerator>();

    IEnumerator ChunkBuildCoordinator()
    {
        while(true)
        {
            while (chunkColumnBuildQueue.Count > 0)
                yield return StartCoroutine(chunkColumnBuildQueue.Dequeue());
            yield return null;
        }
    }



    public static PerlinSettings surfaceLayerSettings;
    public PerlinGrapher surfaceLayer;

    //public static PerlinSettings hillsLayerSettings;
    //public PerlinGrapher hillsLayer;

    public static PerlinSettings mountainsLayerSettings;
    //public PerlinGrapher mountainsLayer;

    public static PerlinSettings stoneLayerSettings;
    public PerlinGrapher stoneLayer;

    // public static PerlinSettings cavesLayerSettings;
    // public PerlinGrapher3D cavesLayer;

    // public static PerlinSettings coalTopLayerSettings;
    // public PerlinGrapher coalTopLayer;

    // public static PerlinSettings coalBottomLayerSettings;
    //public PerlinGrapher coalBottomLayer;

    public static PerlinSettings bedrockLayerSettings;
    public PerlinGrapher bedrockLayer;

    

    public GameObject IntroCamera;
    public GameObject player;
    public Slider loadingBar;


    void Start()
    {
       
        surfaceLayerSettings = new PerlinSettings(surfaceLayer.heightScale, surfaceLayer.Scale, surfaceLayer.octaves, surfaceLayer.heightOffset, surfaceLayer.probability);
        stoneLayerSettings = new PerlinSettings(stoneLayer.heightScale, stoneLayer.Scale, stoneLayer.octaves, stoneLayer.heightOffset, stoneLayer.probability);
        //cavesLayerSettings = new PerlinSettings(cavesLayer.heightScale, cavesLayer.Scale, cavesLayer.octaves, cavesLayer.heightOffset, cavesLayer.CutOff);
        //coalTopLayerSettings = new PerlinSettings(coalTopLayer.heightScale, coalTopLayer.Scale, coalTopLayer.octaves, coalTopLayer.heightOffset, coalTopLayer.probability);
        //coalBottomLayerSettings = new PerlinSettings(coalBottomLayer.heightScale, coalBottomLayer.Scale, coalBottomLayer.octaves, coalBottomLayer.heightOffset, coalBottomLayer.probability);

        StartCoroutine(BuildWorld());
        loadingBar.maxValue = worldDimensions.x * worldDimensions.z; // loadingBar max value set to amount of chunks in world by using the x, y and z values of the world dimensions

    }

    void BuildChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int chunkPosition = new Vector3Int(x , y * chunkDimensions.y, z );

            if(!chunkCheck.Contains(chunkPosition))
            {
                GameObject chunk = Instantiate(chunkPrefab);

                chunk.name = "Chunk: " + chunkPosition.x + chunkPosition.y + chunkPosition.z;
                Chunk c = chunk.GetComponent<Chunk>();
                c.CreateChunk(chunkDimensions, chunkPosition);
                chunkCheck.Add(chunkPosition);
                chunks.Add(chunkPosition, c);
            }

            else
            {
                chunks[chunkPosition].meshRenderer.enabled = true;
            }
           
        }

        chunkColumns.Add(new Vector2Int(x, z));
    }


    // Corotuine is necessary to allow world to be built and render without any crashes
    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
                for (int x = 0; x < worldDimensions.x; x++)
                {
                    BuildChunkColumn(x * chunkDimensions.x,z * chunkDimensions.z);
                    loadingBar.value++; 
                    yield return null;

                }
        }

        IntroCamera.SetActive(false);
        player.SetActive(true);

        float xposition = (worldDimensions.x * chunkDimensions.x) /2.0f;
        float zposition = (worldDimensions.z * chunkDimensions.z) / 2.0f;
        Chunk playerChunk = chunkPrefab.GetComponent<Chunk>();
        player.transform.position = new Vector3(xposition, 100, zposition);

        lastPlayerPosition = Vector3Int.CeilToInt(player.transform.position);
        StartCoroutine(ChunkBuildCoordinator());
        StartCoroutine(UpdateWorld());
      
    }

    IEnumerator BuildWorld(int x, int z, int rad)
    {
        int nextRad = rad - 1;
        if (rad <= 0) yield break;

        BuildChunkColumn(x + chunkDimensions.x, z);
        chunkColumnBuildQueue.Enqueue(BuildWorld(x, z + chunkDimensions.z, nextRad));

        BuildChunkColumn(x - chunkDimensions.x, z );
        chunkColumnBuildQueue.Enqueue(BuildWorld(x, z + chunkDimensions.z, nextRad));

        BuildChunkColumn(x, z + chunkDimensions.z);
        chunkColumnBuildQueue.Enqueue(BuildWorld(x, z + chunkDimensions.z, nextRad));

        BuildChunkColumn(x, z - chunkDimensions.z);
        chunkColumnBuildQueue.Enqueue(BuildWorld(x, z + chunkDimensions.z, nextRad));
        yield return null;
    }

    WaitForSeconds UpdateWorldPause = new WaitForSeconds(0.5f);

    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if((lastPlayerPosition - player.transform.position).magnitude > chunkDimensions.x)
            {

                lastPlayerPosition = Vector3Int.CeilToInt(player.transform.position);
                int LastPlayerPositionX = (int)(player.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int LastPlayerPositionZ = (int)(player.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                chunkColumnBuildQueue.Enqueue(BuildWorld(LastPlayerPositionX, LastPlayerPositionZ, drawRadius));
                chunkColumnBuildQueue.Enqueue(Hide_Chunk_Columns(LastPlayerPositionX,LastPlayerPositionZ));
            }

            yield return UpdateWorldPause;
        }
    }

    public void HideChunkColumn(int x, int z)
    {
        for (int y = 0; y < worldDimensions.y; y++)
        {
            Vector3Int chunkPosition = new Vector3Int(x, y * chunkDimensions.y, z);
            if (chunkCheck.Contains(chunkPosition))
            {
                chunks[chunkPosition].meshRenderer.enabled = false;
            }

        }
    }


    IEnumerator Hide_Chunk_Columns(int x, int z)
    {
        Vector2Int playerPosition = new Vector2Int(x, z);

        foreach(Vector2Int chunkcolumn in chunkColumns )
        {
            if((chunkcolumn - playerPosition).magnitude >= drawRadius * chunkDimensions.x)
            {
                HideChunkColumn(chunkcolumn.x,chunkcolumn.y);
            }
        }


        yield return null;
    }


  
}
