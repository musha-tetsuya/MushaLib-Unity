using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor
{
    /// <summary>
    /// マップエディタ
    /// </summary>
    internal class MapEditor : MonoBehaviour
    {
        /// <summary>
        /// 新規マップのサイズ
        /// </summary>
        [SerializeField]
        private Vector2Int m_NewMapSize = new(16, 16);

        /// <summary>
        /// 新規マップのページ内セルサイズ
        /// </summary>
        [SerializeField]
        private Vector2 m_NewMapPageCellSize = new(16, 16);

        /// <summary>
        /// 新規マップのページ内セル数
        /// </summary>
        [SerializeField]
        private Vector2Int m_NewMapPageCellCount = new(16, 16);

        /// <summary>
        /// 無限スクロールビュー
        /// </summary>
        [SerializeField]
        private InfiniteScrollView.InfiniteScrollView m_ScrollView;

        /// <summary>
        /// 保存先ディレクトリ
        /// </summary>
        [SerializeField]
        private DefaultAsset m_SaveDirectory;

        /// <summary>
        /// ステート管理
        /// </summary>
        private ValueStateManager<MapEditor> m_StateManager;

        /// <summary>
        /// マップサイズ
        /// </summary>
        public Vector2Int NewMapSize => m_NewMapSize;

        /// <summary>
        /// 新規マップのページ内セルサイズ
        /// </summary>
        public Vector2 NewMapPageCellSize => m_NewMapPageCellSize;

        /// <summary>
        /// 新規マップのページ内セル数
        /// </summary>
        public Vector2Int NewMapPageCellCount => m_NewMapPageCellCount;

        /// <summary>
        /// 無限スクロールビュー
        /// </summary>
        public InfiniteScrollView.InfiniteScrollView ScrollView => m_ScrollView;

        /// <summary>
        /// 保存先ディレクトリ
        /// </summary>
        public DefaultAsset SaveDirectory => m_SaveDirectory;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            m_StateManager = new(this);
            m_StateManager.AddTo(destroyCancellationToken);
            m_StateManager.ChangeState(new State.SelectNewOrLoadState()).Forget();
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        private void OnGUI()
        {
            (m_StateManager.CurrentState as IGUIState)?.OnGUI();
        }
    }
}
