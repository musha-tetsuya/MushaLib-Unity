using MushaLib.DQ.SelectableList;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MushaLib.DQ.TextInput
{
    /// <summary>
    /// テキスト入力画面
    /// </summary>
    public class TextInputView : SelectableListView
    {
        /// <summary>
        /// テキストテーブル設定
        /// </summary>
        public void SetTextTable(string[] textTable)
        {
            var elements = Content.GetComponentsInChildren<SelectableTextElement>(true).OrderBy(x => x.transform.GetSiblingIndex()).ToArray();
            bool active;

            for (int i = 0; i < elements.Length; i++)
            {
                if (i < textTable.Length && !string.IsNullOrEmpty(textTable[i]))
                {
                    active = true;
                    elements[i].TextMesh.text = textTable[i];
                }
                else
                {
                    active = false;
                }

                if (active != elements[i].gameObject.activeSelf)
                {
                    elements[i].gameObject.SetActive(active);
                }
            }
        }
    }
}
