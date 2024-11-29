using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using TMPro;
using UniRx;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

namespace MushaLib.DQ.MessageWindow
{
    /// <summary>
    /// メッセージウィンドウ
    /// </summary>
    public class MessageWindowView : MonoBehaviour
    {
        /// <summary>
        /// ウィンドウボタン
        /// </summary>
        [SerializeField]
        private Button m_WindowButton;

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
        /// クリック待ちイベント提供
        /// </summary>
        [SerializeField]
        private Events.MessageWindowEventProvider m_WaitClickEventProvider;

        /// <summary>
        /// 言語切り替え待機中かどうか
        /// </summary>
        private bool m_IsWaitingLocaleChange;

        /// <summary>
        /// 一行の高さ
        /// </summary>
        private float m_SingleLineHeight;

        /// <summary>
        /// 未完了イベント
        /// </summary>
        private List<Events.IMessageWindowEvent> m_UncompletedEvents = new();

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
        public IObservable<Unit> OnClick => m_WindowButton.OnClickAsObservable();

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

            // 一行の高さ取得
            m_SingleLineHeight = m_Text.GetPreferredValues("\n", m_TextArea.rect.width, m_TextArea.rect.height).y;
        }

        /// <summary>
        /// 言語切り替え時
        /// </summary>
        private void OnSelectedLocaleChanged(Locale locale)
        {
            m_LocaleCancellation?.Cancel();
            m_LocaleCancellation?.Dispose();
            m_LocaleCancellation = new();

            UniTask.Void(async () =>
            {
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
            });
        }

        /// <summary>
        /// イベント実行
        /// </summary>
        public async UniTask RunEvent(Events.IMessageWindowEvent messageEvent, CancellationToken cancellationToken = default)
        {
            await RunEvent(new[] { messageEvent }, cancellationToken);
        }

        /// <summary>
        /// イベント実行
        /// </summary>
        public async UniTask RunEvent(IEnumerable<Events.IMessageWindowEvent> messageEvents, CancellationToken cancellationToken = default)
        {
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
                catch
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

                    throw;
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
            // 現在のテキスト高
            var textHeight = m_Text.GetPreferredValues().y;

            // 現在のテキストがエリア外に出るまで一行ずつスクロール
            while (Mathf.Abs(textHeight - m_Text.rectTransform.anchoredPosition.y) >= 1f)
            {
                var startPos = m_Text.rectTransform.anchoredPosition;
                var endPos = startPos + Vector2.up * m_SingleLineHeight;
                var time = 0f;
                    
                while (time < m_ScrollDuration)
                {
                    m_Text.rectTransform.anchoredPosition = Vector2.Lerp(startPos, endPos, time / m_ScrollDuration);

                    time += Time.deltaTime;

                    await UniTask.NextFrame(cancellationToken);
                }

                m_Text.rectTransform.anchoredPosition = endPos;
            }

            // テキストクリア
            m_Text.text = null;
            m_Text.rectTransform.anchoredPosition = Vector2.zero;

            var sb = new StringBuilder(message.Length);

            for (int i = 0; i < message.Length; i++)
            {
                // 一文字追加
                sb.Append(message[i]);

                // 一文字追加した場合のテキスト高
                textHeight = m_Text.GetPreferredValues(sb.ToString(), m_TextArea.rect.width, m_TextArea.rect.height).y;

                // エリアからはみ出しそう
                if (textHeight > m_TextArea.rect.height)
                {
                    // クリック待ち
                    await (m_WaitClickEventProvider?.GetEvent() ?? new Events.WaitClickEvent()).Run(this, cancellationToken);

                    // 続きからメッセージ表示
                    await ShowMessage(message[i..], cancellationToken);
                    break;
                }

                // テキスト更新
                m_Text.text = sb.ToString();

                if (m_Interval > 0f)
                {
                    // 次の文字表示まで少し待機
                    await UniTask.Delay((int)(m_Interval * 1000), cancellationToken: cancellationToken);
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

            m_IsWaitingLocaleChange = false;

            m_UncompletedEvents.Clear();

            m_LocaleCancellation?.Cancel();
            m_LocaleCancellation?.Dispose();
            m_LocaleCancellation = new();

            m_UserCancellation?.Cancel();
            m_UserCancellation?.Dispose();
            m_UserCancellation = null;
        }
    }
}
