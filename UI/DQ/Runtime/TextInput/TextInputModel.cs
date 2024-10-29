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
    public class TextInputModel : IDisposable
    {
        /// <summary>
        /// テキスト
        /// </summary>
        private ReactiveProperty<string> m_Text = new();
        public IReadOnlyReactiveProperty<string> Text => m_Text;

        /// <summary>
        /// 決定時
        /// </summary>
        private Subject<Unit> m_OnSubmit = new();
        public IObservable<Unit> OnSubmit => m_OnSubmit;

        /// <summary>
        /// テキストテーブル提供
        /// </summary>
        public TextTableProvider TextTableProvider { get; } = new();

        /// <summary>
        /// 最大長
        /// </summary>
        public int MaxLength { get; set; }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            m_Text.Dispose();
            m_OnSubmit.Dispose();
        }

        /// <summary>
        /// テキスト更新
        /// </summary>
        public void UpdateText(string text)
        {
            m_Text.SetValueAndForceNotify(text);
        }

        /// <summary>
        /// 決定通知
        /// </summary>
        public void InvokeOnSubmit()
        {
            m_OnSubmit.OnNext(Unit.Default);
        }
    }
}
