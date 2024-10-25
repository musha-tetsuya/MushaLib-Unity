using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.UI.DQ.MessageWindowEvents
{
    /// <summary>
    /// メッセージウィンドウ文字列イベント
    /// </summary>
    public interface IStringEvent : IMessageWindowEvent
    {
        /// <summary>
        /// 文字列取得
        /// </summary>
        UniTask<string> GetString(CancellationToken cancellationToken);
    }
}