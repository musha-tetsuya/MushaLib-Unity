using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

namespace MushaLib.Core
{
    /// <summary>
    /// シングルトンのMonoBehaviourを実装する抽象クラス
    /// </summary>
    /// <remarks>
    /// 継承先クラスではUnity.VisualScripting.SingletonAttributeを付与すること。
    /// AttributeのAutomaticをtrueにすると、Instanceへのアクセス時にインスタンスが存在しなかったら自動でインスタンスを作成する。
    /// AttributeのPersistentをtrueにすると、DontDestroyOnLoadになる。
    /// </remarks>
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
    {
        /// <summary>
        /// インスタンスへのアクセス
        /// </summary>
        public static T Instance => Singleton<T>.instance;

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            Singleton<T>.Awake(this as T);

            var attribute = GetType().GetAttribute<SingletonAttribute>();
            if (attribute.Persistent)
            {
                DontDestroyOnLoad(gameObject);
            }
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            Singleton<T>.OnDestroy(this as T);
        }
    }
}