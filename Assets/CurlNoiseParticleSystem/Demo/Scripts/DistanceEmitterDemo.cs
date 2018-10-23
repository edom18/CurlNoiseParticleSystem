using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurlNoiseParticleSystem.Emitter;

namespace CurlNoiseParticleSystem.Demo
{
    [RequireComponent(typeof(DistanceEmitter))]
    public class DistanceEmitterDemo : MonoBehaviour
    {
        private DistanceEmitter _emitter;

        private void Start()
        {
            _emitter = GetComponent<DistanceEmitter>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _emitter.Play();
            }
        }

        private void OnGUI()
        {
            if (_emitter.IsPlaying)
            {
                if (GUI.Button(new Rect(10, 90, 150, 30), "Stop Distance Emit"))
                {
                    _emitter.Stop();
                }
            }
            else
            {
                if (GUI.Button(new Rect(10, 90, 150, 30), "Start Distance Emit"))
                {
                    _emitter.Play();
                }
            }
        }
    }
}
