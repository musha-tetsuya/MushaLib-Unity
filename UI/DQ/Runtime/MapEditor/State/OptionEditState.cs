using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using UnityEditor;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// オプション編集ステート
    /// </summary>
    internal class OptionEditState : ValueStateBase<MapEditor>, IGUIState, IElementClickHandler
    {
        /// <summary>
        /// 数値テキスト
        /// </summary>
        private string m_NumberText = "0";

        /// <summary>
        /// 開始
        /// </summary>
        public override UniTask StartAsync(CancellationToken cancellationToken)
        {
            int[] optionData = null;

            switch (EditorUtility.DisplayDialogComplex("MapEditor", "オプションデータ編集", "New", "Continue", "Load"))
            {
                // New
                case 0:
                    {
                        optionData = Array.Empty<int>();
                    }
                    break;

                // Continue
                case 1:
                    {
                        optionData = null;
                    }
                    break;

                // Load
                case 2:
                    {
                        var path = EditorUtility.OpenFilePanel("編集するファイルの選択", Value.OptionDataDirectory, "json");

                        if (!string.IsNullOrEmpty(path))
                        {
                            try
                            {
                                optionData = JsonConvert.DeserializeObject<int[]>(File.ReadAllText(path));
                            }
                            catch (Exception ex)
                            {
                                Debug.LogException(ex);
                            }
                        }
                    }
                    break;
            }

            if (optionData != null)
            {
                for (int i = 0; i < Value.OptionData.Length; i++)
                {
                    Value.OptionData[i] = i < optionData.Length ? optionData[i] : 0;
                }
            }

            foreach (var element in Value.ScrollView.ScrollElements)
            {
                if (element is MapEditorElementView view)
                {
                    view.TextMesh.enabled = true;
                    view.TextMesh.text = Value.OptionData[element.Index].ToString();
                }
            }

            return UniTask.CompletedTask;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public override void Dispose()
        {
            foreach (var view in Value.ScrollView.ScrollElements.OfType<MapEditorElementView>())
            {
                view.TextMesh.enabled = false;
            }
        }

        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            m_NumberText = GUILayout.TextField(m_NumberText);

            if (GUILayout.Button("Back", GUILayout.ExpandWidth(false)))
            {
                StateManager.PopState();
            }

            if (GUILayout.Button("Save", GUILayout.ExpandWidth(false)))
            {
                var path = EditorUtility.SaveFilePanel("Save OptionData", Value.OptionDataDirectory, "", "json");

                if (!string.IsNullOrEmpty(path))
                {
                    File.WriteAllText(path, JsonConvert.SerializeObject(Value.OptionData));
                    AssetDatabase.Refresh();
                }
            }
        }

        /// <summary>
        /// 要素クリック時
        /// </summary>
        void IElementClickHandler.OnClickElement(MapEditorElementView view, int index)
        {
            if (!int.TryParse(m_NumberText, out var num))
            {
                Debug.LogWarning($"{m_NumberText} を数値に変換出来ません。");
                num = 0;
            }

            view.TextMesh.text = num.ToString();

            Value.OptionData[index] = num;
        }
    }
}
