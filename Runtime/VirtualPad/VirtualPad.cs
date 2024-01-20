using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.InputSystem;

namespace MushaLib.VirtualPad
{
    /// <summary>
    /// 仮想パッド
    /// </summary>
    [ExecuteAlways]
    public class VirtualPad : MonoBehaviour
    {
        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// 左側のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_LeftSide;

        /// <summary>
        /// 右側のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RightSide;

        /// <summary>
        /// ボタン群
        /// </summary>
        [SerializeField]
        private VirtualPadButton[] m_Buttons;

        /// <summary>
        /// 入力操作の抽象化
        /// </summary>
        [SerializeField]
        private InputActionProperty m_InputActionProperty;

        /// <summary>
        /// リピート間隔
        /// </summary>
        [SerializeField]
        private float m_RepeatInterval = 0.1f;

        /// <summary>
        /// InputControlパスからボタンへのマップ
        /// </summary>
        private Dictionary<string, VirtualPadButton> m_PathToButtonMap;

        /// <summary>
        /// ボタンを押した時のイベント
        /// </summary>
        private Subject<(ButtonType buttonType, ButtonPressPhase pressPhase)> m_OnPress = new();

        /// <summary>
        /// ボタンを離した時のイベント
        /// </summary>
        private Subject<ButtonType> m_OnRelease = new();

        /// <summary>
        /// ボタンを押した時のイベントへのアクセス
        /// </summary>
        public IObservable<(ButtonType buttonType, ButtonPressPhase pressPhase)> OnPress => this.m_OnPress;

        /// <summary>
        /// ボタンを離した時のイベントへのアクセス
        /// </summary>
        public IObservable<ButtonType> OnRelease => this.m_OnRelease;

        /// <summary>
        /// リピート間隔
        /// </summary>
        public float RepeatInterval => this.m_RepeatInterval;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            this.m_InputActionProperty.action?.Disable();
            this.m_InputActionProperty.action?.Dispose();
        }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            // 自身のRectTransformに変更があったらスケールを再計算する
            this.m_RectTransform
                .OnRectTransformDimensionsChangeAsObservable()
                .Subscribe(_ => RecalcScale())
                .AddTo(this.destroyCancellationToken);

            // 入力操作イベントを設定
            this.m_InputActionProperty.action.started += OnInputActionStarted;
            this.m_InputActionProperty.action.performed += OnInputActionPerformed;
            this.m_InputActionProperty.action.canceled += OnInputActionCanceled;
            this.m_InputActionProperty.action.Enable();
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            // InputControlパスからボタンへのマップ作成
            this.m_PathToButtonMap = this.m_Buttons.ToDictionary(x => x.control.path, x => x);

            // 初回スケール計算
            RecalcScale();
        }

#if UNITY_EDITOR
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
            {
                RecalcScale();
            }
        }
#endif

        /// <summary>
        /// スケールの再計算
        /// </summary>
        private void RecalcScale()
        {
            if (this.m_RectTransform == null || this.m_LeftSide == null || this.m_RightSide == null)
            {
                return;
            }

            var minWidth = this.m_LeftSide.sizeDelta.x + this.m_RightSide.sizeDelta.x;
            var minHeight = Mathf.Max(this.m_LeftSide.sizeDelta.y, this.m_RightSide.sizeDelta.y);

            var scaleX = this.m_RectTransform.sizeDelta.x / minWidth;
            var scaleY = this.m_RectTransform.sizeDelta.y / minHeight;

            this.m_LeftSide.localScale =
            this.m_RightSide.localScale = Vector3.one * Mathf.Min(scaleX, scaleY, 1f);
        }

        /// <summary>
        /// ボタンを押した瞬間
        /// </summary>
        private void OnInputActionStarted(InputAction.CallbackContext context)
        {
            if (this.m_PathToButtonMap.TryGetValue(context.control.path, out var button))
            {
                this.m_OnPress.OnNext((button.buttonType, ButtonPressPhase.Pressed));
            }
        }

        /// <summary>
        /// ボタンの長押しが成立した時
        /// </summary>
        private void OnInputActionPerformed(InputAction.CallbackContext context)
        {
            if (this.m_PathToButtonMap.TryGetValue(context.control.path, out var button))
            {
                this.m_OnPress.OnNext((button.buttonType, ButtonPressPhase.LongPressed));

                // リピートするなら
                if (this.m_RepeatInterval > 0f)
                {
                    var cts = CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken, button.destroyCancellationToken);
                    cts.Token.Register(cts.Dispose);

                    // リピート開始
                    UniTaskAsyncEnumerable.Interval(TimeSpan.FromSeconds(this.m_RepeatInterval))
                        .Subscribe(_ => this.m_OnPress.OnNext((button.buttonType, ButtonPressPhase.Repeat)))
                        .AddTo(cts.Token);

                    // ボタンを離したらリピート終了
                    this.m_OnRelease.Where(x => x == button.buttonType)
                        .Subscribe(x => cts.Cancel())
                        .AddTo(cts.Token);
                }
            }
        }

        /// <summary>
        /// ボタンを離した時
        /// </summary>
        private void OnInputActionCanceled(InputAction.CallbackContext context)
        {
            if (this.m_PathToButtonMap.TryGetValue(context.control.path, out var button))
            {
                this.m_OnRelease.OnNext(button.buttonType);
            }
        }
    }
}