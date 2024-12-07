using Cysharp.Threading.Tasks;
using MushaLib.AddressableAssets;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.U2D;

namespace MushaLib.UI
{
    /// <summary>
    /// スプライト管理
    /// </summary>
    public class SpriteManager : IDisposable
    {
        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource = new();

        /// <summary>
        /// スプライトハンドルテーブル
        /// </summary>
        private DictionaryDisposable<string, AddressableAssetHandle<Sprite>> m_SpriteHandleTable = new();

        /// <summary>
        /// アトラスハンドルテーブル
        /// </summary>
        private DictionaryDisposable<string, AddressableAssetHandle<SpriteAtlas>> m_AtlasHandleTable = new();

        /// <summary>
        /// アトラスキャッシュテーブル
        /// </summary>
        private DictionaryDisposable<string, SpriteAtlasCache> m_AtlasCacheTable = new();

        /// <summary>
        /// 解放
        /// </summary>
        public void Dispose()
        {
            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = null;

            m_SpriteHandleTable.Dispose();
            m_AtlasHandleTable.Dispose();
            m_AtlasCacheTable.Dispose();
        }

        /// <summary>
        /// スプライトの非同期取得
        /// </summary>
        public async UniTask<Sprite> GetSpriteAsync(string spriteKey, string atlasKey = null, CancellationToken cancellationToken = default)
        {
            // アトラスが指定されていない
            if (string.IsNullOrEmpty(atlasKey))
            {
                if (!m_SpriteHandleTable.TryGetValue(spriteKey, out var spriteHandle))
                {
                    // スプライトハンドル作成
                    spriteHandle = m_SpriteHandleTable[spriteKey] = new(spriteKey);
                }

                // まだロードが終わってない
                if (!spriteHandle.IsValid() || !spriteHandle.IsDone)
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(m_CancellationTokenSource.Token, cancellationToken);

                    // ロード
                    await spriteHandle.LoadAsync(cts.Token);
                }

                return spriteHandle.Asset;
            }
            else
            {
                if (!m_AtlasHandleTable.TryGetValue(atlasKey, out var atlasHandle))
                {
                    // アトラスハンドル作成
                    atlasHandle = m_AtlasHandleTable[atlasKey] = new(atlasKey);
                }

                // まだロードが終わってない
                if (!atlasHandle.IsValid() || !atlasHandle.IsDone)
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(m_CancellationTokenSource.Token, cancellationToken);

                    // ロード
                    await atlasHandle.LoadAsync(cts.Token);
                }

                if (!m_AtlasCacheTable.TryGetValue(atlasKey, out var atlasCache))
                {
                    // アトラスキャッシュ作成
                    atlasCache = m_AtlasCacheTable[atlasKey] = new SpriteAtlasCache { Atlas = atlasHandle.Asset };
                }

                return atlasCache.GetSprite(spriteKey);
            }
        }
    }
}
