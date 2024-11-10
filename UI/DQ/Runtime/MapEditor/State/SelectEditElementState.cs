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
using UnityEngine.AddressableAssets;

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
        /// クリック処理の破棄テーブル
        /// </summary>
        private DictionaryDisposable<MapEditorElementView, IDisposable> m_OnClickDisposableTable = new();

        /// <summary>
        /// construct
        /// </summary>
        public SelectEditElementState(MapEditorData editorData)
        {
            m_EditorData = editorData;
        }

        /// <summary>
        /// Start
        /// </summary>
        public override UniTask Start(CancellationToken cancellationToken)
        {
            if (m_EditorData == null)
            {
                // 新規作成
                m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                m_EditorData.Size = Value.Size;
                m_EditorData.Sprites = new Sprite[Value.Size.x * Value.Size.y];
            }
            else
            {
                // 上書きされないよう複製
                var oldEditorData = m_EditorData;
                m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                m_EditorData.Size = oldEditorData.Size;
                m_EditorData.Sprites = oldEditorData.Sprites.ToArray();
            }

            // スクロールビュー要素数設定
            Value.ScrollView.ElementCount = m_EditorData.Sprites.Length;

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
                        var selectSpriteState = new SelectSpriteState();

                        StateManager
                            .PushState(selectSpriteState, () =>
                            {
                                m_EditorData.Sprites[index] = view.Image.sprite = selectSpriteState.SelectedSprite;
                            })
                            .Forget();
                    });
            };

            // スクロールビュー開始
            Value.ScrollView.Initialize();

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// End
        /// </summary>
        public override void End()
        {
            m_OnClickDisposableTable.Dispose();
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
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
                        oldEditorData.Sprites = m_EditorData.Sprites;
                        EditorUtility.SetDirty(oldEditorData);
                        AssetDatabase.SaveAssetIfDirty(oldEditorData);
                    }

                    // 上書きされないよう新規インスタンスに
                    oldEditorData = m_EditorData;
                    m_EditorData = ScriptableObject.CreateInstance<MapEditorData>();
                    m_EditorData.Size = oldEditorData.Size;
                    m_EditorData.Sprites = oldEditorData.Sprites.ToArray();
                }
            }
        }
    }
}
