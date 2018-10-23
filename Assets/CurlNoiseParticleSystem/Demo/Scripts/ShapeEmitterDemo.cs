using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CurlNoiseParticleSystem.Emitter;

namespace CurlNoiseParticleSystem.Demo
{
    [RequireComponent(typeof(ShapeEmitter))]
    public class ShapeEmitterDemo : MonoBehaviour
    {
        private ShapeEmitter _emitter;

        private void Start()
        {
            _emitter = GetComponent<ShapeEmitter>();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                _emitter.Emit();
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 50, 150, 30), "Shape Emit"))
            {
                _emitter.Emit();
            }
        }
    }
}
