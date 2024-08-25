using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.UI.DQ.MessageWindowEvents
{
    /// <summary>
    /// メッセージウィンドウ文字列イベント基底
    /// </summary>
    public abstract class StringEventBase : IStringEvent
    {
        /// <summary>
        /// 文字列取得
        /// </summary>
        public abstract UniTask<string> GetString(CancellationToken cancellationToken);

        /// <summary>
        /// 実行
        /// </summary>
        public virtual async UniTask Run(MessageWindow messageWindow, CancellationToken cancellationToken)
        {
            var message = await GetString(cancellationToken);

            await messageWindow.ShowMessage(message, cancellationToken);
        }
    }
}