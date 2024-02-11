using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウイベントインターフェース
    /// </summary>
    public interface IEvent
    {
        /// <summary>
        /// 実行
        /// </summary>
        UniTask Run(MessageWindow messageWindow, CancellationToken cancellationToken);
    }
}
