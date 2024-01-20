using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

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
        /// 長押し成立時間
        /// </summary>
        [SerializeField]
        private float m_HoldTime = 0.5f;

        /// <summary>
        /// リピート間隔
        /// </summary>
        [SerializeField]
        private float m_RepeatInterval = 0.1f;

        /// <summary>
        /// ボタンキャンセルトークン
        /// </summary>
        private Dictionary<ButtonType, CancellationTokenSource> m_ButtonCancellations = new();

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
        public async void OnInputStarted(ButtonType buttonType)
        {
            if (this.m_ButtonCancellations.TryGetValue(buttonType, out var cts))
            {
                cts.Cancel(true);
                cts.Dispose();
            }

            cts = this.m_ButtonCancellations[buttonType] = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            this.m_OnPress.OnNext((buttonType, ButtonPressPhase.Pressed));

            try
            {
                // 長押し待機
                await UniTask.Delay((int)(this.m_HoldTime * 1000), cancellationToken: cts.Token);
            }
            catch
            {
                return;
            }

            this.m_OnPress.OnNext((buttonType, ButtonPressPhase.LongPressed));

            while (true)
            {
                try
                {
                    if (this.m_RepeatInterval <= 0f)
                    {
                        // リピートONになるまで待機
                        await UniTask.WaitUntil(() => this.m_RepeatInterval > 0f, cancellationToken: cts.Token);
                    }

                    // リピート待機
                    await UniTask.Delay((int)(this.m_RepeatInterval * 1000), cancellationToken: cts.Token);
                }
                catch
                {
                    return;
                }

                this.m_OnPress.OnNext((buttonType, ButtonPressPhase.Repeat));
            }
        }

        /// <summary>
        /// ボタンを離した時
        /// </summary>
        public void OnInputCanceled(ButtonType buttonType)
        {
            if (this.m_ButtonCancellations.TryGetValue(buttonType, out var cts))
            {
                cts.Cancel();
                cts.Dispose();
                this.m_ButtonCancellations.Remove(buttonType);

                this.m_OnRelease.OnNext(buttonType);
            }
        }
    }
}