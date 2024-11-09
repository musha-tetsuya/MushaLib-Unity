using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// スプライト選択
    /// </summary>
    internal class SelectSpriteState : ValueStateBase<MapEditor>, IGUIState
    {
        /// <summary>
        /// ObjectPickerコントロールID
        /// </summary>
        private int? m_ObjectPickerControlId;

        /// <summary>
        /// 選択したスプライト
        /// </summary>
        public Sprite SelectedSprite { get; private set; }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            if (!m_ObjectPickerControlId.HasValue)
            {
                m_ObjectPickerControlId = GUIUtility.GetControlID(FocusType.Passive);

                EditorGUIUtility.ShowObjectPicker<Sprite>(null, false, "", m_ObjectPickerControlId.Value);
            }
            else
            {
                switch (Event.current?.commandName)
                {
                    case "ObjectSelectorUpdated":
                        {
                            if (EditorGUIUtility.GetObjectPickerControlID() == m_ObjectPickerControlId)
                            {
                                SelectedSprite = (Sprite)EditorGUIUtility.GetObjectPickerObject();
                            }
                        }
                        break;

                    case "ObjectSelectorClosed":
                        {
                            StateManager.PopState();
                        }
                        break;
                }
            }
        }
    }
}
