using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// 編集する要素の選択
    /// </summary>
    internal class SelectEditElementState : ValueStateBase<MapEditor>, IGUIState
    {
        /// <summary>
        /// マップ編集データ
        /// </summary>
        private MapEditorData m_EditorData;

        /// <summary>
        /// Ctrlキー入力の監視
        /// </summary>
        private InputAction m_CtrlAction;

        /// <summary>
        /// クリック処理の破棄テーブル
        /// </summary>
        private DictionaryDisposable<MapEditorElementView, IDisposable> m_OnClickDisposableTable = new();

        /// <summary>
        /// construct
        /// </summary>
        public SelectEditElementState(MapEditorData editorData)
        {
            m_EditorData = editorData;

            m_CtrlAction = new("Ctrl", InputActionType.Button);
            m_CtrlAction.AddBinding("<Keyboard>/ctrl");
            m_CtrlAction.AddBinding("<Keyboard>/ctrl+right");
            m_CtrlAction.Enable();
        }

        /// <summary>
        /// 開始
        /// </summary>
        public override UniTask StartAsync(CancellationToken cancellationToken)
        {
            if (m_EditorData == null)
            {
                // 新規作成
                m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                m_EditorData.Size = Value.NewMapSize;
                m_EditorData.Sprites = new Sprite[Value.NewMapSize.x * Value.NewMapSize.y];
                m_EditorData.PageCellSize = Value.NewMapPageCellSize;
                m_EditorData.PageCellCount = Value.NewMapPageCellCount;
            }
            else
            {
                // 上書きされないよう複製
                var oldEditorData = m_EditorData;
                m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                m_EditorData.Size = oldEditorData.Size;
                m_EditorData.Sprites = oldEditorData.Sprites.ToArray();
                m_EditorData.PageCellSize = oldEditorData.PageCellSize;
                m_EditorData.PageCellCount = oldEditorData.PageCellCount;
            }

            // スクロールビュー要素数設定
            Value.ScrollView.ElementCount = m_EditorData.Sprites.Length;

            // スクロールビューページレイアウト設定
            Value.ScrollView.PageLayout.CellSize = m_EditorData.PageCellSize;
            Value.ScrollView.PageLayout.CellCount = m_EditorData.PageCellCount;

            // スクロールビュー要素更新時
            Value.ScrollView.OnUpdateElement += (element, index) =>
            {
                var view = element as MapEditorElementView;

                // スプライト設定
                view.Image.sprite = m_EditorData.Sprites[index];

                // クリック時
                m_OnClickDisposableTable[view] = view.Button
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        if (m_CtrlAction.ReadValue<float>() > 0)
                        {
                            m_EditorData.Sprites[index] = view.Image.sprite = Value.CurrentSpriteImage.sprite;
                        }
                        else
                        {
                            var selectSpriteState = new SelectSpriteState();

                            StateManager
                                .PushState(selectSpriteState, () =>
                                {
                                    m_EditorData.Sprites[index] = view.Image.sprite = Value.CurrentSpriteImage.sprite = selectSpriteState.SelectedSprite;
                                })
                                .Forget();
                        }
                    });
            };

            // スクロールビュー開始
            Value.ScrollView.Initialize();

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public override void Dispose()
        {
            m_CtrlAction.Disable();

            m_OnClickDisposableTable.Dispose();
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            GUILayout.Label("Ctrl押しながらクリックで、一つ前のスプライトを貼り付け");

            if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
            {
                var directory = Value.SaveDirectory != null ? AssetDatabase.GetAssetPath(Value.SaveDirectory) : "";
                var path = EditorUtility.SaveFilePanelInProject("Save MapData", "", "asset", "", directory);

                if (!string.IsNullOrEmpty(path))
                {
                    var oldEditorData = AssetDatabase.LoadAssetAtPath<MapEditorData>(path);
                    if (oldEditorData == null)
                    {
                        // 新規保存
                        AssetDatabase.CreateAsset(m_EditorData, path);
                    }
                    else
                    {
                        // 上書き保存
                        oldEditorData.Size = m_EditorData.Size;
                        oldEditorData.Sprites = m_EditorData.Sprites;
                        oldEditorData.PageCellSize = m_EditorData.PageCellSize;
                        oldEditorData.PageCellCount = m_EditorData.PageCellCount;
                        EditorUtility.SetDirty(oldEditorData);
                        AssetDatabase.SaveAssetIfDirty(oldEditorData);
                    }

                    // 上書きされないよう新規インスタンスに
                    oldEditorData = m_EditorData;
                    m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                    m_EditorData.Size = oldEditorData.Size;
                    m_EditorData.Sprites = oldEditorData.Sprites.ToArray();
                    m_EditorData.PageCellSize = oldEditorData.PageCellSize;
                    m_EditorData.PageCellCount = oldEditorData.PageCellCount;
                }
            }
        }
    }
}
