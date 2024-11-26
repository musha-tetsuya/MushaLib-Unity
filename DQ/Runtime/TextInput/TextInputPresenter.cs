using MushaLib.DQ.SelectableList;
using MushaLib.UI.VirtualPad;
using MushaLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.TextInput
{
    /// <summary>
    /// テキスト入力の制御
    /// </summary>
    public class TextInputPresenter : SelectableListPresenter
    {
        /// <summary>
        /// モデル
        /// </summary>
        private readonly TextInputModel m_Model;

        /// <summary>
        /// ビュー
        /// </summary>
        private readonly TextInputView m_View;

        /// <summary>
        /// 決定時
        /// </summary>
        private Subject<Unit> m_OnSubmit = new();

        /// <summary>
        /// 決定時
        /// </summary>
        public IObservable<Unit> OnSubmit => m_OnSubmit;

        /// <summary>
        /// construct
        /// </summary>
        public TextInputPresenter(TextInputModel model, TextInputView view)
            : base(model, view)
        {
            m_Model = model;
            m_View = view;
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public override void Dispose()
        {
            base.Dispose();

            m_OnSubmit.Dispose();
        }

        /// <summary>
        /// 開始
        /// </summary>
        public override void Start(CancellationToken cancellationToken = default)
        {
            m_View.SetTextTable(m_Model.TextTableProvider.GetTextTable());

            base.Start(cancellationToken);
        }

        /// <summary>
        /// パッド操作時
        /// </summary>
        public override void OnPadPressed(ButtonType buttonType)
        {
            base.OnPadPressed(buttonType);

            if (m_View.Interactable)
            {
                switch (buttonType)
                {
                    case ButtonType.B:
                        {
                            var currentText = m_Model.Text.Value ?? "";

                            if (currentText.Length > 0)
                            {
                                m_Model.UpdateText(currentText.Substring(0, currentText.Length - 1));
                            }
                        }
                        break;
                }
            }
        }

        /// <summary>
        /// テキスト選択時
        /// </summary>
        protected override void InvokeOnSelected()
        {
            var element = m_View.Elements[m_Model.CurrentIndex];
            var elementIndex = element.transform.GetSiblingIndex();

            switch (elementIndex)
            {
                // ひらがな or カタカナ
                case 71:
                    {
                        m_Model.TextTableProvider.CurrentMode = m_Model.TextTableProvider.CurrentMode == TextInputMode.Hiragana ? TextInputMode.Katakana : TextInputMode.Hiragana;
                        m_View.SetTextTable(m_Model.TextTableProvider.GetTextTable());
                        m_View.Initialize();
                    }
                    break;

                // カタカナ or アルファベット
                case 73:
                    {
                        m_Model.TextTableProvider.CurrentMode = m_Model.TextTableProvider.CurrentMode == TextInputMode.Alphabet ? TextInputMode.Katakana : TextInputMode.Alphabet;
                        m_View.SetTextTable(m_Model.TextTableProvider.GetTextTable());
                        m_View.Initialize();
                    }
                    break;

                // 濁点
                case 75:
                    {
                        if (m_Model.TextTableProvider.CurrentMode != TextInputMode.Alphabet)
                        {
                            var currentText = m_Model.Text.Value ?? "";

                            if (currentText.Length > 0 && KanaConverter.DakutenTable.TryGetValue(currentText.Last().ToString(), out var dakuten))
                            {
                                m_Model.UpdateText(currentText.Substring(0, currentText.Length - 1) + dakuten);
                            }
                        }
                    }
                    break;

                // 半濁点
                case 76:
                    {
                        if (m_Model.TextTableProvider.CurrentMode != TextInputMode.Alphabet)
                        {
                            var currentText = m_Model.Text.Value ?? "";

                            if (currentText.Length > 0 && KanaConverter.HandakutenTable.TryGetValue(currentText.Last().ToString(), out var handakuten))
                            {
                                m_Model.UpdateText(currentText.Substring(0, currentText.Length - 1) + handakuten);
                            }
                        }
                    }
                    break;

                // もどる
                case 77:
                    {
                        var currentText = m_Model.Text.Value ?? "";

                        if (currentText.Length > 0)
                        {
                            m_Model.UpdateText(currentText.Substring(0, currentText.Length - 1));
                        }
                    }
                    break;

                // すすむ
                case 78:
                    {
                        var currentText = m_Model.Text.Value ?? "";

                        if (currentText.Length == m_Model.MaxLength)
                        {
                            currentText = currentText.Substring(0, currentText.Length - 1);
                        }

                        m_Model.UpdateText(currentText + "　");
                    }
                    break;

                // OK
                case 79:
                    {
                        m_OnSubmit.OnNext(Unit.Default);
                    }
                    break;

                // テキスト
                default:
                    {
                        var currentText = m_Model.Text.Value ?? "";

                        if (currentText.Length == m_Model.MaxLength)
                        {
                            currentText = currentText.Substring(0, currentText.Length - 1);
                        }

                        m_Model.UpdateText(currentText + (element as SelectableTextElement).TextMesh.text);
                    }
                    break;
            }
        }
    }
}
