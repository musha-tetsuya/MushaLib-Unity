using MushaLib.UI.DQ.SelectableList;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.TextInput
{
    /// <summary>
    /// テキスト入力画面インターフェース
    /// </summary>
    public interface ITextInputView
    {
        /// <summary>
        /// テキスト選択時
        /// </summary>
        IObservable<SelectableTextElement> OnSelected { get; }

        /// <summary>
        /// 初期化
        /// </summary>
        void Initialize();

        /// <summary>
        /// テキストテーブル設定
        /// </summary>
        void SetTextTable(string[] textTable);
    }
}
