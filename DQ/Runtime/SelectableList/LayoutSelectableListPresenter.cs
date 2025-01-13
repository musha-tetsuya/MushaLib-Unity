using Cysharp.Threading.Tasks;
using MushaLib.UI.VirtualPad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// レイアウト付き選択可能リスト制御
    /// </summary>
    public class LayoutSelectableListPresenter : SelectableListPresenter
    {
        /// <summary>
        /// モデル
        /// </summary>
        private readonly LayoutSelectableListModel m_Model;

        /// <summary>
        /// ビュー
        /// </summary>
        private readonly LayoutSelectableListView m_View;

        /// <summary>
        /// 閉じるボタン押下時
        /// </summary>
        private Subject<Unit> m_OnClickCloseButton = new();

        /// <summary>
        /// 閉じるボタン購読の破棄
        /// </summary>
        private IDisposable m_OnClickCloseButtonDisposable;

        /// <summary>
        /// モデル
        /// </summary>
        public LayoutSelectableListModel Model => m_Model;

        /// <summary>
        /// ビュー
        /// </summary>
        public LayoutSelectableListView View => m_View;

        /// <summary>
        /// 閉じるボタン押下時
        /// </summary>
        public IObservable<Unit> OnClickCloseButton => m_OnClickCloseButton;

        /// <summary>
        /// construct
        /// </summary>
        public LayoutSelectableListPresenter(LayoutSelectableListModel model, LayoutSelectableListView view)
            : base(model, view)
        {
            m_Model = model;
            m_View = view;
        }

        /// <summary>
        /// 解放
        /// </summary>
        public override void Dispose()
        {
            m_OnClickCloseButtonDisposable?.Dispose();
            m_OnClickCloseButtonDisposable = null;

            base.Dispose();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public override async UniTask InitializeAsync(CancellationToken cancellationToken)
        {
            // 閉じるボタンの表示設定
            m_View.CloseButton.gameObject.SetActive(m_Model.UseCloseButton);

            // 閉じるボタン押下時
            m_OnClickCloseButtonDisposable?.Dispose();
            m_OnClickCloseButtonDisposable = m_View.CloseButton.OnClickAsObservable().Subscribe(m_OnClickCloseButton.OnNext);

            var tasks = new List<UniTask>();
            var viewSize = m_View.RectTransform.sizeDelta;

            if (m_Model.HeaderLocalizedString != null)
            {
                // ヘッダーのテキスト設定
                tasks.Add(m_View.HeaderLocalizeStringEvent.OnUpdateString.AsObservable().ToUniTask(true, cancellationToken));
                m_View.HeaderContent.SetActive(true);
                m_View.HeaderLocalizeStringEvent.StringReference = m_Model.HeaderLocalizedString;

                viewSize.y += (m_View.HeaderContent.transform as RectTransform).rect.height;
            }
            else
            {
                m_View.HeaderContent.SetActive(false);
                m_View.Content.anchoredPosition = Vector2.zero;
            }

            if (m_Model.ElementLocalizedStrings != null)
            {
                for (int i = 0; i < m_Model.ElementLocalizedStrings.Length; i++)
                {
                    // 要素の追加
                    var element = GameObject.Instantiate(m_View.ElementPrefab, m_View.Content);

                    // 要素のテキスト設定
                    tasks.Add(element.LocalizeStringEvent.OnUpdateString.AsObservable().ToUniTask(true, cancellationToken));
                    element.LocalizeStringEvent.StringReference = m_Model.ElementLocalizedStrings[i];
                }

                // レイアウト反映されるまでの1フレーム待機
                tasks.Add(UniTask.NextFrame(cancellationToken));
            }

            // レイアウトとテキストが反映されるまで待機
            await UniTask.WhenAll(tasks);

            // 初期化
            await base.InitializeAsync(cancellationToken);

            switch (m_View.ContentLayoutGroup)
            {
                case VerticalLayoutGroup:
                    viewSize.y += m_View.ContentLayoutGroup.preferredHeight;
                    break;

                case GridLayoutGroup:
                    viewSize.x = m_View.ContentLayoutGroup.preferredWidth;
                    viewSize.y += m_View.ContentLayoutGroup.preferredHeight;
                    break;
            }

            // Viewのサイズ調整
            m_View.RectTransform.sizeDelta = viewSize;
        }

        /// <summary>
        /// パッド操作時
        /// </summary>
        public override void OnPadPressed(ButtonType buttonType)
        {
            if (buttonType == ButtonType.B && m_Model.UseCloseButton && m_View.Interactable)
            {
                m_OnClickCloseButton.OnNext(Unit.Default);
                return;
            }

            base.OnPadPressed(buttonType);
        }
    }
}