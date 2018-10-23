using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CurlNoiseParticleSystem
{
    public class ObjectPool<T> : MonoBehaviour where T : CacheBehaviour
    {
        private T[] _pool;
        private Transform _poolRoot;
        private Stack<int> _freeIndexStack = new Stack<int>();
        private GameObject _prefab;

        private readonly Vector3 VECTOR3_ZERO = Vector3.zero;
        private readonly Quaternion QUATERNION_IDENTITY = Quaternion.identity;

        private void Awake()
        {
            _poolRoot = transform;
        }

        public void Initialize(int limit, GameObject prefab)
        {
            _prefab = prefab;

            _pool = new T[limit];

            Generate();
        }

        private void Generate()
        {
            for (int i = 0; i < _pool.Length; i++)
            {
                GameObject go = Instantiate(_prefab); 
                go.name = _prefab.name + ":" + i;
                T comp = go.GetComponent<T>();
                comp.Index = i;
                _pool[i] = comp;
                go.transform.SetParent(_poolRoot);
                go.transform.localPosition = Vector3.zero;
                go.transform.localRotation = Quaternion.identity;
                go.SetActive(false);

                _freeIndexStack.Push(i);
            }
        }

        public T Get()
        {
            if (_freeIndexStack.Count == 0)
            {
                return default(T);
            }

            int freeIndex = _freeIndexStack.Pop();
            T obj = _pool[freeIndex];
            obj.GameObject.SetActive(true);
            obj.Wakeup();

            return obj;
        }

        public void Back(T obj)
        {
            if (!_freeIndexStack.Contains(obj.Index))
            {
                _freeIndexStack.Push(obj.Index);
            }

            obj.Transform.SetParent(_poolRoot);
            obj.Transform.localPosition = VECTOR3_ZERO;
            obj.Transform.localRotation = QUATERNION_IDENTITY;
            obj.GameObject.SetActive(false);
            obj.Sleep();
        }
    }
}