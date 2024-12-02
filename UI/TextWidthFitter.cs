using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace MushaLib.UI
{
    /// <summary>
    /// テキストに合わせてRectTransform幅を調整するコンポーネント
    /// </summary>
    public class TextWidthFitter : MonoBehaviour
    {
        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI m_TextMesh;

        /// <summary>
        /// RectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// テキストセット
        /// </summary>
        public void SetText(string text)
        {
            m_TextMesh.text = text;

            var size = m_RectTransform.sizeDelta;
            size.x = m_TextMesh.GetPreferredValues().x;
            m_RectTransform.sizeDelta = size;
        }
    }
}
