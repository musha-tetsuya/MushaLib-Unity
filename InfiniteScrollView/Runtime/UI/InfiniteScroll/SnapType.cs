using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.InfiniteScroll
{
    /// <summary>
    /// スナップタイプ
    /// </summary>
    public enum SnapType
    {
        /// <summary>
        /// なし
        /// </summary>
        None,

        /// <summary>
        /// ページ単位
        /// </summary>
        Page,

        /// <summary>
        /// 要素単位
        /// </summary>
        Element,
    }
}
