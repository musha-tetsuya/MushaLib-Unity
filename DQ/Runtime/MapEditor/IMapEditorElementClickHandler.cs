using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.MapEditor
{
    /// <summary>
    /// 要素クリックインターフェース
    /// </summary>
    internal interface IMapEditorElementClickHandler
    {
        /// <summary>
        /// 要素クリック時
        /// </summary>
        void OnClickElement(MapEditorElementView view, int index);
    }
}
