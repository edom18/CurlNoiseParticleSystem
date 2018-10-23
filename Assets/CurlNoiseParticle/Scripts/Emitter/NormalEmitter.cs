using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticle
{
    public class NormalEmitter : MonoBehaviour
    {
        [SerializeField]
        private Color _particleColor = Color.white;

        [SerializeField]
        private int _countPerParticle = 1;

        [SerializeField]
        private float _delay = 0.5f;

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

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.E))
            {
                Emit();
            }
        }

        private void OnGUI()
        {
            if (GUI.Button(new Rect(10, 10, 150, 30), "Normal Emit"))
            {
                Emit();
            }
        }

        /// <summary>
        /// Normal emit particles.
        /// </summary>
        private void Emit()
        {
            _particle.Emit(new ParticleParam
            {
                Position = transform.position,
                Delay = 0,
                Color = ColorVec,
            }, 500);
        }
    }
}
