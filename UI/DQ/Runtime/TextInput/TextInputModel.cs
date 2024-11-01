using MushaLib.UI.DQ.SelectableList;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.UI.DQ.TextInput
{
    /// <summary>
    /// テキスト入力モデル
    /// </summary>
    public class TextInputModel : SelectableListModel, IDisposable
    {
        /// <summary>
        /// テキスト
        /// </summary>
        private ReactiveProperty<string> m_Text = new();

        /// <summary>
        /// テキスト
        /// </summary>
        public IReadOnlyReactiveProperty<string> Text => m_Text;

        /// <summary>
        /// 最大長
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// テキストテーブル提供
        /// </summary>
        public TextTableProvider TextTableProvider { get; } = new();

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            m_Text.Dispose();
        }

        /// <summary>
        /// テキスト更新
        /// </summary>
        public void UpdateText(string text)
        {
            m_Text.SetValueAndForceNotify(text);
        }
    }
}
