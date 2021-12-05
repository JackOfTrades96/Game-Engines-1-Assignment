using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PerlinGrapher3D : MonoBehaviour
{

    public float heightScale = 2;
    [Range(0.0f, 1.0f)]
    public float Scale = 0.5f;
    public int octaves = 1;
    public float heightOffset = 1;
    [Range(0.0f, 10.0f)]
    public float CutOff = 1f;

    Vector3 CaveDimensions = new Vector3(10, 10, 10);


    void CreateCaveBlocks()
    {
        for (int z = 0; z < CaveDimensions.z; z++)
        {
            for (int y = 0; y < CaveDimensions.y; y++)
            {
                for (int x = 0; x < CaveDimensions.x; x++)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.name = "perlin_cube";
                    cube.transform.parent = this.transform;
                    cube.transform.position = new Vector3(x, y, z);

                }
            }
        }
    }

    void Graph()
    {
        MeshRenderer[] cubes = this.GetComponentsInChildren<MeshRenderer>();
        if (cubes.Length == 0)
        {
            CreateCaveBlocks();
        }

        if (cubes.Length == 0)
            return;

        for (int z = 0; z < CaveDimensions.z; z++)
        {
            for (int y = 0; y < CaveDimensions.y; y++)
            {
                for (int x = 0; x < CaveDimensions.x; x++)
                {
                    float perlin3D = MeshManager.fBm3D(x, y, z, octaves, Scale, heightScale, heightOffset);
                    if (perlin3D < CutOff)
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
