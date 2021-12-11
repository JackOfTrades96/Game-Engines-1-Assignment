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
    public static Vector3Int worldDimensions = new Vector3Int(8, 16, 8);
    //public static Vector3Int backgroundWorldDimensions = new Vector3Int(4, 16, 4);
    public static Vector3Int chunkDimensions = new Vector3Int(16, 16, 16);
    public GameObject chunkPrefab;
    public GameObject IntroCamera;
    public GameObject Player;
    public Slider loadingBar;

    public static PerlinSettings dirtLayerSettings;
    public PerlinGrapher dirtLayer;

    public static PerlinSettings stoneLayerSettings;
    public PerlinGrapher stoneLayer;

    public static PerlinSettings coalTopLayerSettings;
    public PerlinGrapher coalTopLayer;

    public static PerlinSettings coalBottomLayerSettings;
    public PerlinGrapher coalBottomLayer;

    public static PerlinSettings ironTopLayerSettings;
    public PerlinGrapher ironTopLayer;

    public static PerlinSettings ironBottomLayerSettings;
    public PerlinGrapher ironBottomLayer;

    public static PerlinSettings goldTopLayerSettings;
    public PerlinGrapher goldTopLayer;

    public static PerlinSettings goldBottomLayerSettings;
    public PerlinGrapher goldBottomLayer;

    public static PerlinSettings diamondTopLayerSettings;
    public PerlinGrapher diamondTopLayer;

    public static PerlinSettings diamondBottomLayerSettings;
    public PerlinGrapher diamondBottomLayer;


    public static PerlinSettings caveLayerSettings;
    public PerlinGrapher3D caveLayer;

    HashSet<Vector3Int> chunkCheck = new HashSet<Vector3Int>();
    HashSet<Vector2Int> chunkColumns = new HashSet<Vector2Int>();
    Dictionary<Vector3Int, Chunk> chunks = new Dictionary<Vector3Int, Chunk>();

    Vector3Int lastPlayerPosition;
    int drawRadius = 3;

    Queue<IEnumerator> chunkColumnBuildQueue = new Queue<IEnumerator>();

    IEnumerator ChunkBuildCoordinator()
    {
        while (true)
        {
            while (chunkColumnBuildQueue.Count > 0)
                yield return StartCoroutine(chunkColumnBuildQueue.Dequeue());
            yield return null;
        }
    }


  
    void Start()
    {
        loadingBar.maxValue = worldDimensions.x * worldDimensions.z;

        dirtLayerSettings = new PerlinSettings(dirtLayer.heightScale, dirtLayer.scale,
                                     dirtLayer.octaves, dirtLayer.heightOffset, dirtLayer.probability);

        stoneLayerSettings = new PerlinSettings(stoneLayer.heightScale, stoneLayer.scale,
                             stoneLayer.octaves, stoneLayer.heightOffset, stoneLayer.probability);

        coalTopLayerSettings = new PerlinSettings(coalTopLayer.heightScale, coalTopLayer.scale,
                     coalTopLayer.octaves, coalTopLayer.heightOffset, coalTopLayer.probability);

        coalBottomLayerSettings = new PerlinSettings(coalBottomLayer.heightScale, coalBottomLayer.scale,
                     coalBottomLayer.octaves, coalBottomLayer.heightOffset, coalBottomLayer.probability);

        caveLayerSettings = new PerlinSettings(caveLayer.heightScale, caveLayer.scale,
             caveLayer.octaves, caveLayer.heightOffset, caveLayer.DrawCutOff);

        StartCoroutine(BuildWorld());
    }

    private void Update()
    {
       
    }


    void BuildChunkColumn(int x, int z, bool meshEnabled = true)
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
        chunkColumns.Add(new Vector2Int(x, z));
    }

  /*  IEnumerator BuildExtraWorld()
    {
        int zEnd = worldDimensions.z + backgroundWorldDimensions.z;
        int zStart = worldDimensions.z - 1;
        int xEnd = worldDimensions.x + backgroundWorldDimensions.x;
        int xStart = worldDimensions.x - 1;


        for (int z = zStart; z < zEnd; z++)
        {
            for (int x = 0; x < xEnd; x++)
            {
                BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z, false);
                yield return null;
            }

        }

        for (int z = 0; z < zEnd; z++)
        {
            for (int x = xStart; x < xEnd; x++)
            {
                BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z, false);
                yield return null;
            }

        }


    }
  */


    IEnumerator BuildWorld()
    {
        for (int z = 0; z < worldDimensions.z; z++)
        {
            for (int x = 0; x < worldDimensions.x; x++)
            {
                BuildChunkColumn(x * chunkDimensions.x, z * chunkDimensions.z);
                loadingBar.value++;
                yield return null;
            }

        }

        IntroCamera.SetActive(false);
     

        int xpos = (worldDimensions.x * chunkDimensions.x) / 2;
        int zpos = (worldDimensions.z * chunkDimensions.z) / 2;

        
        Player.transform.position = new Vector3(xpos, 100, zpos);
        loadingBar.gameObject.SetActive(false);
        Player.SetActive(true);
        lastPlayerPosition = Vector3Int.CeilToInt(Player.transform.position);
        StartCoroutine(ChunkBuildCoordinator());
        StartCoroutine(UpdateWorld());
        //StartCoroutine(BuildExtraWorld());
    }

    WaitForSeconds UpdateWorldPause = new WaitForSeconds(0.5f);
    IEnumerator UpdateWorld()
    {
        while (true)
        {
            if ((lastPlayerPosition - Player.transform.position).magnitude > (chunkDimensions.x))
            {
                lastPlayerPosition = Vector3Int.CeilToInt(Player.transform.position);
                int posx = (int)(Player.transform.position.x / chunkDimensions.x) * chunkDimensions.x;
                int posz = (int)(Player.transform.position.z / chunkDimensions.z) * chunkDimensions.z;
                chunkColumnBuildQueue.Enqueue(BuildRecursiveWorld(posx, posz, drawRadius));
                chunkColumnBuildQueue.Enqueue(Hide_Chunk_Columns(posx, posz));
            }
            yield return UpdateWorldPause;
        }
    }

    public void HideChunkColumn(int x, int z)
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

    IEnumerator Hide_Chunk_Columns(int x, int z)
    {
        Vector2Int playerPosition = new Vector2Int(x, z);
        foreach (Vector2Int cc in chunkColumns)
        {
            if ((cc - playerPosition).magnitude >= drawRadius * chunkDimensions.x)
            {
                HideChunkColumn(cc.x, cc.y);
            }
        }
        yield return null;
    }

    IEnumerator BuildRecursiveWorld(int x, int z, int rad)
    {
        int nextrad = rad - 1;
        if (rad <= 0) yield break;

        BuildChunkColumn(x, z + chunkDimensions.z);
        chunkColumnBuildQueue.Enqueue(BuildRecursiveWorld(x, z + chunkDimensions.z, nextrad));
        yield return null;

        BuildChunkColumn(x, z - chunkDimensions.z);
        chunkColumnBuildQueue.Enqueue(BuildRecursiveWorld(x, z - chunkDimensions.z, nextrad));
        yield return null;

        BuildChunkColumn(x + chunkDimensions.x, z);
        chunkColumnBuildQueue.Enqueue(BuildRecursiveWorld(x + chunkDimensions.x, z, nextrad));
        yield return null;

        BuildChunkColumn(x - chunkDimensions.x, z);
        chunkColumnBuildQueue.Enqueue(BuildRecursiveWorld(x - chunkDimensions.x, z, nextrad));
        yield return null;
    }

}


