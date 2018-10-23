using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem.Emitter
{
    /// <summary>
    /// Emit particle from shape surface.
    /// </summary>
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

        /// <summary>
        /// Burst with particle param list.
        /// </summary>
        public void Emit()
        {
            CurlParticle particle = CurlParticleSystem.Instance.Get();
            particle.EmitWithMesh(_filter, _countPerParticle, _delay);
        }
    }
}
