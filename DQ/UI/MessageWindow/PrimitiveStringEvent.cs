using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow
{
    /// <summary>
    /// メッセージウィンドウプリミティブ文字列イベント
    /// </summary>
    public class PrimitiveStringEvent : StringEventBase
    {
        /// <summary>
        /// テキスト
        /// </summary>
        private string m_Text;

        /// <summary>
        /// construct
        /// </summary>
        public PrimitiveStringEvent(string text)
        {
            m_Text = text;
        }

        /// <summary>
        /// 文字列取得
        /// </summary>
        public override UniTask<string> GetString(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(m_Text);
        }
    }
}