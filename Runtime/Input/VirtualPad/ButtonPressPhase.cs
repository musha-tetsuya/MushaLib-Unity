using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.Input.VirtualPad
{
    /// <summary>
    /// ボタン押下フェーズ
    /// </summary>
    public enum ButtonPressPhase
    {
        /// <summary>
        /// 押した瞬間
        /// </summary>
        Pressed,

        /// <summary>
        /// 長押し成立時
        /// </summary>
        LongPressed,

        /// <summary>
        /// リピート時
        /// </summary>
        Repeat,
    }
}
