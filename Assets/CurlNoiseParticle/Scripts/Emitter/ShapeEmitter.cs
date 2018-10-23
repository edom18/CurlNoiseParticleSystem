using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem
{
    public class ShapeEmitter : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _filter;

        [SerializeField]
        private int _countPerParticle = 1;

        [SerializeField]
        private float _delay = 0.5f;

        private CurlParticle _particle;

        private void Start()
        {
            _particle = CurlParticleSystem.Instance.Get();
            _particle.AutoRelease = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                Emit();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _particle.Stop();
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 50, 150, 30), "Shape Emit"))
            {
                Emit();
            }
        }

        /// <summary>
        /// Burst with particle param list.
        /// </summary>
        private void Emit()
        {
            CurlParticle particle = CurlParticleSystem.Instance.Get();
            particle.EmitWithMesh(_filter, _countPerParticle, _delay);
        }
    }
}
