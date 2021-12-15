using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGrapher3D : MonoBehaviour
{
  // Perlin Graph SETTINGS
    public float heightScale = 2;
    public float Scale = 0.5f;
    public int octaves = 1;
    public float heightOffset = 1;
    public float Probability = 1;

    Vector3 CaveDimensions = new Vector3(8,8,8); // 8x8x8 blocks 

    void CreateCaveBlocks()
    {
        for (int z = 0; z < CaveDimensions.z; z++)
        {
            for (int y = 0; y < CaveDimensions.y; y++)
            {
                for (int x = 0; x < CaveDimensions.x; x++)
                {
                    GameObject CaveBlocks = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    CaveBlocks.name = "Perlin_3D_CaveBlock";
                    CaveBlocks.transform.parent = this.transform;
                    CaveBlocks.transform.position = new Vector3(x, y, z);
                }
            }
        }
    }

    void Graph()
    {
        
        MeshRenderer[] cubes = this.GetComponentsInChildren<MeshRenderer>();
        if (cubes.Length == 0)
            CreateCaveBlocks();

        if (cubes.Length == 0) return;

        for (int z = 0; z < CaveDimensions.z; z++)
        {
            for (int y = 0; y < CaveDimensions.y; y++)
            {
                for (int x = 0; x < CaveDimensions.x; x++)
                {
                    float PerlinNoise3D = FractalBrownianMotion3D.fBm3D(x, y, z, octaves, Scale, heightScale, heightOffset);
                    if (PerlinNoise3D < Probability)
                        cubes[x + (int)CaveDimensions.x * (y + (int)CaveDimensions.z * z)].enabled = false;
                    else
                        cubes[x + (int)CaveDimensions.x * (y + (int)CaveDimensions.z * z)].enabled = true;
                }
            }
        }
    }

    void OnValidate()
    {
        Graph();
    }
}
