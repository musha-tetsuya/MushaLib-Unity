using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow
{
    /// <summary>
    /// メッセージウィンドウ文字列イベント
    /// </summary>
    public interface IStringEvent : IEvent
    {
        /// <summary>
        /// 文字列取得
        /// </summary>
        UniTask<string> GetString(CancellationToken cancellationToken);
    }
}