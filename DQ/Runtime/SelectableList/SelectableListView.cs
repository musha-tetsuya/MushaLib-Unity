using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reflection;
using System.Threading;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// 選択可能リスト
    /// </summary>
    public class SelectableListView : MonoBehaviour
    {
        /// <summary>
        /// CanvasGroup
        /// </summary>
        [SerializeField]
        private CanvasGroup m_CanvasGroup;

        /// <summary>
        /// コンテンツ
        /// </summary>
        [SerializeField]
        private RectTransform m_Content;

        /// <summary>
        /// コンテンツのレイアウトグループ
        /// </summary>
        [SerializeField]
        private LayoutGroup m_ContentLayoutGroup;

        /// <summary>
        /// セル数
        /// </summary>
        [SerializeField]
        private Vector2Int m_CellCount = Vector2Int.one;

        /// <summary>
        /// 軸
        /// </summary>
        [SerializeField]
        private GridLayoutGroup.Axis m_StartAxis;

        /// <summary>
        /// 要素クリック購読のキャンセル
        /// </summary>
        private CancellationTokenSource m_OnClickCancellation;

        /// <summary>
        /// 要素クリック時
        /// </summary>
        private Subject<SelectableElement> m_OnClick = new();

        /// <summary>
        /// 操作可不可
        /// </summary>
        public bool Interactable
        {
            get => m_CanvasGroup.interactable;
            set => m_CanvasGroup.interactable = value;
        }

        /// <summary>
        /// キャンバスグループ
        /// </summary>
        public CanvasGroup CanvasGroup => m_CanvasGroup;

        /// <summary>
        /// コンテンツ
        /// </summary>
        public RectTransform Content => m_Content;

        /// <summary>
        /// コンテンツのレイアウトグループ
        /// </summary>
        public LayoutGroup ContentLayoutGroup => m_ContentLayoutGroup;

        /// <summary>
        /// セル数
        /// </summary>
        public Vector2Int CellCount => m_CellCount;

        /// <summary>
        /// 軸
        /// </summary>
        public GridLayoutGroup.Axis StartAxis => m_StartAxis;

        /// <summary>
        /// 要素リスト
        /// </summary>
        public ReadOnlyCollection<SelectableElement> Elements { get; private set; }

        /// <summary>
        /// 要素クリック時
        /// </summary>
        public IObservable<SelectableElement> OnClick => m_OnClick;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            m_OnClickCancellation?.Cancel();
            m_OnClickCancellation?.Dispose();
            m_OnClickCancellation = null;
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Initialize()
        {
            // LayoutGroupが付いているなら、LayoutGroupの設定に合わせてセル数と軸を決める
            if (m_ContentLayoutGroup != null)
            {
                Elements = Array.AsReadOnly(m_Content.GetComponentsInChildren<SelectableElement>().OrderBy(x => x.transform.GetSiblingIndex()).ToArray());

                if (m_ContentLayoutGroup is GridLayoutGroup gridLayoutGroup)
                {
                    if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.Flexible)
                    {
                        if (gridLayoutGroup.startAxis == GridLayoutGroup.Axis.Horizontal)
                        {
                            m_CellCount.x = 1;

                            var remainWidth = m_Content.rect.width - (gridLayoutGroup.padding.left + gridLayoutGroup.cellSize.x + gridLayoutGroup.padding.right);
                            if (remainWidth > 0f)
                            {
                                m_CellCount.x += (int)(remainWidth / (gridLayoutGroup.cellSize.x + gridLayoutGroup.spacing.x));
                            }

                            m_CellCount.x = Mathf.Min(Elements.Count, m_CellCount.x);
                            m_CellCount.y = Mathf.CeilToInt((float)Elements.Count / m_CellCount.x);
                        }
                        else
                        {
                            m_CellCount.y = 1;

                            var remainHeight = m_Content.rect.height - (gridLayoutGroup.padding.top + gridLayoutGroup.cellSize.y + gridLayoutGroup.padding.bottom);
                            if (remainHeight > 0f)
                            {
                                m_CellCount.y += (int)(remainHeight / (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
                            }

                            m_CellCount.y = Mathf.Min(Elements.Count, m_CellCount.y);
                            m_CellCount.x = Mathf.CeilToInt((float)Elements.Count / m_CellCount.y);
                        }
                    }
                    else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                    {
                        m_CellCount.x = Mathf.Min(Elements.Count, gridLayoutGroup.constraintCount);
                        m_CellCount.y = Mathf.CeilToInt((float)Elements.Count / m_CellCount.x);
                    }
                    else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                    {
                        m_CellCount.y = Mathf.Min(Elements.Count, gridLayoutGroup.constraintCount);
                        m_CellCount.x = Mathf.CeilToInt((float)Elements.Count / m_CellCount.y);
                    }

                    m_StartAxis = gridLayoutGroup.startAxis;
                }
                else if (m_ContentLayoutGroup is HorizontalLayoutGroup)
                {
                    m_CellCount.x = Elements.Count;
                    m_CellCount.y = 1;

                    m_StartAxis = GridLayoutGroup.Axis.Horizontal;
                }
                else if (m_ContentLayoutGroup is VerticalLayoutGroup)
                {
                    m_CellCount.x = 1;
                    m_CellCount.y = Elements.Count;

                    m_StartAxis = GridLayoutGroup.Axis.Vertical;
                }
            }
            else
            {
                Elements = Array.AsReadOnly(m_Content.GetComponentsInChildren<SelectableElement>(true).OrderBy(x => x.transform.GetSiblingIndex()).ToArray());

                m_CellCount.x = Mathf.Min(Elements.Count, Mathf.Max(m_CellCount.x, 1));
                m_CellCount.y = Mathf.Min(Elements.Count, Mathf.Max(m_CellCount.y, 1));
            }

            // 要素クリック時処理の登録
            m_OnClickCancellation?.Cancel();
            m_OnClickCancellation?.Dispose();
            m_OnClickCancellation = new CancellationTokenSource();

            for (int i = 0; i < Elements.Count; i++)
            {
                int index = i;

                Elements[i].Button
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        m_OnClick.OnNext(Elements[index]);
                    })
                    .AddTo(m_OnClickCancellation.Token);
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// カスタムインスペクター
        /// </summary>
        [CustomEditor(typeof(SelectableListView), true)]
        protected class SelectableListViewInspector : Editor
        {
            /// <summary>
            /// ターゲット
            /// </summary>
            private SelectableListView m_Target;

            /// <summary>
            /// 変数名リスト
            /// </summary>
            private List<string> m_FieldNames = new();

            /// <summary>
            /// ターゲット
            /// </summary>
            private SelectableListView Target => m_Target ??= target as SelectableListView;

            /// <summary>
            /// OnEnable
            /// </summary>
            protected virtual void OnEnable()
            {
                var currentType = target.GetType();

                while (currentType != null)
                {
                    m_FieldNames.AddRange(currentType
                        .GetFields(BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
                        .Where(x => x.IsDefined(typeof(SerializeField), false))
                        .Where(x => !x.IsDefined(typeof(HideInInspector), false))
                        .Select(x => x.Name));

                    currentType = currentType.BaseType;
                }

                m_FieldNames.Remove("m_CellCount");
                m_FieldNames.Remove("m_StartAxis");
            }

            /// <summary>
            /// OnInspectorGUI
            /// </summary>
            public override void OnInspectorGUI()
            {
                serializedObject.Update();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                EditorGUI.EndDisabledGroup();

                foreach (var fieldName in m_FieldNames)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty(fieldName));
                }

                if (Target.m_ContentLayoutGroup == null)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CellCount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_StartAxis"));
                }

                serializedObject.ApplyModifiedProperties();
            }
        }
#endif
    }
}
