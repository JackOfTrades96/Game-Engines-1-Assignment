using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float heightScale = 2;
    [Range(0.0f, 1.0f)]
    public float Scale = 0.5f;
    [Range(0.0f, 1.0f)]
    public int octaves = 1;
    [Range(0.0f, 1.0f)]
    public float heightOffset = 1;
    [Range(0.0f,1.0f)]
    public float probability = 1;


     void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        Graph();
    }

    float fBm(float x, float z)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i <  octaves; i ++)
        {
            total += Mathf.PerlinNoise(x * Scale * frequency, z * Scale * frequency) * heightScale;
            frequency *= 2; 
        }
        return total;
    }

    void Graph()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;

        int z = 0;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        for(int x = 0; x < lineRenderer.positionCount; x ++)
        {
            float y = MeshManager.fBm(x, z, octaves, Scale, heightScale, heightOffset);
            positions[x] = new Vector3(x, y, z);

        }

        lineRenderer.SetPositions(positions);
    }


     void OnValidate()
    {
        Graph();
    }

}
