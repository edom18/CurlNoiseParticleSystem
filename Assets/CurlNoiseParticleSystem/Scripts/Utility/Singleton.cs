using UnityEngine;
using System.Collections;
using System.Collections.Generic;

namespace CurlNoiseParticleSystem
{
    /// <summary>
    /// シングルトンのベースクラス
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class Singleton<T> : MonoBehaviour where T : MonoBehaviour
    {
        private static T _instance;
        public static T Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                System.Type type = typeof(T);
                T instance = FindObjectOfType(type) as T;
                if (instance != null)
                {
                    _instance = instance;
                    return _instance;
                }

                var name = type.ToString();
                var obj = new GameObject(name, type);
                instance = obj.GetComponent<T>();
                if (instance != null)
                {
                    _instance = instance;
                    return instance;
                }

                return null;
            }
        }

        protected virtual void Awake()
        {
            if (_instance == null)
            {
                _instance = this as T;
            }

            if (_instance != this)
            {
                Debug.LogWarningFormat("This instance will be deleted. Because this class is singleton behaviour. {0}", this);
                Destroy(this);
                return;
            }

            transform.parent = null;

            DontDestroyOnLoad(this);

            OnAwake();
        }

        protected virtual void OnAwake() { }
    }
}
