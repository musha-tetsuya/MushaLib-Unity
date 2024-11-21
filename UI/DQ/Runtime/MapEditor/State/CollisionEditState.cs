using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// コリジョン編集ステート
    /// </summary>
    internal class CollisionEditState : ValueStateBase<MapEditor>, IGUIState, IElementClickHandler
    {
        /// <summary>
        /// マップ編集データ
        /// </summary>
        private readonly MapEditorData m_EditorData;

        /// <summary>
        /// 数値テキスト
        /// </summary>
        private string m_NumberText = "0";

        /// <summary>
        /// construct
        /// </summary>
        public CollisionEditState(MapEditorData editorData)
        {
            m_EditorData = editorData;
        }

        /// <summary>
        /// 開始
        /// </summary>
        public override UniTask StartAsync(CancellationToken cancellationToken)
        {
            foreach (var view in Value.ScrollView.ScrollElements.OfType<MapEditorElementView>())
            {
                view.TextMesh.enabled = true;
            }

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public override void Dispose()
        {
            foreach (var view in Value.ScrollView.ScrollElements.OfType<MapEditorElementView>())
            {
                view.TextMesh.enabled = false;
            }
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            m_NumberText = GUILayout.TextField(m_NumberText);

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                StateManager.PopState();
            }
        }

        /// <summary>
        /// 要素クリック時
        /// </summary>
        void IElementClickHandler.OnClickElement(MapEditorElementView view, int index)
        {
            if (!int.TryParse(m_NumberText, out var num))
            {
                Debug.LogWarning($"{m_NumberText} を数値に変換出来ません。");
                num = 0;
            }

            view.TextMesh.text = num.ToString();
            m_EditorData.ChipDatas[index].CollisionNum = num;
        }
    }
}
