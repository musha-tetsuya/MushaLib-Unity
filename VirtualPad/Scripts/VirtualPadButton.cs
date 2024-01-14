using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.OnScreen;

namespace MushaLib.VirtualPad
{
    /// <summary>
    /// 仮想パッドボタン
    /// </summary>
    public class VirtualPadButton : OnScreenButton, IPointerExitHandler
    {
        /// <summary>
        /// ボタンタイプ
        /// </summary>
        [SerializeField]
        private ButtonType m_ButtonType;

        /// <summary>
        /// ボタンタイプへのアクセス
        /// </summary>
        public ButtonType buttonType => this.m_ButtonType;

        /// <summary>
        /// OnPointerExit
        /// </summary>
        void IPointerExitHandler.OnPointerExit(PointerEventData eventData)
        {
            SendValueToControl(0.0f);
        }
    }
}