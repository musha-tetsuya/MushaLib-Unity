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

namespace MushaLib
{
    /// <summary>
    /// ローカライズ文字列構築機能
    /// </summary>
    /// <remarks>
    /// 使用方法：
    /// 1. UpdateStringにTextMeshProUGUI.textを設定
    /// 2. Addメソッドで文字列やLocalizedStringを追加
    /// 3. Buildメソッドでテキストを構築
    /// </remarks>
    public class LocalizeStringBuilder : MonoBehaviour
    {
        /// <summary>
        /// 文字列更新時イベント
        /// </summary>
        [SerializeField]
        private UnityEventString m_UpdateString;

        /// <summary>
        /// 文字列
        /// </summary>
        private string m_String;

        /// <summary>
        /// 文字列構築タスクリスト
        /// </summary>
        private List<Func<UniTask<string>>> m_StringBuilders = new();

        /// <summary>
        /// 構築済みカウント
        /// </summary>
        private int m_BuildCount;

        /// <summary>
        /// 文字列構築キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_BuildCancellation;

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

            m_BuildCancellation?.Cancel();
            m_BuildCancellation?.Dispose();
            m_BuildCancellation = null;
        }

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            LocalizationSettings.SelectedLocaleChanged += OnSelectedLocaleChanged;
        }

        /// <summary>
        /// 言語変更時
        /// </summary>
        private void OnSelectedLocaleChanged(Locale locale)
        {
            m_String = null;
            m_BuildCount = 0;

            Build().Forget();
        }

        /// <summary>
        /// テキスト構築
        /// </summary>
        public async UniTask Build()
        {
            if (m_BuildCount < m_StringBuilders.Count)
            {
                m_BuildCancellation?.Cancel();
                m_BuildCancellation?.Dispose();
                m_BuildCancellation = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

                var msgs = await UniTask.WhenAll(m_StringBuilders.Skip(m_BuildCount).Select(x => x.Invoke())).AttachExternalCancellation(m_BuildCancellation.Token);

                m_String += msgs.Aggregate((a, b) => a + b);

                m_BuildCount = m_StringBuilders.Count;

                m_UpdateString?.Invoke(m_String);
            }
        }

        /// <summary>
        /// テキスト追加
        /// </summary>
        public void Add(Func<UniTask<string>> builder)
        {
            m_StringBuilders.Add(builder);
        }

        /// <summary>
        /// テキスト追加
        /// </summary>
        public void Add(string msg, CancellationToken cancellationToken = default)
        {
            m_StringBuilders.Add(() => UniTask.FromResult(msg).AttachExternalCancellation(cancellationToken));
        }

        /// <summary>
        /// テキスト追加
        /// </summary>
        public void Add(LocalizedString localizedString, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrEmpty(localizedString.TableReference))
            {
                // テーブルが設定されていない場合はデフォルトテーブルを使用する
                localizedString.TableReference = LocalizationSettings.StringDatabase.DefaultTable;
            }

            m_StringBuilders.Add(() => localizedString.GetLocalizedStringAsync().ToUniTask(cancellationToken: cancellationToken));
        }

        /// <summary>
        /// テキストクリア
        /// </summary>
        public void Clear()
        {
            m_BuildCancellation?.Cancel();
            m_BuildCancellation?.Dispose();
            m_BuildCancellation = null;

            m_String = null;
            m_StringBuilders.Clear();
            m_BuildCount = 0;

            m_UpdateString?.Invoke(m_String);
        }
    }
}
