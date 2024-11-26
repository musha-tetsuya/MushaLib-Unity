using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Localization;

namespace MushaLib.Utilities
{
    /// <summary>
    /// ローカライズ文字列ユーティリティ
    /// </summary>
    public static class LocalizedStringUtility
    {
        /// <summary>
        /// ローカライズ文字列の取得とリソースの自動解放
        /// </summary>
        public static async UniTask<string> GetLocalizedStringAndReleaseAsync(this LocalizedString self, CancellationToken cancellationToken)
        {
            var handle = self.GetLocalizedStringAsync();

            try
            {
                return await handle.WithCancellation(cancellationToken);
            }
            catch
            {
                throw;
            }
            finally
            {
                Addressables.Release(handle);
            }
        }
    }
}
