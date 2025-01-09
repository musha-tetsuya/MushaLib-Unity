using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;

namespace MushaLib.Localization
{
    /// <summary>
    /// ローカライズテーブルを基にIVariableを提供するクラス
    /// </summary>
    public class LocalizedStringVariableProvider : IVariableProvider
    {
        /// <summary>
        /// テーブル
        /// </summary>
        private readonly TableReference m_TableReference;

        /// <summary>
        /// エントリ
        /// </summary>
        private readonly TableEntryReference m_TableEntryReference;

        /// <summary>
        /// construct
        /// </summary>
        public LocalizedStringVariableProvider(TableReference tableReference, TableEntryReference tableEntryReference)
        {
            m_TableReference = tableReference;
            m_TableEntryReference = tableEntryReference;
        }

        /// <summary>
        /// IVariableの非同期取得
        /// </summary>
        public async UniTask<IVariable> GetVariableAsync(CancellationToken cancellationToken)
        {
            var utcs = new UniTaskCompletionSource<string>();

            using (cancellationToken.RegisterWithoutCaptureExecutionContext(() => utcs.TrySetCanceled()))
            {
                var handle = LocalizationSettings.StringDatabase.GetLocalizedStringAsync(m_TableReference, m_TableEntryReference);

                handle.Completed += x => utcs.TrySetResult(x.Result);

                return new StringVariable { Value = await utcs.Task };
            }
        }
    }
}
