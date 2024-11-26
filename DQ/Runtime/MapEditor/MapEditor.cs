using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using MushaLib.UI.InfiniteScroll;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.MapEditor
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
        private InfiniteScrollView m_ScrollView;

        /// <summary>
        /// 現在のスプライトイメージ
        /// </summary>
        [SerializeField]
        private Image m_CurrentSpriteImage;

        /// <summary>
        /// 保存先ディレクトリ
        /// </summary>
        [SerializeField]
        private DefaultAsset m_SaveDirectory;

        /// <summary>
        /// オプションデータディレクトリ
        /// </summary>
        [SerializeField]
        private DefaultAsset m_OptionDataDirectory;

        /// <summary>
        /// ステート管理
        /// </summary>
        private ValueStateManager<MapEditor> m_StateManager;

        /// <summary>
        /// 無限スクロールビュー
        /// </summary>
        public InfiniteScrollView ScrollView => m_ScrollView;

        /// <summary>
        /// 現在のスプライトイメージ
        /// </summary>
        public Image CurrentSpriteImage => m_CurrentSpriteImage;

        /// <summary>
        /// 保存先ディレクトリ
        /// </summary>
        public string SaveDirectory => m_SaveDirectory != null ? AssetDatabase.GetAssetPath(m_SaveDirectory) : "";

        /// <summary>
        /// オプションデータディレクトリ
        /// </summary>
        public string OptionDataDirectory => m_OptionDataDirectory != null ? AssetDatabase.GetAssetPath(m_OptionDataDirectory) : "";

        /// <summary>
        /// 編集データ
        /// </summary>
        public MapEditorData EditorData { get; set; }

        /// <summary>
        /// オプションデータ
        /// </summary>
        public int[] OptionData { get; set; }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            m_StateManager = new(this);
            m_StateManager.AddTo(destroyCancellationToken);

            if (EditorUtility.DisplayDialog("MapEditor", "マップデータ編集", "Load", "New"))
            {
                var selectLoadDataState = new State.SelectLoadDataState();

                // ロードするデータの選択ステートへ
                m_StateManager
                    .PushState(selectLoadDataState, () =>
                    {
                        // データが選択された？
                        if (selectLoadDataState.SelectedEditorData != null)
                        {
                            EditorData = MapEditorData.Copy(selectLoadDataState.SelectedEditorData);

                            OptionData = new int[EditorData.Sprites.Length];

                            // 編集モード選択ステートへ
                            m_StateManager.ChangeState(new State.SelectEditModeState()).Forget();
                        }
                    })
                    .Forget();
            }
            else
            {
                // 新規作成
                EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                EditorData.Size = m_NewMapSize;
                EditorData.Sprites = new Sprite[m_NewMapSize.x * m_NewMapSize.y];
                EditorData.PageCellSize = m_NewMapPageCellSize;
                EditorData.PageCellCount = m_NewMapPageCellCount;

                OptionData = new int[EditorData.Sprites.Length];

                m_StateManager.ChangeState(new State.SelectEditModeState()).Forget();
            }
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
