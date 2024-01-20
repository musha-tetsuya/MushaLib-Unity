using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
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
        /// 入力開始時イベント
        /// </summary>
        [SerializeField]
        private UnityEvent<ButtonType> m_OnInputStarted;

        /// <summary>
        /// 入力キャンセル時イベント
        /// </summary>
        [SerializeField]
        private UnityEvent<ButtonType> m_OnInputCanceled;

        /// <summary>
        /// OnPointerDown
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerDown(eventData);

            this.m_OnInputStarted.Invoke(this.m_ButtonType);
        }

        /// <summary>
        /// OnPointerUp
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerUp(eventData);

            this.m_OnInputCanceled.Invoke(this.m_ButtonType);
        }

        /// <summary>
        /// OnPointerExit
        /// </summary>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            this.m_OnInputCanceled.Invoke(this.m_ButtonType);
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
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnInputStarted"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnInputCanceled"));

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}