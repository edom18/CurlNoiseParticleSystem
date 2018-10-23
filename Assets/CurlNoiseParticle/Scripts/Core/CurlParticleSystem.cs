using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem
{
    public class CurlParticleSystem : Singleton<CurlParticleSystem>
    {
        [SerializeField]
        private int _limit = 5;

        [SerializeField]
        private GameObject _curlParticlePrefab;

        private CurlParticlePool _pool;

        protected override void OnAwake()
        {
            base.OnAwake();

            _pool = gameObject.EnsureComponent<CurlParticlePool>();
            _pool.Initialize(_limit, _curlParticlePrefab);
        }

        public CurlParticle Get()
        {
            CurlParticle particle = _pool.Get();
            particle.OnStop += OnStopHandler;
            return particle;
        }

        public void Release(CurlParticle particle)
        {
            _pool.Back(particle);
        }

        private void OnStopHandler(CurlParticle particle)
        {
            particle.OnStop -= OnStopHandler;
            Release(particle);
        }
    }
}
