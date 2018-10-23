using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem
{
    [CreateAssetMenu]
    public class CurlParticleProfile : ScriptableObject
    {
        public CurlNoiseType CurlNoiseType = CurlNoiseType.Normal;

        [Range(0f, 1f)]
        public float CurlNoiseIntencity = 1f;

        #if UNITY_2017
        [ColorUsage(true)]
        #elif UNITY_2018
        [ColorUsage(true, true)]
        #endif
        public Color ParticleColor = Color.white;

        public float SpeedFactor = 1.0f;

        public float[] NoiseScales = new[] { 0.4f, 0.23f, 0.11f, };
        public float[] NoiseGain = new[] { 1.0f, 0.5f, 0.25f, };

        public float PlumeBase = -3f;
        public float PlumeHeight = 8f;
        public float PlumeCeiling = 3.0f;

        public float RingRadius = 1.25f;
        public float RingMagnitude = 10.0f;
        public float RingFalloff = 0.7f;
        public float RingSpeed = 0.3f;
        public float RingPerSecond = 0.125f;
        public Vector3 RisingForce = new Vector3(0, 0, -0.3f);

        public int Seed = 100;

        public int Octaves = 5;
        public float Frequency = 5.0f;
    }
}
