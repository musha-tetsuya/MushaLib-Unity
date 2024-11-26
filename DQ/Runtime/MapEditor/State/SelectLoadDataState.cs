using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MushaLib.DQ.MapEditor.State
{
    /// <summary>
    /// ロードするマップデータの選択
    /// </summary>
    internal class SelectLoadDataState : ValueStateBase<MapEditor>, IGUIState
    {
        /// <summary>
        /// ObjectPickerコントロールID
        /// </summary>
        private int? m_ObjectPickerControlId;

        /// <summary>
        /// 選択したマップ編集データ
        /// </summary>
        public MapEditorData SelectedEditorData { get; private set; }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            if (!m_ObjectPickerControlId.HasValue)
            {
                m_ObjectPickerControlId = GUIUtility.GetControlID(FocusType.Passive);

                EditorGUIUtility.ShowObjectPicker<MapEditorData>(null, false, "", m_ObjectPickerControlId.Value);
            }
            else
            {
                switch (Event.current?.commandName)
                {
                    case "ObjectSelectorUpdated":
                        {
                            if (EditorGUIUtility.GetObjectPickerControlID() == m_ObjectPickerControlId)
                            {
                                SelectedEditorData = (MapEditorData)EditorGUIUtility.GetObjectPickerObject();
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
