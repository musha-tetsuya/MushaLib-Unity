using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// 要素クリックインターフェース
    /// </summary>
    internal interface IElementClickHandler
    {
        /// <summary>
        /// 要素クリック時
        /// </summary>
        void OnClickElement(MapEditorElementView view, int index);
    }
}
