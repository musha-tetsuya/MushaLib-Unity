using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow
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
