using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.UI.DQ
{
    /// <summary>
    /// 選択可能リスト
    /// </summary>
    public class SelectableList : MonoBehaviour, ISelectableList
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
        /// 要素数
        /// </summary>
        public int Count => m_Content.childCount;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            m_OnClickElement.Dispose();
        }

        /// <summary>
        /// 要素取得
        /// </summary>
        public SelectableElement GetElement(int index)
        {
            if (0 <= index && index < Count)
            {
                return m_Content.GetChild(index).GetComponent<SelectableElement>();
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 全要素取得
        /// </summary>
        public IEnumerable<SelectableElement> GetElements()
        {
            return m_Content.GetComponentsInChildren<SelectableElement>();
        }

        /// <summary>
        /// 全要素操作の一括変更
        /// </summary>
        public void SetInteractable(bool interactable)
        {
            m_CanvasGroup.interactable = interactable;
        }
    }
}
