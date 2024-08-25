using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.UI.DQ.MessageWindowEvents
{
    /// <summary>
    /// メッセージウィンドウイベントインターフェース
    /// </summary>
    public interface IMessageWindowEvent
    {
        /// <summary>
        /// 実行
        /// </summary>
        UniTask Run(MessageWindow messageWindow, CancellationToken cancellationToken);
    }
}
