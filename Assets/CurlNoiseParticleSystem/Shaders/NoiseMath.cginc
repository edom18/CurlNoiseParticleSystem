#ifndef __NOISEMATH_CGINC__
#define __NOISEMATH_CGINC__

#include "Define.cginc"

float Noise(float3 vec);

// Ramp関数
float Ramp(float r)
{
    if (r >= 1.0)
    {
        return 1.0;
    }
    else if (r <= -1.0)
    {
        return -1.0;
    }
    else
    {
        // ((15.0 / 8.0) * r) - ((10.0 / 8.0) * (r * r * r)) + ((3.0 / 8.0) * (r * r * r * r * r))
        return (1.875 * r) - (1.25 * (r * r * r)) + (0.375 * (r * r * r * r * r));
    }
}

float Fade(float t)
{
    return t * t * t * (t * (t * 6.0 - 15.0) + 10.0);
}

float Lerp(float t, float a, float b)
{
    return a + t * (b - a);
}

float Grad(int hash, float x, float y, float z)
{
    int h = hash & 15;
    float u = (h < 8) ? x : y;
    float v = (h < 4) ? y : (h == 12 || h == 14) ? x : z;
    return ((h & 1) == 0 ? u : -u) + ((h & 2) == 0 ? v : -v);
}

inline float rand(float2 seed)
{
    return frac(sin(dot(seed.xy, float2(12.9898, 78.233))) * 43758.5453);
}

inline float3 rand3(float2 seed)
{
    return 2.0 * (float3(rand(seed * 1), rand(seed * 2), rand(seed * 3)) - 0.5);
}

inline float randRange(float2 seed, float minr, float maxr)
{
    float t = (rand(seed) + 1.0) * 0.5;
    return minr * (1.0 - t) + maxr * t;
}

/// Squre（平方）
inline float sqr(float x)
{
    return x * x;
}

/// ふたつのベクトルを合成する
float3 Constraint(float3 potential, float3 normal, float alpha)
{
    // (N・ψ(X))を計算する
    float dp = abs(dot(potential, normal));

    // ψ_constrained(X) = αψ(X) + (1 - α)N(N・ψ(X))
    return (alpha * potential) + ((1.0 - alpha) * dp * normal);
}

/// パーリンノイズ
float PerlinNoise(float3 vec, float octaves)
{
    float result = 0;
    float amp = 1.0;

    result += Noise(vec) * amp;
    vec *= 2.0;
    amp *= 0.5;

    for (int i = 0; i < octaves; i++)
    {
        result += Noise(vec) * amp;
        vec *= 2.0;
        amp *= 0.5;
    }

    return result;
}

/// パーリンノイズによるベクトル場
/// 3Dとして3要素を計算。
/// それぞれのノイズは明らかに違う（極端に大きなオフセット）を持たせた値とする
float3 Pnoise(float3 vec, float octaves)
{
    float x = PerlinNoise(vec, octaves);

    float y = PerlinNoise(float3(
        vec.y + 31.416,
        vec.z - 47.853,
        vec.x + 12.793
    ), octaves);

    float z = PerlinNoise(float3(
        vec.z - 233.145,
        vec.x - 113.408,
        vec.y - 185.31
    ), octaves);

    return float3(x, y, z);
}

/// 本来は右パラメータのカッコ内はマイナスが正しいが、
/// 表現的にプラスのほうが「ぽい」エフェクトになっているのでフェイク関数として定義
float3 CurlNoiseRotFake(float3 p_x0, float3 p_x1, float3 p_y0, float3 p_y1, float3 p_z0, float3 p_z1)
{
    const float e2 = 2.0 * E;
    const float invE2 = 1.0 / e2;

    float x = (p_y1.z - p_y0.z) - (p_z1.y + p_z0.y);
    float y = (p_z1.x - p_z0.x) - (p_x1.z + p_x0.z);
    float z = (p_x1.y - p_x0.y) - (p_y1.x + p_y0.x);

    return float3(x, y, z) * invE2;
}

/// カールノイズの回転処理（∇演算）
float3 CurlNoiseRot(float3 p_x0, float3 p_x1, float3 p_y0, float3 p_y1, float3 p_z0, float3 p_z1)
{
    const float e2 = 2.0 * E;
    const float invE2 = 1.0 / e2;

    float x = (p_y1.z - p_y0.z) - (p_z1.y - p_z0.y);
    float y = (p_z1.x - p_z0.x) - (p_x1.z - p_x0.z);
    float z = (p_x1.y - p_x0.y) - (p_y1.x - p_y0.x);

    return float3(x, y, z) * invE2;
}

#endif