using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.DQ.SelectableList
{
    /// <summary>
    /// 選択可能要素
    /// </summary>
    public class SelectableElement : MonoBehaviour
    {
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

        /// <summary>
        /// ボタン
        /// </summary>
        public Button Button => m_Button;
        
        /// <summary>
        /// 矢印
        /// </summary>
        public Arrow Arrow => m_Arrow;
    }
}
