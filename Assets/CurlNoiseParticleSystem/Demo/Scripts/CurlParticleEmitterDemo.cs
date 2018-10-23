using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurlNoiseParticleSystem.Emitter;

namespace CurlNoiseParticleSystem.Demo
{
    [RequireComponent(typeof(CurlParticleEmitter))]
    public class CurlParticleEmitterDemo : MonoBehaviour
    {
        private CurlParticleEmitter _emitter;

        private void Start()
        {
            _emitter = GetComponent<CurlParticleEmitter>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                _emitter.Emit();
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 30), "Normal Emit"))
            {
                _emitter.Emit();
            }
        }
    }
}
