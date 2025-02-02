using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// 選択可能テキスト要素
    /// </summary>
    public class SelectableTextElement : SelectableElement
    {
        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI m_TextMesh;

        /// <summary>
        /// テキスト
        /// </summary>
        public TextMeshProUGUI TextMesh => m_TextMesh;
    }
}
