using MushaLib.UI.DQ.SelectableList;
using MushaLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEngine;

namespace MushaLib.UI.DQ.TextInput
{
    /// <summary>
    /// テキスト選択処理の制御
    /// </summary>
    public class TextSelectionPresenter : IDisposable
    {
        /// <summary>
        /// モデル
        /// </summary>
        private readonly TextInputModel m_Model;

        /// <summary>
        /// ビュー
        /// </summary>
        private readonly ITextInputView m_View;

        /// <summary>
        /// テキスト選択処理の破棄
        /// </summary>
        private IDisposable m_SelectedDisposable;

        /// <summary>
        /// construct
        /// </summary>
        public TextSelectionPresenter(TextInputModel model, ITextInputView view)
        {
            m_Model = model;
            m_View = view;
            m_SelectedDisposable = m_View.OnSelected.Subscribe(OnSelected);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            m_SelectedDisposable?.Dispose();
            m_SelectedDisposable = null;
        }

        /// <summary>
        /// テキスト選択時
        /// </summary>
        private void OnSelected(SelectableTextElement element)
        {
            var elementIndex = element.transform.GetSiblingIndex();

            switch (elementIndex)
            {
                // 濁点
                case 67:
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
                case 69:
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

                // もどる
                case 75:
                    {
                        var currentText = m_Model.Text.Value ?? "";

                        if (currentText.Length > 0)
                        {
                            m_Model.UpdateText(currentText.Substring(0, currentText.Length - 1));
                        }
                    }
                    break;

                // すすむ
                case 77:
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
                        m_Model.InvokeOnSubmit();
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

                        m_Model.UpdateText(currentText + element.TextMesh.text);
                    }
                    break;
            }
        }
    }
}
