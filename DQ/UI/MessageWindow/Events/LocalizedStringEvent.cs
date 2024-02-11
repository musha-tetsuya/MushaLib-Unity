using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;

namespace MushaLib.DQ.UI.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウローカライズ文字列イベント
    /// </summary>
    public class LocalizedStringEvent : StringEventBase
    {
        /// <summary>
        /// ローカライズ文字列
        /// </summary>
        private LocalizedString m_LocalizedString;

        /// <summary>
        /// construct
        /// </summary>
        public LocalizedStringEvent(LocalizedString localizedString)
        {
            m_LocalizedString = localizedString;
        }

        /// <summary>
        /// 文字列取得
        /// </summary>
        public override async UniTask<string> GetString(CancellationToken cancellationToken)
        {
            return await m_LocalizedString.GetLocalizedStringAsync().ToUniTask(cancellationToken: cancellationToken);
        }
    }
}