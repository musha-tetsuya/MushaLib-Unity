using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.InfiniteScroll
{
    /// <summary>
    /// スクロール要素インターフェース
    /// </summary>
    public interface IScrollElement
    {
        /// <summary>
        /// RectTransform
        /// </summary>
        RectTransform RectTransform { get; }

        /// <summary>
        /// Viewport内ローカル座標（左上基準）
        /// </summary>
        Vector2 LocalPosition { get; set; }

        /// <summary>
        /// 何番目の要素か
        /// </summary>
        int Index { get; set; }

        /// <summary>
        /// 何列目の要素か
        /// </summary>
        int Column { get; set; }

        /// <summary>
        /// 何行目の要素か
        /// </summary>
        int Row { get; set; }

        /// <summary>
        /// ページ内で何番目の要素か
        /// </summary>
        int LocalIndex { get; set; }
        
        /// <summary>
        /// ページ内で何列目の要素か
        /// </summary>
        int LocalColumn { get; set; }

        /// <summary>
        /// ページ内で何行目の要素か
        /// </summary>
        int LocalRow { get; set; }

        /// <summary>
        /// 何番目のページか
        /// </summary>
        int PageIndex { get; set; }

        /// <summary>
        /// 何列目のページか
        /// </summary>
        int PageColumn { get; set; }

        /// <summary>
        /// 何行目のページか
        /// </summary>
        int PageRow { get; set; }
    }
}