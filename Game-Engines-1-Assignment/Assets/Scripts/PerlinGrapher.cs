using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]

public class PerlinGrapher : MonoBehaviour
{
    public LineRenderer lineRenderer;
    public float heightScale = 2; // Adjust height of the Perlin Graph.
    public float Scale = 0.5f; // The Perlin Noise Scale.
    public int octaves = 1;
    public float heightOffset = 1; 
    public float probability = 1; // The probality of the block texture being rendered.
   
    void Start()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        Graph();
    }

    
    // Graph Function draws the linerender to show the Perlin Settings 
    void Graph()
    {
        lineRenderer = this.GetComponent<LineRenderer>();
        lineRenderer.positionCount = 100;
        int z = 0;
        Vector3[] positions = new Vector3[lineRenderer.positionCount];
        for (int x = 0; x < lineRenderer.positionCount; x++)
        {
            float y = FractalBrownianMotion.fBm(x, z, octaves, Scale, heightScale, heightOffset);
            positions[x] = new Vector3(x, y, z);
        }
        lineRenderer.SetPositions(positions);
    }

    void OnValidate()
    {
        Graph();
    }

}
