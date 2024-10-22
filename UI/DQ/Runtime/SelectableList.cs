using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.UI.DQ
{
    /// <summary>
    /// 選択可能リスト
    /// </summary>
    public class SelectableList : MonoBehaviour
    {
        /// <summary>
        /// CanvasGroup
        /// </summary>
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// コンテンツ
        /// </summary>
        [SerializeField]
        private Transform m_Content;

        /// <summary>
        /// 要素クリック時
        /// </summary>
        private Subject<int> m_OnClickElement = new Subject<int>();

        /// <summary>
        /// CanvasGroup
        /// </summary>
        public CanvasGroup CanvasGroup => m_CanvasGroup;

        /// <summary>
        /// コンテンツ
        /// </summary>
        public Transform Content => m_Content;

        /// <summary>
        /// 要素クリック時
        /// </summary>
        public IObservable<int> OnClickElementObservable => m_OnClickElement;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            m_OnClickElement.Dispose();
        }

        /// <summary>
        /// 要素クリック時
        /// </summary>
        public void OnClickElement(SelectableElement element)
        {
            m_OnClickElement.OnNext(element.transform.GetSiblingIndex());
        }

        /// <summary>
        /// 要素取得
        /// </summary>
        public SelectableElement GetElement(int index)
        {
            if (0 <= index && index < m_Content.childCount)
            {
                return m_Content.GetChild(index).GetComponent<SelectableElement>();
            }
            else
            {
                return null;
            }
        }
    }
}
