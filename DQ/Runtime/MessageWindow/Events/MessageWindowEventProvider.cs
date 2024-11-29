using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウイベント提供
    /// </summary>
    public abstract class MessageWindowEventProvider : MonoBehaviour
    {
        /// <summary>
        /// メッセージウィンドウイベント取得
        /// </summary>
        public abstract IMessageWindowEvent GetEvent();
    }
}
