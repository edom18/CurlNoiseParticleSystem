#ifndef __DEFINE_CGINC__
#define __DEFINE_CGINC__

struct Particle
{
    int id;
    int active;
    float3 position;
    float3 velocity;
    float3 color;
    float scale;
    float baseScale;
    float time;
    float lifeTime;
    float delay;
};

#define E 0.0009765625

#endif