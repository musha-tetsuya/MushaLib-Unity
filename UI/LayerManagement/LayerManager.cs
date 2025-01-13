using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MushaLib.UI.LayerManagement
{
    /// <summary>
    /// レイヤー管理
    /// </summary>
    public class LayerManager<T>
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        private static LayerManager<T> m_Instance;

        /// <summary>
        /// インスタンス
        /// </summary>
        private static LayerManager<T> Instance => m_Instance ??= new();

        /// <summary>
        /// コンテナテーブル
        /// </summary>
        private Dictionary<string, LayerContainer<T>> m_ContainerTable = new();

        /// <summary>
        /// コンテナ追加
        /// </summary>
        public static void AddContainer(LayerContainer<T> container)
        {
            Instance.m_ContainerTable.Add(container.gameObject.scene.name, container);
        }

        /// <summary>
        /// コンテナ除去
        /// </summary>
        public static void RemoveContainer(LayerContainer<T> container)
        {
            Instance.m_ContainerTable.Remove(container.gameObject.scene.name);
        }

        /// <summary>
        /// レイヤー取得
        /// </summary>
        public static Layer<T> GetLayer(string sceneName, T layerType)
        {
            if (Instance.m_ContainerTable.TryGetValue(sceneName, out var container))
            {
                return container.GetLayer(layerType);
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// レイヤー取得
        /// </summary>
        public static Layer<T> GetCurrentLayer(T layerType)
        {
            return GetLayer(SceneManager.GetActiveScene().name, layerType);
        }
    }
}
