using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// レイアウト付き選択可能リスト
    /// </summary>
    public class LayoutSelectableListView : SelectableListView
    {
        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        [SerializeField]
        private Button m_CloseButton;

        /// <summary>
        /// ヘッダーテキスト
        /// </summary>
        [SerializeField]
        private LocalizeStringEvent m_HeaderLocalizeStringEvent;

        /// <summary>
        /// 要素プレハブ
        /// </summary>
        [SerializeField]
        private LocalizeSelectableTextElement m_ElementPrefab;

        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        public RectTransform RectTransform => m_RectTransform;

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        public Button CloseButton => m_CloseButton;

        /// <summary>
        /// ヘッダーテキスト
        /// </summary>
        public LocalizeStringEvent HeaderLocalizeStringEvent => m_HeaderLocalizeStringEvent;

        /// <summary>
        /// 要素プレハブ
        /// </summary>
        public LocalizeSelectableTextElement ElementPrefab => m_ElementPrefab;
    }
}