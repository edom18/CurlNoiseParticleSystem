using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem.Emitter
{
    /// <summary>
    /// Emit particles with distance.
    /// </summary>
    public class DistanceEmitter : MonoBehaviour
    {
        [SerializeField]
        #if UNITY_2017
        [ColorUsage(true)]
        #elif UNITY_2018
        [ColorUsage(true, true)]
        #endif
        private Color _particleColor = Color.white;

        [SerializeField]
        private int _particleCount = 1000;

        [SerializeField]
        private float _distanceThreshold = 0.1f;
        private float _sqrDistanceThreshold = 0;

        private bool _isPlaying = false;
        public bool IsPlaying
        {
            get { return _isPlaying; }
        }

        private CurlParticle _particle;

        private Vector3 _prevPos;

        private Vector3 ColorVec
        {
            get { return new Vector3(_particleColor.r, _particleColor.g, _particleColor.b); }
        }

        #region ### MonoBehaviour ###
        private void Start()
        {
            _particle = CurlParticleSystem.Instance.Get();
            _particle.AutoRelease = false;

            _sqrDistanceThreshold = _distanceThreshold * _distanceThreshold;
        }

        private void OnValidate()
        {
            _sqrDistanceThreshold = _distanceThreshold * _distanceThreshold;
        }

        private void Update()
        {
            if (_isPlaying)
            {
                EmitCheck();
            }
        }

        #endregion ### MonoBehaviour ###

        /// <summary>
        /// Play particle system.
        /// </summary>
        public void Play()
        {
            _isPlaying = true;
        }

        /// <summary>
        /// Stop particle system.
        /// </summary>
        public void Stop()
        {
            _isPlaying = false;
        }

        /// <summary>
        /// Check emit particles.
        /// </summary>
        private void EmitCheck()
        {
            if (_prevPos == default(Vector3))
            {
                _prevPos = transform.position;
                return;
            }

            Vector3 delta = transform.position - _prevPos;
            if (delta.sqrMagnitude < _sqrDistanceThreshold)
            {
                return;
            }

            float len = delta.magnitude;
            Vector3 dir = delta.normalized;

            for (float t = 0; t <= _distanceThreshold; t += 0.05f)
            {
                _particle.Emit(new ParticleParam
                {
                    Position = _prevPos + (dir * t),
                    Delay = 0,
                    Color = ColorVec, 
                }, _particleCount);
            }

            _prevPos = transform.position;
        }
    }
}
