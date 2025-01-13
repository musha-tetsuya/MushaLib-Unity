using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Components;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// ローカライズ選択可能テキスト要素
    /// </summary>
    public class LocalizeSelectableTextElement : SelectableTextElement
    {
        /// <summary>
        /// ローカライズテキスト
        /// </summary>
        [SerializeField]
        private LocalizeStringEvent m_LocalizeStringEvent;

        /// <summary>
        /// ローカライズテキスト
        /// </summary>
        public LocalizeStringEvent LocalizeStringEvent => m_LocalizeStringEvent;
    }
}