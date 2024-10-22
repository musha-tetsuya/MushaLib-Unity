using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.DQ
{
    /// <summary>
    /// ウィンドウ
    /// </summary>
    public class Window : MonoBehaviour
    {
        /// <summary>
        /// 閉じるボタン
        /// </summary>
        [SerializeField]
        private Button m_CloseButton;

        /// <summary>
        /// 閉じるボタン
        /// </summary>
        public Button CloseButton => m_CloseButton;
    }
}
