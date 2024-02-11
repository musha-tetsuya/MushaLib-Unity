using MushaLib.UI.InfiniteScrollView;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.UI
{
    /// <summary>
    /// リストウィンドウ
    /// </summary>
    public abstract class ListWindow : Window
    {
        /// <summary>
        /// スクロールビュー
        /// </summary>
        [SerializeField]
        private InfiniteScrollView m_ScrollView;

        /// <summary>
        /// スクロールビュー
        /// </summary>
        public InfiniteScrollView ScrollView => m_ScrollView;
    }
}
