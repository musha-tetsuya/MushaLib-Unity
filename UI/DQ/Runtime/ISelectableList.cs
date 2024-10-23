using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ
{
    /// <summary>
    /// 選択可能要素リストインターフェース
    /// </summary>
    public interface ISelectableList
    {
        /// <summary>
        /// 要素数
        /// </summary>
        public int Count { get; }

        /// <summary>
        /// 要素取得
        /// </summary>
        public SelectableElement GetElement(int index);

        /// <summary>
        /// 全要素取得
        /// </summary>
        public IEnumerable<SelectableElement> GetElements();

        /// <summary>
        /// 全要素操作の一括変更
        /// </summary>
        public void SetInteractable(bool interactable);
    }
}
