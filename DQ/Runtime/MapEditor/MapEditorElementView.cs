using MushaLib.UI.InfiniteScrollView;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.MapEditor
{
    /// <summary>
    /// マップエディタ要素
    /// </summary>
    internal class MapEditorElementView : ScrollElement
    {
        /// <summary>
        /// イメージ
        /// </summary>
        [SerializeField]
        private Image m_Image;

        /// <summary>
        /// ボタン
        /// </summary>
        [SerializeField]
        private Button m_Button;

        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI m_TextMesh;

        /// <summary>
        /// イメージ
        /// </summary>
        public Image Image => m_Image;

        /// <summary>
        /// ボタン
        /// </summary>
        public Button Button => m_Button;

        /// <summary>
        /// テキスト
        /// </summary>
        public TextMeshProUGUI TextMesh => m_TextMesh;
    }
}
