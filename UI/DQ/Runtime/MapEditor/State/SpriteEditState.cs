using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// スプライト編集ステート
    /// </summary>
    internal class SpriteEditState : ValueStateBase<MapEditor>, IGUIState, IElementClickHandler
    {
        /// <summary>
        /// マップ編集データ
        /// </summary>
        private readonly MapEditorData m_EditorData;

        /// <summary>
        /// Ctrlキー入力監視
        /// </summary>
        private InputAction m_CtrlAction;

        /// <summary>
        /// construct
        /// </summary>
        public SpriteEditState(MapEditorData editorData)
        {
            m_EditorData = editorData;
        }

        /// <summary>
        /// 開始
        /// </summary>
        public override UniTask StartAsync(CancellationToken cancellationToken)
        {
            m_CtrlAction = new("Ctrl", InputActionType.Button);
            m_CtrlAction.AddBinding("<Keyboard>/ctrl");
            m_CtrlAction.AddBinding("<Keyboard>/ctrl+right");
            m_CtrlAction.Enable();

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public override void Dispose()
        {
            m_CtrlAction.Disable();
            m_CtrlAction.Dispose();
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            GUILayout.Label("Ctrl押しながらクリックで、一つ前のスプライトを貼り付け");

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
            if (m_CtrlAction.ReadValue<float>() > 0)
            {
                // 一つ前のスプライトを貼り付け
                m_EditorData.ChipDatas[index].Sprite = view.Image.sprite = Value.CurrentSpriteImage.sprite;
            }
            else
            {
                m_CtrlAction.Disable();

                var selectSpriteState = new SelectSpriteState();

                // スプライト選択ステートへ
                StateManager
                    .PushState(selectSpriteState, () =>
                    {
                        // 選択したスプライトを貼り付け
                        m_EditorData.ChipDatas[index].Sprite = view.Image.sprite = Value.CurrentSpriteImage.sprite = selectSpriteState.SelectedSprite;

                        m_CtrlAction.Enable();
                    })
                    .Forget();
            }
        }
    }
}
