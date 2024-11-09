using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// GUIステートインターフェース
    /// </summary>
    public interface IGUIState
    {
        /// <summary>
        /// OnGUI
        /// </summary>
        void OnGUI();
    }
}
