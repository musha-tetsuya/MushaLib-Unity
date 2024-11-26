using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.MapEditor.State
{
    /// <summary>
    /// 編集モードの選択
    /// </summary>
    internal class SelectEditModeState : ValueStateBase<MapEditor>, IGUIState
    {
        /// <summary>
        /// クリック処理の破棄テーブル
        /// </summary>
        private DictionaryDisposable<MapEditorElementView, IDisposable> m_OnClickDisposableTable = new();

        /// <summary>
        /// 開始
        /// </summary>
        public override UniTask StartAsync(CancellationToken cancellationToken)
        {
            // スクロールビュー要素数設定
            Value.ScrollView.ElementCount = Value.EditorData.Sprites.Length;

            // スクロールビューページレイアウト設定
            Value.ScrollView.PageLayout.CellSize = Value.EditorData.PageCellSize;
            Value.ScrollView.PageLayout.CellCount = Value.EditorData.PageCellCount;

            // スクロールビュー要素更新時
            Value.ScrollView.OnUpdateElement += (element, index) =>
            {
                var view = element as MapEditorElementView;

                // スプライト設定
                view.Image.sprite = Value.EditorData.Sprites[index];

                // オプション設定
                view.TextMesh.text = Value.OptionData[index].ToString();

                // クリック時
                m_OnClickDisposableTable[view] = view.Button
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        (StateManager.CurrentState as IMapEditorElementClickHandler)?.OnClickElement(view, index);
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
                StateManager.PushState(new SpriteEditState()).Forget();
            }

            if (GUILayout.Button("オプション編集"))
            {
                StateManager.PushState(new OptionEditState()).Forget();
            }
        }
    }
}
