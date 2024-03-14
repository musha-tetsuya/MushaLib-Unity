using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MushaLib.VirtualPad
{
    /// <summary>
    /// 仮想パッドボタン
    /// </summary>
    public class VirtualPadButton : Button
    {
        /// <summary>
        /// ボタンID
        /// </summary>
        [SerializeField]
        private int m_ButtonId;

        /// <summary>
        /// 押下時イベント
        /// </summary>
        [SerializeField]
        private UnityEvent<int, object> m_OnPressed;

        /// <summary>
        /// 離脱時イベント
        /// </summary>
        [SerializeField]
        private UnityEvent<int, object> m_OnReleased;

        /// <summary>
        /// 入力操作の抽象化
        /// </summary>
        [SerializeField]
        private InputActionProperty m_InputAction;

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected override void OnDestroy()
        {
            // 入力操作イベントを解除
            this.m_InputAction.action?.Disable();
            this.m_InputAction.action?.Dispose();

            base.OnDestroy();
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected override void Awake()
        {
            base.Awake();

            // 入力操作イベントを設定
            this.m_InputAction.action.started += OnInputActionStarted;
            this.m_InputAction.action.canceled += OnInputActionCanceled;
            this.m_InputAction.action.Enable();
        }

        /// <summary>
        /// OnPointerDown
        /// </summary>
        public override void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerDown(eventData);

            this.m_OnPressed.Invoke(this.m_ButtonId, this);
        }

        /// <summary>
        /// OnPointerUp
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerUp(eventData);

            this.m_OnReleased.Invoke(this.m_ButtonId, this);
        }

        /// <summary>
        /// OnPointerExit
        /// </summary>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            this.m_OnReleased.Invoke(this.m_ButtonId, this);
        }

        /// <summary>
        /// InputAction入力開始時
        /// </summary>
        private void OnInputActionStarted(InputAction.CallbackContext context)
        {
            this.m_OnPressed.Invoke(this.m_ButtonId, this.m_InputAction.action);
        }

        /// <summary>
        /// InputAction入力終了時
        /// </summary>
        private void OnInputActionCanceled(InputAction.CallbackContext context)
        {
            this.m_OnReleased.Invoke(this.m_ButtonId, this.m_InputAction.action);
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

                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_ButtonId"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnPressed"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_OnReleased"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_InputAction"));

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}