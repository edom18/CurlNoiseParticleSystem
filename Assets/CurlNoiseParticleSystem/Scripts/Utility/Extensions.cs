using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.IO;

namespace CurlNoiseParticleSystem
{
    /// <summary>
    /// System.Actionの拡張
    /// </summary>
    public static class ActionExtensions
    {
        public static void SafeInvoke(this System.Action action)
        {
            if (action != null)
            {
                action.Invoke();
            }
        }

        public static void SafeInvoke<T>(this System.Action<T> action, T val)
        {
            if (action != null)
            {
                action.Invoke(val);
            }
        }

        public static void SafeInvoke<T1, T2>(this System.Action<T1, T2> action, T1 val1, T2 val2)
        {
            if (action != null)
            {
                action.Invoke(val1, val2);
            }
        }

        public static void SafeInvoke<T1, T2, T3>(this System.Action<T1, T2, T3> action, T1 val1, T2 val2, T3 val3)
        {
            if (action != null)
            {
                action.Invoke(val1, val2, val3);
            }
        }

        public static void SafeInvoke<T1, T2, T3, T4>(this System.Action<T1, T2, T3, T4> action, T1 val1, T2 val2, T3 val3, T4 val4)
        {
            if (action != null)
            {
                action.Invoke(val1, val2, val3, val4);
            }
        }
    }

    /// <summary>
    /// UnityEventの拡張
    /// </summary>
    public static class UnityEventExtensions
    {
        public static void SafeInvoke(this UnityEvent evt)
        {
            if (evt != null)
            {
                evt.Invoke();
            }
        }

        public static void SafeInvoke<T0>(this UnityEvent<T0> evt, T0 val0)
        {
            if (evt != null)
            {
                evt.Invoke(val0);
            }
        }

        public static void SafeInvoke<T0, T1>(this UnityEvent<T0, T1> evt, T0 val0, T1 val1)
        {
            if (evt != null)
            {
                evt.Invoke(val0, val1);
            }
        }

        public static void SafeInvoke<T0, T1, T2>(this UnityEvent<T0, T1, T2> evt, T0 val0, T1 val1, T2 val2)
        {
            if (evt != null)
            {
                evt.Invoke(val0, val1, val2);
            }
        }

        public static void SafeInvoke<T0, T1, T2, T3>(this UnityEvent<T0, T1, T2, T3> evt, T0 val0, T1 val1, T2 val2, T3 val3)
        {
            if (evt != null)
            {
                evt.Invoke(val0, val1, val2, val3);
            }
        }
    }

    public static class TranformExtension
    {
        public static void SetPosX(this Transform trans, float x)
        {
            Vector3 pos = trans.position;
            pos.x = x;
            trans.position = pos;
        }

        public static void SetPosY(this Transform trans, float y)
        {
            Vector3 pos = trans.position;
            pos.y = y;
            trans.position = pos;
        }

        public static void SetPosZ(this Transform trans, float z)
        {
            Vector3 pos = trans.position;
            pos.x = z;
            trans.position = pos;
        }
    }

    public static class GameObjectExtension
    {
        public static T EnsureComponent<T>(this GameObject target) where T : Component
        {
            T comp = target.GetComponent<T>();
            if (comp == null)
            {
                comp = target.AddComponent<T>();
            }
            return comp;
        }
    }
}
