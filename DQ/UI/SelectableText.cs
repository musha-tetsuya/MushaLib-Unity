using MushaLib.Localization;
using MushaLib.UI.InfiniteScrollView;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization.Components;
using UnityEngine.UI;

namespace MushaLib.DQ.UI
{
    /// <summary>
    /// 選択可能テキスト
    /// </summary>
    public class SelectableText : ScrollElement
    {
        /// <summary>
        /// 文字列構築機能
        /// </summary>
        [SerializeField]
        private LocalizeStringEvent m_LocalizeStringEvent;

        /// <summary>
        /// 文字列構築機能
        /// </summary>
        [SerializeField]
        private LocalizeStringBuilder m_LocalizeStringBuilder;

        /// <summary>
        /// ボタン
        /// </summary>
        [SerializeField]
        private Button m_Button;

        /// <summary>
        /// 矢印
        /// </summary>
        [SerializeField]
        private Arrow m_Arrow;

        public LocalizeStringEvent LocalizeStringEvent => m_LocalizeStringEvent;

        public LocalizeStringBuilder LocalizeStringBuilder => m_LocalizeStringBuilder;
        
        public Button Button => m_Button;
        
        public Arrow Arrow => m_Arrow;
    }
}
