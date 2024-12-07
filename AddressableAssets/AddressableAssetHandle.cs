using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;

namespace MushaLib.AddressableAssets
{
    /// <summary>
    /// アセットのロード及び管理するためのハンドルクラス
    /// </summary>
    public class AddressableAssetHandle<T> : IDisposable
    {
        /// <summary>
        /// キー
        /// </summary>
        private readonly string m_Key;

        /// <summary>
        /// ハンドル
        /// </summary>
        private AsyncOperationHandle<T> m_Handle;

        /// <summary>
        /// ロードが完了したかどうか
        /// </summary>
        public bool IsDone => m_Handle.IsDone;

        /// <summary>
        /// アセットのインスタンス
        /// </summary>
        public T Asset => m_Handle.Result;

        /// <summary>
        /// construct
        /// </summary>
        public AddressableAssetHandle(string key)
        {
            m_Key = key;
        }

        /// <summary>
        /// 解放
        /// </summary>
        public void Dispose()
        {
            Addressables.Release(m_Handle);
        }

        /// <summary>
        /// ハンドルが有効かどうか
        /// </summary>
        public bool IsValid()
        {
            return m_Handle.IsValid();
        }

        /// <summary>
        /// アセットの非同期ロード
        /// </summary>
        public async UniTask LoadAsync(CancellationToken cancellationToken = default)
        {
            if (!m_Handle.IsValid())
            {
                m_Handle = Addressables.LoadAssetAsync<T>(m_Key);
            }

            if (!m_Handle.IsDone)
            {
                await m_Handle;
            }

            cancellationToken.ThrowIfCancellationRequested();

            if (m_Handle.Status == AsyncOperationStatus.Failed)
            {
                throw new Exception($"{GetType().Name}: Status is Failed. m_Key={m_Key}, {m_Handle.OperationException.Message}", m_Handle.OperationException);
            }
        }
    }
}
