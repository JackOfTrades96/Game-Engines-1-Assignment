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

    public static Vector3 worldDimensions = new Vector3(4,24,4); // world dimensions (How many chunks within the world)
    public static Vector3 chunkDimensions = new Vector3(16,16,16); // chunk dimensions (chunkWidth,ChunkHeight,ChunkDepth)
    public GameObject chunkPrefab;
    

    public static PerlinSettings surfaceLayerSettings;
    public PerlinGrapher surfaceLayer;

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
        loadingBar.maxValue = worldDimensions.x * worldDimensions.y * worldDimensions.z; // loadingBar max value set to amount of chunks in world by using the x, y and z values of the world dimensions

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
                    loadingBar.value++;
                    yield return null;

                }
            }
        }

        IntroCamera.SetActive(false);
        player.SetActive(true);

        float xposition = (worldDimensions.x * chunkDimensions.x) /2.0f;
        float zposition = (worldDimensions.z * chunkDimensions.z) / 2.0f;
        Chunk playerChunk = chunkPrefab.GetComponent<Chunk>();
        //float yposition = MeshManager.fBm(xposition, zposition, playerChunk.octaves, playerChunk.Scale, playerChunk.heighScale, playerChunk.heightOffset) + 10;
        player.transform.position = new Vector3(xposition, 100, zposition);
       

      
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
