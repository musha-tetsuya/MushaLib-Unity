using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEditor;
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
        /// Ctrlキー入力監視
        /// </summary>
        private InputAction m_CtrlAction;

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

            if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
            {
                var path = EditorUtility.SaveFilePanelInProject("Save MapData", "", "asset", "", Value.SaveDirectory);

                if (!string.IsNullOrEmpty(path))
                {
                    var oldEditorData = AssetDatabase.LoadAssetAtPath<MapEditorData>(path);
                    if (oldEditorData == null)
                    {
                        // 新規保存
                        AssetDatabase.CreateAsset(Value.EditorData, path);
                    }
                    else
                    {
                        // 上書き保存
                        oldEditorData.Size = Value.EditorData.Size;
                        oldEditorData.Sprites = Value.EditorData.Sprites;
                        oldEditorData.PageCellSize = Value.EditorData.PageCellSize;
                        oldEditorData.PageCellCount = Value.EditorData.PageCellCount;
                        EditorUtility.SetDirty(oldEditorData);
                        AssetDatabase.SaveAssetIfDirty(oldEditorData);
                    }

                    // 上書きされないよう新規インスタンスに
                    Value.EditorData = MapEditorData.Copy(Value.EditorData);
                }
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
                Value.EditorData.Sprites[index] = view.Image.sprite = Value.CurrentSpriteImage.sprite;
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
                        Value.EditorData.Sprites[index] = view.Image.sprite = Value.CurrentSpriteImage.sprite = selectSpriteState.SelectedSprite;

                        m_CtrlAction.Enable();
                    })
                    .Forget();
            }
        }
    }
}
