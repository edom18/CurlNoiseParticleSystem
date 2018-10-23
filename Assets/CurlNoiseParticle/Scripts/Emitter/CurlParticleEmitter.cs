using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem.Emitter
{
    /// <summary>
    /// Emit normal curl noise particles.
    /// </summary>
    public class CurlParticleEmitter : MonoBehaviour
    {
        [SerializeField]
        private Color _particleColor = Color.white;

        [SerializeField]
        private int _countPerParticle = 500;

        private CurlParticle _particle;

        private Vector3 ColorVec
        {
            get { return new Vector3(_particleColor.r, _particleColor.g, _particleColor.b); }
        }

        private void Start()
        {
            _particle = CurlParticleSystem.Instance.Get();
            _particle.AutoRelease = false;
        }

        /// <summary>
        /// Emit particles.
        /// </summary>
        public void Emit()
        {
            _particle.Emit(new ParticleParam
            {
                Position = transform.position,
                Delay = 0,
                Color = ColorVec,
            }, _countPerParticle);
        }
    }
}
