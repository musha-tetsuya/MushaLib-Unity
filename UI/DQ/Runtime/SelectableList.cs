using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        /// セル数
        /// </summary>
        [SerializeField]
        private Vector2Int m_CellCount = Vector2Int.one;

        /// <summary>
        /// 軸
        /// </summary>
        [SerializeField]
        private GridLayoutGroup.Axis m_Axis;

        /// <summary>
        /// 要素数
        /// </summary>
        public int Count => m_Content.childCount;

        /// <summary>
        /// セル数
        /// </summary>
        public Vector2Int CellCount => m_CellCount;

        /// <summary>
        /// 軸
        /// </summary>
        public GridLayoutGroup.Axis Axis => m_Axis;

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
            return m_Content.GetComponentsInChildren<SelectableElement>(true);
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
