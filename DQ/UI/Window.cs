using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.UI
{
    /// <summary>
    /// ウィンドウ
    /// </summary>
    public abstract class Window : MonoBehaviour
    {
        /// <summary>
        /// CanvasGroup
        /// </summary>
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        [SerializeField]
        private Button m_CloseButton;

        /// <summary>
        /// CanvasGroup
        /// </summary>
        public CanvasGroup CanvasGroup => m_CanvasGroup;

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        public Button CloseButton => m_CloseButton;
    }
}
