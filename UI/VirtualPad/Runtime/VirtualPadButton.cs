using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.UI;

namespace MushaLib.UI.VirtualPad
{
    /// <summary>
    /// 仮想パッドボタン
    /// </summary>
    public class VirtualPadButton : Button
    {
        /// <summary>
        /// イベントタイプ
        /// </summary>
        public enum EventType
        {
            PointerEvent,
            InputAction,
        }

        /// <summary>
        /// 入力操作の抽象化
        /// </summary>
        [SerializeField]
        private InputActionProperty m_InputAction;

        /// <summary>
        /// 押下時
        /// </summary>
        private Subject<EventType> m_OnPressed = new();

        /// <summary>
        /// 離脱時
        /// </summary>
        private Subject<EventType> m_OnReleased = new();

        /// <summary>
        /// 押下時
        /// </summary>
        public IObservable<EventType> OnPressed => m_OnPressed;

        /// <summary>
        /// 離脱時
        /// </summary>
        public IObservable<EventType> OnReleased => m_OnReleased;

        /// <summary>
        /// OnDestroy
        /// </summary>
        protected override void OnDestroy()
        {
            // 入力操作イベントを解除
            this.m_InputAction.action?.Disable();
            this.m_InputAction.action?.Dispose();

            this.m_OnPressed.Dispose();
            this.m_OnReleased.Dispose();

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

            this.m_OnPressed.OnNext(EventType.PointerEvent);
        }

        /// <summary>
        /// OnPointerUp
        /// </summary>
        public override void OnPointerUp(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
                return;

            base.OnPointerUp(eventData);

            this.m_OnReleased.OnNext(EventType.PointerEvent);
        }

        /// <summary>
        /// OnPointerExit
        /// </summary>
        public override void OnPointerExit(PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            this.m_OnReleased.OnNext(EventType.PointerEvent);
        }

        /// <summary>
        /// InputAction入力開始時
        /// </summary>
        private void OnInputActionStarted(InputAction.CallbackContext context)
        {
            this.m_OnPressed.OnNext(EventType.InputAction);
        }

        /// <summary>
        /// InputAction入力終了時
        /// </summary>
        private void OnInputActionCanceled(InputAction.CallbackContext context)
        {
            this.m_OnReleased.OnNext(EventType.InputAction);
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

                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_InputAction"));

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}