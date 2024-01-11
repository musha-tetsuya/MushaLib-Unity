using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.InfiniteScrollView
{
    /// <summary>
    /// スクロール要素基底
    /// </summary>
    [RequireComponent(typeof(RectTransform))]
    public class ScrollElement : MonoBehaviour, IScrollElement
    {
        /// <summary>
        /// RectTransform
        /// </summary>
        private RectTransform m_RectTransform;

        /// <summary>
        /// RectTransform
        /// </summary>
        RectTransform IScrollElement.RectTransform => m_RectTransform ?? (m_RectTransform = transform as RectTransform);

        /// <summary>
        /// Viewport内ローカル座標（左上基準）
        /// </summary>
        Vector2 IScrollElement.LocalPosition { get; set; }

        /// <summary>
        /// 何番目の要素か
        /// </summary>
        int IScrollElement.Index { get; set; }

        /// <summary>
        /// 何列目の要素か
        /// </summary>
        int IScrollElement.Column { get; set; }

        /// <summary>
        /// 何行目の要素か
        /// </summary>
        int IScrollElement.Row { get; set; }

        /// <summary>
        /// ページ内で何番目の要素か
        /// </summary>
        int IScrollElement.LocalIndex { get; set; }

        /// <summary>
        /// ページ内で何列目の要素か
        /// </summary>
        int IScrollElement.LocalColumn { get; set; }

        /// <summary>
        /// ページ内で何行目の要素か
        /// </summary>
        int IScrollElement.LocalRow { get; set; }

        /// <summary>
        /// 何番目のページか
        /// </summary>
        int IScrollElement.PageIndex { get; set; }

        /// <summary>
        /// 何列目のページか
        /// </summary>
        int IScrollElement.PageColumn { get; set; }

        /// <summary>
        /// 何行目のページか
        /// </summary>
        int IScrollElement.PageRow { get; set; }
    }
}
