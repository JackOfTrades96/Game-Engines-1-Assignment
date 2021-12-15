using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FractalBrownianMotion 
{
    public static float fBm(float x, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        float total = 0;
        float frequency = 1;
        for (int i = 0; i < octaves; i++)
        {
            total += Mathf.PerlinNoise(x * scale * frequency, z * scale * frequency) * heightScale;
            frequency *= 2;
        }
        return total + heightOffset; // in order for the return value to be used wherever the Perlin graph is drawn.
    }
}
