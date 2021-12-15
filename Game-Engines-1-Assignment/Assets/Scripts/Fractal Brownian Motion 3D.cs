using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class FractalBrownianMotion3D 
{
    public static float fBm3D(float x, float y, float z, int octaves, float scale, float heightScale, float heightOffset)
    {
        // To create 3D perlin noise use the Perlin noise values of each axis.

        float xy = FractalBrownianMotion.fBm(x, y, octaves, scale, heightScale, heightOffset); 
        float xz = FractalBrownianMotion.fBm(x, z, octaves, scale, heightScale, heightOffset);
        float yx = FractalBrownianMotion.fBm(y, x, octaves, scale, heightScale, heightOffset);
        float yz = FractalBrownianMotion.fBm(y, z, octaves, scale, heightScale, heightOffset);
        float zx = FractalBrownianMotion.fBm(z, x, octaves, scale, heightScale, heightOffset);
        float zy = FractalBrownianMotion.fBm(z, y, octaves, scale, heightScale, heightOffset);

        return (xy + yz + xz + yx + zy + zx) / 6.0f; // combine the 6 perlin noises values and divide by 6 to get the Perlin noise average of the area.
    }
}
