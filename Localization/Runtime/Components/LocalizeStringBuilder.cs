using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Events;
using UnityEngine.Localization.Settings;

namespace MushaLib.Localization.Components
{
    /// <summary>
    /// ローカライズ文字列構築機能
    /// </summary>
    public class LocalizeStringBuilder : MonoBehaviour
    {
        /// <summary>
        /// 文字列更新時イベント
        /// </summary>
        [SerializeField]
        private UnityEventString m_UpdateString;

        /// <summary>
        /// 文字列構築処理
        /// </summary>
        private Func<UniTask<string>> m_StringBuilder;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource;

        /// <summary>
        /// 文字列更新時イベント
        /// </summary>
        public UnityEventString OnUpdateString
        {
            get => m_UpdateString;
            set => m_UpdateString = value;
        }

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            LocalizationSettings.SelectedLocaleChanged -= OnSelectedLocaleChanged;
        }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            BuildString().Forget();
        }

        /// <summary>
        /// 言語変更時
        /// </summary>
        private void OnSelectedLocaleChanged(Locale locale)
        {
            BuildString().Forget();
        }

        /// <summary>
        /// テキスト構築
        /// </summary>
        private async UniTask BuildString()
        {
            if (m_StringBuilder != null)
            {
                string text;

                try
                {
                    text = await m_StringBuilder.Invoke().AttachExternalCancellation(m_CancellationTokenSource.Token);
                }
                catch
                {
                    return;
                }

                m_UpdateString?.Invoke(text);
            }
        }

        /// <summary>
        /// 文字列構築処理の登録
        /// </summary>
        public void SetStringBuilder(Func<UniTask<string>> builder, CancellationToken cancellationToken = default)
        {
            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellationToken);

            m_StringBuilder = builder;
        }

        /// <summary>
        /// 文字列構築処理の登録
        /// </summary>
        public void SetStringBuilder(LocalizedString localizedString, CancellationToken cancellationToken = default)
        {
            SetStringBuilder(new[] { localizedString }, cancellationToken);
        }

        /// <summary>
        /// 文字列構築処理の登録
        /// </summary>
        public void SetStringBuilder(IEnumerable<LocalizedString> localizedStrings, CancellationToken cancellationToken = default)
        {
            foreach (var x in localizedStrings)
            {
                if (string.IsNullOrEmpty(x.TableReference))
                {
                    // テーブルが設定されていない場合はデフォルトテーブルを使用する
                    x.TableReference = LocalizationSettings.StringDatabase.DefaultTable;
                }
            }

            SetStringBuilder(async () =>
            {
                var msgs = await UniTask.WhenAll(localizedStrings.Select(x => x.GetLocalizedStringAsync().ToUniTask()));

                return msgs.Select((msg, i) => msg + (localizedStrings.ElementAt(i) as ConcatableLocalizedString)?.ConcatenatingCharacter).Aggregate((a, b) => a + b);

            }, cancellationToken);
        }
    }
}
