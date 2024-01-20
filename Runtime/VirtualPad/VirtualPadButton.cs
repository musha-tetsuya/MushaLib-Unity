using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem.Layouts;
using UnityEngine.UI;

namespace MushaLib.VirtualPad
{
    /// <summary>
    /// 仮想パッドボタン
    /// </summary>
    public class VirtualPadButton : Button
    {
        /// <summary>
        /// ボタンタイプ
        /// </summary>
        [SerializeField]
        private ButtonType m_ButtonType;

        /// <summary>
        /// コントロールパス
        /// </summary>
        [SerializeField, InputControl]
        private string[] m_ControlPaths;

        /// <summary>
        /// 押下時イベント
        /// </summary>
        [SerializeField]
        private UnityEvent<VirtualPadButton> m_OnPressed;

        /// <summary>
        /// 離脱時イベント
        /// </summary>
        [SerializeField]
        private UnityEvent<VirtualPadButton> m_OnReleased;

        /// <summary>
        /// ボタンタイプ
        /// </summary>
        public ButtonType ButtonType => this.m_ButtonType;

        /// <summary>
        /// コントロールパス
        /// </summary>
        public string[] ControlPaths => this.m_ControlPaths;

        /// <summary>
        /// OnPointerDown
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerDown(eventData);

            this.m_OnPressed.Invoke(this);
        }

        /// <summary>
        /// OnPointerUp
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerUp(eventData);

            this.m_OnReleased.Invoke(this);
        }

        /// <summary>
        /// OnPointerExit
        /// </summary>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            this.m_OnReleased.Invoke(this);
        }

#if UNITY_EDITOR
        /// <summary>
        /// カスタムインスペクター
        /// </summary>
        [CustomEditor(typeof(VirtualPadButton))]
        private class CustomInspector : ButtonEditor
        {
            /// <summary>
            /// OnInspectorGUI
            /// </summary>
            public override void OnInspectorGUI()
            {
                base.OnInspectorGUI();

                serializedObject.Update();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ButtonType"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ControlPaths"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnPressed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnReleased"));

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}