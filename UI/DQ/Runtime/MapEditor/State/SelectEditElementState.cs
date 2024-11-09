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
        /// EditorUserSettingsのキー
        /// </summary>
        private static readonly string EditorUserSettingsKey = typeof(SelectEditElementState).FullName;

        /// <summary>
        /// マップデータ
        /// </summary>
        private MapData m_MapData;

        /// <summary>
        /// クリック処理の破棄テーブル
        /// </summary>
        private DictionaryDisposable<MapEditorElementView, IDisposable> m_OnClickDisposableTable = new();

        /// <summary>
        /// construct
        /// </summary>
        public SelectEditElementState(MapData mapData)
        {
            m_MapData = mapData;
        }

        /// <summary>
        /// Start
        /// </summary>
        public override UniTask Start(CancellationToken cancellationToken)
        {
            if (m_MapData == null)
            {
                // 新規作成
                m_MapData = ScriptableObject.CreateInstance<MapData>();
                m_MapData.Size = Value.Size;
                m_MapData.Sprites = new AssetReferenceSprite[Value.Size.x * Value.Size.y];
            }
            else
            {
                // 上書きされないよう複製
                var oldMapData = m_MapData;
                m_MapData = ScriptableObject.CreateInstance<MapData>();
                m_MapData.Size = oldMapData.Size;
                m_MapData.Sprites = oldMapData.Sprites.ToArray();
            }

            // スクロールビュー要素数設定
            Value.ScrollView.ElementCount = m_MapData.Sprites.Length;

            // スクロールビュー要素更新時
            Value.ScrollView.OnUpdateElement += (element, index) =>
            {
                var view = element as MapEditorElementView;

                // スプライト設定
                view.Image.sprite = m_MapData.Sprites[index]?.editorAsset as Sprite;

                // クリック時
                m_OnClickDisposableTable[view] = view.Button
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        var selectSpriteState = new SelectSpriteState();

                        StateManager
                            .PushState(selectSpriteState, () =>
                            {
                                view.Image.sprite = selectSpriteState.SelectedSprite;

                                if (selectSpriteState.SelectedSprite == null)
                                {
                                    m_MapData.Sprites[index] = null;
                                }
                                else if (AssetDatabase.TryGetGUIDAndLocalFileIdentifier(selectSpriteState.SelectedSprite, out string guid, out long localId))
                                {
                                    m_MapData.Sprites[index] = new(guid);
                                }
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
                var path = EditorUtility.SaveFilePanelInProject("Save MapData", "", "asset", "", EditorUserSettings.GetConfigValue($"{EditorUserSettingsKey}.path"));

                if (!string.IsNullOrEmpty(path))
                {
                    var oldMapData = AssetDatabase.LoadAssetAtPath<MapData>(path);
                    if (oldMapData == null)
                    {
                        // 新規保存
                        AssetDatabase.CreateAsset(m_MapData, path);
                    }
                    else
                    {
                        // 上書き保存
                        oldMapData.Size = m_MapData.Size;
                        oldMapData.Sprites = m_MapData.Sprites;
                        AssetDatabase.SaveAssetIfDirty(oldMapData);
                    }

                    EditorUserSettings.SetConfigValue($"{EditorUserSettingsKey}.path", path);

                    // 上書きされないよう新規インスタンスに
                    oldMapData = m_MapData;
                    m_MapData = ScriptableObject.CreateInstance<MapData>();
                    m_MapData.Size = oldMapData.Size;
                    m_MapData.Sprites = oldMapData.Sprites.ToArray();
                }
            }
        }
    }
}
