using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace MushaLib.UI.VirtualPad
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
        private Dictionary<int, (CancellationTokenSource cts, List<object> factors)> m_ButtonCancellations = new();

        /// <summary>
        /// ボタンを押した時のイベント
        /// </summary>
        private Subject<(int buttonId, ButtonPressPhase pressPhase)> m_OnPress = new();

        /// <summary>
        /// ボタンを離した時のイベント
        /// </summary>
        private Subject<int> m_OnRelease = new();

        /// <summary>
        /// ボタンを押した時のイベントへのアクセス
        /// </summary>
        public IObservable<(int buttonId, ButtonPressPhase pressPhase)> OnPress => this.m_OnPress;

        /// <summary>
        /// ボタンを離した時のイベントへのアクセス
        /// </summary>
        public IObservable<int> OnRelease => this.m_OnRelease;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            foreach (var cancellation in this.m_ButtonCancellations.Values)
            {
                cancellation.cts.Cancel();
                cancellation.cts.Dispose();
            }

            this.m_ButtonCancellations.Clear();
        }

        /// <summary>
        /// 自身のRectTransformに変更があったらスケールを再計算する
        /// </summary>
        private void OnRectTransformDimensionsChange()
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
        public void OnButtonPressed(int buttonId, object factor)
        {
            // 既に押下中なら新規要因を追加
            if (this.m_ButtonCancellations.TryGetValue(buttonId, out var cancellation))
            {
                cancellation.factors.Add(factor);
                return;
            }

            // キャンセルトークン作成
            cancellation = (CancellationTokenSource.CreateLinkedTokenSource(this.destroyCancellationToken), new() { factor });
            this.m_ButtonCancellations[buttonId] = cancellation;

            this.m_OnPress.OnNext((buttonId, ButtonPressPhase.Pressed));

            UniTask.Void(async () =>
            {
                try
                {
                    // 長押し待機
                    await UniTask.Delay((int)(this.m_HoldTime * 1000), cancellationToken: cancellation.cts.Token);
                }
                catch
                {
                    return;
                }

                this.m_OnPress.OnNext((buttonId, ButtonPressPhase.LongPressed));

                while (true)
                {
                    try
                    {
                        if (this.m_RepeatInterval <= 0f)
                        {
                            // リピートONになるまで待機
                            await UniTask.WaitUntil(() => this.m_RepeatInterval > 0f, cancellationToken: cancellation.cts.Token);
                        }

                        // リピート待機
                        await UniTask.Delay((int)(this.m_RepeatInterval * 1000), cancellationToken: cancellation.cts.Token);
                    }
                    catch
                    {
                        return;
                    }

                    this.m_OnPress.OnNext((buttonId, ButtonPressPhase.Repeat));
                }
            });
        }

        /// <summary>
        /// ボタンを離した時
        /// </summary>
        public void OnButtonReleased(int buttonId, object factor)
        {
            if (this.m_ButtonCancellations.TryGetValue(buttonId, out var cancellation))
            {
                // 要因除去
                cancellation.factors.Remove(factor);

                // 全要因が無くなった
                if (cancellation.factors.Count == 0)
                {
                    // 押下状態キャンセル
                    cancellation.cts.Cancel();
                    cancellation.cts.Dispose();
                    this.m_ButtonCancellations.Remove(buttonId);

                    this.m_OnRelease.OnNext(buttonId);
                }
            }
        }
    }
}