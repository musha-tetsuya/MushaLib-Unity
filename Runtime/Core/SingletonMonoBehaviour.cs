using System;
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
    /// 継承先でSingletonAttributeのAutomaticをtrueにすると、Instanceへのアクセス時にインスタンスが存在しなかったら自動でインスタンスを作成する。
    /// 継承先でSingletonAttributeのPersistentをtrueにすると、DontDestroyOnLoadになる。
    /// </remarks>
    [Singleton]
    public abstract class SingletonMonoBehaviour<T> : MonoBehaviour, ISingleton where T : MonoBehaviour, ISingleton
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        private static T instance;

        /// <summary>
        /// インスタンスへのアクセス
        /// </summary>
        public static T Instance => instance ?? (instance = Singleton<T>.instance);

        /// <summary>
        /// アプリケーションの終了中かどうか
        /// </summary>
        private bool m_IsQuitting;

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            if (instance == null)
            {
                Singleton<T>.Awake(instance = this as T);

                var attribute = GetType().GetAttribute<SingletonAttribute>();
                if (attribute.Persistent)
                {
                    DontDestroyOnLoad(gameObject);
                }
            }
            else if (instance != this)
            {
                throw new Exception($"'{GetType()}' instance is already exists.");
            }
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            if (instance == this && !m_IsQuitting)
            {
                Singleton<T>.OnDestroy(instance);
                instance = null;
            }
        }

        /// <summary>
        /// OnApplicationQuit
        /// </summary>
        private void OnApplicationQuit()
        {
            m_IsQuitting = true;
        }
    }
}