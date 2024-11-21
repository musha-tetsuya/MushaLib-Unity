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

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// 編集モードの選択
    /// </summary>
    internal class SelectEditModeState : ValueStateBase<MapEditor>, IGUIState
    {
        /// <summary>
        /// マップ編集データ
        /// </summary>
        private MapEditorData m_EditorData;

        /// <summary>
        /// クリック処理の破棄テーブル
        /// </summary>
        private DictionaryDisposable<MapEditorElementView, IDisposable> m_OnClickDisposableTable = new();

        /// <summary>
        /// construct
        /// </summary>
        public SelectEditModeState(MapEditorData editorData)
        {
            m_EditorData = editorData;
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
                m_EditorData.ChipDatas = Enumerable.Range(0, Value.NewMapSize.x * Value.NewMapSize.y).Select(x => new MapChipEditorData()).ToArray();
                m_EditorData.PageCellSize = Value.NewMapPageCellSize;
                m_EditorData.PageCellCount = Value.NewMapPageCellCount;

                for (int i = 0; i < m_EditorData.ChipDatas.Length; i++)
                {
                    m_EditorData.ChipDatas[i] = new();
                }
            }
            else
            {
                // 上書きされないよう複製
                var oldEditorData = m_EditorData;
                m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                m_EditorData.Size = oldEditorData.Size;
                m_EditorData.ChipDatas = oldEditorData.ChipDatas.ToArray();
                m_EditorData.PageCellSize = oldEditorData.PageCellSize;
                m_EditorData.PageCellCount = oldEditorData.PageCellCount;
            }

            // スクロールビュー要素数設定
            Value.ScrollView.ElementCount = m_EditorData.ChipDatas.Length;

            // スクロールビューページレイアウト設定
            Value.ScrollView.PageLayout.CellSize = m_EditorData.PageCellSize;
            Value.ScrollView.PageLayout.CellCount = m_EditorData.PageCellCount;

            // スクロールビュー要素更新時
            Value.ScrollView.OnUpdateElement += (element, index) =>
            {
                var view = element as MapEditorElementView;

                // スプライト設定
                view.Image.sprite = m_EditorData.ChipDatas[index].Sprite;

                // コリジョン設定
                view.TextMesh.text = m_EditorData.ChipDatas[index].CollisionNum.ToString();

                // クリック時
                m_OnClickDisposableTable[view] = view.Button
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        (StateManager.CurrentState as IElementClickHandler)?.OnClickElement(view, index);
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
            m_OnClickDisposableTable.Dispose();
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            if (GUILayout.Button("スプライト編集"))
            {
                StateManager.PushState(new SpriteEditState(m_EditorData)).Forget();
            }

            if (GUILayout.Button("コリジョン編集"))
            {
                StateManager.PushState(new CollisionEditState(m_EditorData)).Forget();
            }

            if (GUILayout.Button("Save"))
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
                        oldEditorData.ChipDatas = m_EditorData.ChipDatas;
                        oldEditorData.PageCellSize = m_EditorData.PageCellSize;
                        oldEditorData.PageCellCount = m_EditorData.PageCellCount;
                        EditorUtility.SetDirty(oldEditorData);
                        AssetDatabase.SaveAssetIfDirty(oldEditorData);
                    }

                    // 上書きされないよう新規インスタンスに
                    oldEditorData = m_EditorData;
                    m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                    m_EditorData.Size = oldEditorData.Size;
                    m_EditorData.ChipDatas = oldEditorData.ChipDatas.ToArray();
                    m_EditorData.PageCellSize = oldEditorData.PageCellSize;
                    m_EditorData.PageCellCount = oldEditorData.PageCellCount;
                }
            }
        }
    }
}
