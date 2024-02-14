using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using TMPro;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace MushaLib.DQ.UI.MessageWindow
{
    /// <summary>
    /// メッセージウィンドウ
    /// </summary>
    public class MessageWindow : MonoBehaviour
    {
        /// <summary>
        /// テキスト表示範囲
        /// </summary>
        [SerializeField]
        private RectTransform m_TextArea;

        /// <summary>
        /// テキスト
        /// </summary>
        [SerializeField]
        private TextMeshProUGUI m_Text;

        /// <summary>
        /// テキスト表示間隔
        /// </summary>
        [SerializeField]
        private float m_Interval = 0.1f;

        /// <summary>
        /// テキストスクロール時間
        /// </summary>
        [SerializeField]
        private float m_ScrollDuration = 0.1f;

        /// <summary>
        /// 矢印
        /// </summary>
        [SerializeField]
        private Arrow m_Arrow;

        /// <summary>
        /// 言語切り替え中かどうか
        /// </summary>
        private bool m_IsChangingLocale;

        /// <summary>
        /// 言語切り替え待機中かどうか
        /// </summary>
        private bool m_IsWaitingLocaleChange;

        /// <summary>
        /// 未完了イベント
        /// </summary>
        private List<IEvent> m_UncompletedEvents = new();

        /// <summary>
        /// 完了済文字列イベント
        /// </summary>
        private List<IStringEvent> m_CompletedStringEvents = new();

        /// <summary>
        /// クリック時
        /// </summary>
        private Subject<Unit> onClick = new();

        /// <summary>
        /// 言語切り替えによるキャンセルトークン
        /// </summary>
        private CancellationTokenSource m_LocaleCancellation = new();

        /// <summary>
        /// ユーザー操作によるキャンセルトークン
        /// </summary>
        private CancellationTokenSource m_UserCancellation;

        /// <summary>
        /// テキスト表示間隔
        /// </summary>
        public float Interval
        {
            get => m_Interval;
            set => m_Interval = value;
        }

        /// <summary>
        /// 矢印
        /// </summary>
        public Arrow Arrow => m_Arrow;

        /// <summary>
        /// クリック時
        /// </summary>
        public IObservable<Unit> OnClick => onClick;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;

            m_LocaleCancellation?.Cancel();
            m_LocaleCancellation?.Dispose();
            m_LocaleCancellation = null;

            m_UserCancellation?.Cancel();
            m_UserCancellation?.Dispose();
            m_UserCancellation = null;
        }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        /// <summary>
        /// 言語切り替え時
        /// </summary>
        private async void OnSelectedLocaleChanged(Locale locale)
        {
            m_LocaleCancellation?.Cancel();
            m_LocaleCancellation?.Dispose();
            m_LocaleCancellation = new();

            // 完了済文字列イベントがあるなら
            if (m_CompletedStringEvents.Count > 0)
            {
                m_IsChangingLocale = true;

                string[] messages = null;

                using var cts = CancellationTokenSource.CreateLinkedTokenSource(m_LocaleCancellation.Token, m_UserCancellation.Token, destroyCancellationToken);

                try
                {
                    // 完了済文字列イベントを再構築
                    messages = await UniTask.WhenAll(m_CompletedStringEvents.Select(x => x.GetString(cts.Token)));
                }
                catch
                {
                    return;
                }

                m_Text.text = messages.Aggregate((a, b) => a + b);

                LayoutRebuilder.ForceRebuildLayoutImmediate(m_Text.rectTransform);

                // テキスト位置調整
                var textPos = m_Text.rectTransform.anchoredPosition;
                textPos.y = Mathf.Max(0f, m_Text.rectTransform.rect.height - m_TextArea.rect.height);
                m_Text.rectTransform.anchoredPosition = textPos;

                // 言語切り替え完了
                m_IsChangingLocale = false;
            }

            // 未完了イベントがあるなら
            if (m_UncompletedEvents.Count > 0)
            {
                if (!m_IsWaitingLocaleChange)
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(m_LocaleCancellation.Token, m_UserCancellation.Token, destroyCancellationToken);

                    try
                    {
                        // 未完了イベントが待機状態になるまで待つ
                        await UniTask.WaitUntil(() => m_IsWaitingLocaleChange, cancellationToken: cts.Token);
                    }
                    catch
                    {
                        return;
                    }
                }

                // 待機解除
                m_IsWaitingLocaleChange = false;
            }
        }

        /// <summary>
        /// ウィンドウクリック時
        /// </summary>
        public void OnClickWindow()
        {
            onClick.OnNext(Unit.Default);
        }

        /// <summary>
        /// イベント実行
        /// </summary>
        public async UniTask RunEvent(IEvent messageEvent, CancellationToken cancellationToken = default)
        {
            await RunEvent(new[] { messageEvent }, cancellationToken);
        }

        /// <summary>
        /// イベント実行
        /// </summary>
        public async UniTask RunEvent(IEnumerable<IEvent> messageEvents, CancellationToken cancellationToken = default)
        {
            // 言語切り替え中？
            if (m_IsChangingLocale)
            {
                using var waitLocaleChangeCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, destroyCancellationToken);

                // 言語切り替え完了を待つ
                await UniTask.WaitUntil(() => !m_IsChangingLocale, cancellationToken: waitLocaleChangeCancellation.Token);
            }

            m_UserCancellation?.Cancel();
            m_UserCancellation?.Dispose();
            m_UserCancellation = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            m_UncompletedEvents.Clear();
            m_UncompletedEvents.AddRange(messageEvents);

            var localeCancelationToken = m_LocaleCancellation.Token;

            while (m_UncompletedEvents.Count > 0)
            {
                using var runCancellation = CancellationTokenSource.CreateLinkedTokenSource(m_LocaleCancellation.Token, m_UserCancellation.Token, destroyCancellationToken);

                try
                {
                    // イベント実行
                    await m_UncompletedEvents[0].Run(this, runCancellation.Token);
                }
                catch (Exception ex)
                {
                    // 言語切り替えによってキャンセルされた？
                    if (localeCancelationToken.IsCancellationRequested)
                    {
                        // 言語切り替え完了を待機する
                        m_IsWaitingLocaleChange = true;

                        using var waitLocaleChangeCancellation = CancellationTokenSource.CreateLinkedTokenSource(m_UserCancellation.Token, destroyCancellationToken);

                        await UniTask.WaitUntil(() => !m_IsWaitingLocaleChange, cancellationToken: waitLocaleChangeCancellation.Token);

                        // 言語切り替え完了したのでイベント再実行
                        continue;
                    }

                    throw ex;
                }

                if (m_UncompletedEvents[0] is IStringEvent stringEvent)
                {
                    // 完了した文字列イベントをリストに登録
                    m_CompletedStringEvents.Add(stringEvent);
                }

                // イベント完了
                m_UncompletedEvents.RemoveAt(0);
            }
        }

        /// <summary>
        /// メッセージ表示
        /// </summary>
        public async UniTask ShowMessage(string message, CancellationToken cancellationToken)
        {
            bool isScrolling = false;

            // テキスト高が表示高を超えたらスクロールさせる
            using var disposable = m_Text
                .OnRectTransformDimensionsChangeAsObservable()
                .ToUniTaskAsyncEnumerable()
                .SubscribeAwait(async x =>
                {
                    isScrolling = true;

                    float textHeight = m_Text.rectTransform.rect.height;
                    float areaHeight = m_TextArea.rect.height;
                    var textPos = m_Text.rectTransform.anchoredPosition;

                    if (textHeight - textPos.y > areaHeight)
                    {
                        float time = 0f;
                        var endTextPos = textPos;
                        endTextPos.y = textHeight - areaHeight;

                        while (time < m_ScrollDuration)
                        {
                            time += Time.deltaTime;

                            m_Text.rectTransform.anchoredPosition = Vector2.Lerp(textPos, endTextPos, time / m_ScrollDuration);

                            await UniTask.Yield(cancellationToken);
                        }
                    }

                    isScrolling = false;
                });

            int index = 0;

            while (index < message.Length)
            {
                m_Text.text += message[index];

                index++;

                if (m_Interval > 0f)
                {
                    await UniTask.Delay((int)(m_Interval * 1000), cancellationToken: cancellationToken);
                }

                if (isScrolling)
                {
                    // スクロール待ち
                    await UniTask.WaitUntil(() => !isScrolling, cancellationToken: cancellationToken);
                }
            }
        }

        /// <summary>
        /// クリア
        /// </summary>
        public void Clear()
        {
            m_Text.text = null;
            m_Text.rectTransform.anchoredPosition = Vector2.zero;

            m_IsChangingLocale = false;
            m_IsWaitingLocaleChange = false;

            m_UncompletedEvents.Clear();
            m_CompletedStringEvents.Clear();

            m_LocaleCancellation?.Cancel();
            m_LocaleCancellation?.Dispose();
            m_LocaleCancellation = new();

            m_UserCancellation?.Cancel();
            m_UserCancellation?.Dispose();
            m_UserCancellation = null;
        }
    }
}
