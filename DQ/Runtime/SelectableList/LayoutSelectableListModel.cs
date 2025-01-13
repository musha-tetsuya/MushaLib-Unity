using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// レイアウト付き選択可能リストモデル
    /// </summary>
    public class LayoutSelectableListModel : SelectableListModel
    {
        /// <summary>
        /// 閉じるボタンを使うかどうか
        /// </summary>
        public bool UseCloseButton { get; set; }

        /// <summary>
        /// ヘッダーテキスト
        /// </summary>
        public LocalizedString HeaderLocalizedString { get; set; }

        /// <summary>
        /// 要素テキスト群
        /// </summary>
        public LocalizedString[] ElementLocalizedStrings { get; set; }
    }
}