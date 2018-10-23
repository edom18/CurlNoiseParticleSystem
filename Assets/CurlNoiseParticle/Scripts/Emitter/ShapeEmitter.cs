using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticle
{
    public class ShapeEmitter : MonoBehaviour
    {
        [SerializeField]
        private MeshFilter _filter;

        [SerializeField]
        private Color _particleColor = Color.white;

        [SerializeField]
        private int _countPerParticle = 1;

        [SerializeField]
        private float _delay = 0.5f;

        private CurlParticle _particle;

        private Vector3 ColorVec
        {
            get
            {
                return new Vector3(_particleColor.r, _particleColor.g, _particleColor.b);
            }
        }

        private void Start()
        {
            _particle = CurlParticleSystem.Instance.Get();
            _particle.AutoRelease = false;
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.T))
            {
                MeshEmit();
            }

            if (Input.GetKeyDown(KeyCode.E))
            {
                NormalEmit();
            }

            if (Input.GetKeyDown(KeyCode.S))
            {
                _particle.Stop();
            }
        }

        /// <summary>
        /// Normal emit particles.
        /// </summary>
        private void NormalEmit()
        {
            _particle.Emit(new ParticleParam
            {
                Position = transform.position,
                Delay = 0,
                Color = ColorVec,
            }, 500);
        }

        /// <summary>
        /// Burst with particle param list.
        /// </summary>
        private void MeshEmit()
        {
            CurlParticle particle = CurlParticleSystem.Instance.Get();
            particle.EmitWithMesh(_filter, _countPerParticle, _delay);
        }
    }
}
