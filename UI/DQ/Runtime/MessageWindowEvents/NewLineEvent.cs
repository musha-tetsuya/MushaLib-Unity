using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MessageWindowEvents
{
    /// <summary>
    /// メッセージウィンドウ改行イベント
    /// </summary>
    public class NewLineEvent : PrimitiveStringEvent
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static NewLineEvent Instance { get; } = new();

        /// <summary>
        /// construct
        /// </summary>
        private NewLineEvent()
            : base("\n")
        {
        }
    }
}
