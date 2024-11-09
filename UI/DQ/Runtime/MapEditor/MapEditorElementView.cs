using MushaLib.UI.InfiniteScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.DQ.MapEditor
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
        /// イメージ
        /// </summary>
        public Image Image => m_Image;

        /// <summary>
        /// ボタン
        /// </summary>
        public Button Button => m_Button;
    }
}