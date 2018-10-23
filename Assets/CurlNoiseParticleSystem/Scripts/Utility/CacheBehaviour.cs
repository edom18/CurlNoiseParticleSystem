using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem
{
    public class CacheBehaviour : MonoBehaviour
    {
        public GameObject GameObject
        {
            get { return gameObject; }
        }
        public Transform Transform
        {
            get { return transform; }
        }
        public int Index { get; set; }
        public bool IsSleep { get; set; }
        public void Wakeup()
        {
            IsSleep = true;
            OnStart();
        }
        public void Sleep()
        {
            IsSleep = false;
            OnRelease();
        }
        public virtual void OnStart() { }
        public virtual void OnRelease() { }
    }
}