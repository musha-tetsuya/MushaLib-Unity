using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MushaLib.UI.LayerManagement
{
    /// <summary>
    /// レイヤーコンテナ
    /// </summary>
    public class LayerContainer<T> : MonoBehaviour
    {
        /// <summary>
        /// レイヤープレハブ
        /// </summary>
        [SerializeField]
        private Layer<T> m_LayerPrefab;

        /// <summary>
        /// レイヤーテーブル
        /// </summary>
        private Dictionary<T, Layer<T>> m_LayerTable;

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected virtual void OnDestroy()
        {
            LayerManager<T>.RemoveContainer(this);
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            LayerManager<T>.AddContainer(this);
        }

        /// <summary>
        /// レイヤー取得
        /// </summary>
        public Layer<T> GetLayer(T layerType)
        {
            if (m_LayerTable == null)
            {
                m_LayerTable = GetComponentsInChildren<Layer<T>>(true).ToDictionary(x => x.LayerType);
            }

            if (!m_LayerTable.TryGetValue(layerType, out var layer))
            {
                // レイヤー追加
                layer = m_LayerTable[layerType] = Instantiate(m_LayerPrefab, transform);
                layer.LayerType = layerType;
#if UNITY_EDITOR
                layer.name = $"Layer ({layerType})";
#endif

                // ソート
                foreach (var x in m_LayerTable.Values.OrderBy(x => x.SortOrder).Select((x, i) => (x.transform, i)))
                {
                    x.transform.SetSiblingIndex(x.i);
                }
            }

            return layer;
        }
    }
}
