using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.DQ.MessageWindow.Events
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
        public virtual async UniTask Run(MessageWindowView view, CancellationToken cancellationToken)
        {
            var message = await GetString(cancellationToken);

            await view.ShowMessage(message, cancellationToken);
        }
    }
}