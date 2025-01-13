using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.LayerManagement
{
    /// <summary>
    /// レイヤー
    /// </summary>
    public abstract class Layer<T> : MonoBehaviour
    {
        /// <summary>
        /// レイヤータイプ
        /// </summary>
        [field: SerializeField]
        public T LayerType { get; set; }

        /// <summary>
        /// ソート順
        /// </summary>
        public abstract int SortOrder { get; }
    }
}
