using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.DQ.SelectableList
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
        /// クリック処理キャンセル
        /// </summary>
        private CompositeDisposable m_OnClickDisposable;

        /// <summary>
        /// パッド処理キャンセル
        /// </summary>
        private IDisposable m_PadDisposable;

        /// <summary>
        /// 要素選択決定時
        /// </summary>
        private Subject<SelectableElement> m_OnSelected = new();

        /// <summary>
        /// 要素リスト
        /// </summary>
        private SelectableElement[] m_Elements = Array.Empty<SelectableElement>();

        /// <summary>
        /// 現在選択中のインデックス
        /// </summary>
        private int m_CurrentIndex;

        /// <summary>
        /// コンテンツ
        /// </summary>
        public RectTransform Content => m_Content;

        /// <summary>
        /// 要素選択決定時
        /// </summary>
        public IObservable<SelectableElement> OnSelected => m_OnSelected;

        /// <summary>
        /// OnDestroy
        /// </summary>
        private void OnDestroy()
        {
            m_OnClickDisposable?.Dispose();
            m_OnClickDisposable = null;

            m_PadDisposable?.Dispose();
            m_PadDisposable = null;

            m_OnSelected.Dispose();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (m_Content == null)
            {
                Debug.LogError($"{GetType()}: m_Content is null.");
                return;
            }

            // LayoutGroupが付いているなら、LayoutGroupの設定に合わせてセル数と軸を決める
            if (m_Content.TryGetComponent<LayoutGroup>(out var layoutGroup))
            {
                m_Elements = m_Content.GetComponentsInChildren<SelectableElement>().OrderBy(x => x.transform.GetSiblingIndex()).ToArray();

                if (layoutGroup is GridLayoutGroup gridLayoutGroup)
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

                            m_CellCount.x = Mathf.Min(m_Elements.Length, m_CellCount.x);
                            m_CellCount.y = Mathf.CeilToInt((float)m_Elements.Length / m_CellCount.x);
                        }
                        else
                        {
                            m_CellCount.y = 1;

                            var remainHeight = m_Content.rect.height - (gridLayoutGroup.padding.top + gridLayoutGroup.cellSize.y + gridLayoutGroup.padding.bottom);
                            if (remainHeight > 0f)
                            {
                                m_CellCount.y += (int)(remainHeight / (gridLayoutGroup.cellSize.y + gridLayoutGroup.spacing.y));
                            }

                            m_CellCount.y = Mathf.Min(m_Elements.Length, m_CellCount.y);
                            m_CellCount.x = Mathf.CeilToInt((float)m_Elements.Length / m_CellCount.y);
                        }
                    }
                    else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedColumnCount)
                    {
                        m_CellCount.x = Mathf.Min(m_Elements.Length, gridLayoutGroup.constraintCount);
                        m_CellCount.y = Mathf.CeilToInt((float)m_Elements.Length / m_CellCount.x);
                    }
                    else if (gridLayoutGroup.constraint == GridLayoutGroup.Constraint.FixedRowCount)
                    {
                        m_CellCount.y = Mathf.Min(m_Elements.Length, gridLayoutGroup.constraintCount);
                        m_CellCount.x = Mathf.CeilToInt((float)m_Elements.Length / m_CellCount.y);
                    }

                    m_StartAxis = gridLayoutGroup.startAxis;
                }
                else if (layoutGroup is HorizontalLayoutGroup)
                {
                    m_CellCount.x = m_Elements.Length;
                    m_CellCount.y = 1;

                    m_StartAxis = GridLayoutGroup.Axis.Horizontal;
                }
                else if (layoutGroup is VerticalLayoutGroup)
                {
                    m_CellCount.x = 1;
                    m_CellCount.y = m_Elements.Length;

                    m_StartAxis = GridLayoutGroup.Axis.Vertical;
                }
            }
            else
            {
                m_Elements = m_Content.GetComponentsInChildren<SelectableElement>(true).OrderBy(x => x.transform.GetSiblingIndex()).ToArray();

                m_CellCount.x = Mathf.Min(m_Elements.Length, Mathf.Max(m_CellCount.x, 1));
                m_CellCount.y = Mathf.Min(m_Elements.Length, Mathf.Max(m_CellCount.y, 1));
            }

            // 要素クリック時処理の登録
            m_OnClickDisposable?.Dispose();
            m_OnClickDisposable = new CompositeDisposable();

            for (int i = 0; i < m_Elements.Length; i++)
            {
                int index = i;

                m_Elements[i].Button
                    .OnClickAsObservable()
                    .Subscribe(_ => OnClickElement(index))
                    .AddTo(m_OnClickDisposable);
            }
        }

        /// <summary>
        /// 選択インデックスの変更
        /// </summary>
        public void SetCurrentIndex(int index, bool force = false)
        {
            index = (int)Mathf.Repeat(index, m_Elements.Length);

            if (index != m_CurrentIndex || force)
            {
                // 選択中要素の矢印を非表示に
                GetElement(m_CurrentIndex)?.Arrow.SetAnimationType(Arrow.AnimationType.Hide);

                // インデックス変更
                m_CurrentIndex = index;

                // 新しく選択した要素の矢印を点滅表示
                GetElement(m_CurrentIndex)?.Arrow.SetAnimationType(Arrow.AnimationType.Blink);
            }
        }

        /// <summary>
        /// パッド操作時
        /// </summary>
        public virtual void OnPadPressed(SelectableListButtonType buttonType)
        {
            if (m_CanvasGroup.interactable)
            {
                switch (buttonType)
                {
                    case SelectableListButtonType.Up:
                        MoveCurrentIndex(0, -1);
                        break;

                    case SelectableListButtonType.Down:
                        MoveCurrentIndex(0, 1);
                        break;

                    case SelectableListButtonType.Left:
                        MoveCurrentIndex(-1, 0);
                        break;

                    case SelectableListButtonType.Right:
                        MoveCurrentIndex(1, 0);
                        break;

                    case SelectableListButtonType.Submit:
                        m_OnSelected.OnNext(GetElement(m_CurrentIndex));
                        break;
                }
            }
        }

        /// <summary>
        /// 要素取得
        /// </summary>
        private SelectableElement GetElement(int index)
        {
            if (0 <= index && index < m_Elements.Length)
            {
                return m_Elements[index];
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// 選択インデックスの移動
        /// </summary>
        private void MoveCurrentIndex(int moveX, int moveY)
        {
            var pos = Vector2Int.zero;
            var delta = Vector2Int.zero;

            if (m_StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                if (m_CellCount.x > 0)
                {
                    pos.x = m_CurrentIndex % m_CellCount.x;
                    pos.y = m_CurrentIndex / m_CellCount.x;

                    delta.x = 1;
                    delta.y = m_CellCount.x;
                }
            }
            else
            {
                if (m_CellCount.y > 0)
                {
                    pos.x = m_CurrentIndex / m_CellCount.y;
                    pos.y = m_CurrentIndex % m_CellCount.y;

                    delta.x = m_CellCount.y;
                    delta.y = 1;
                }
            }

            while (true)
            {
                pos.x = (int)Mathf.Repeat(pos.x + moveX, m_CellCount.x);
                pos.y = (int)Mathf.Repeat(pos.y + moveY, m_CellCount.y);

                var nextIndex = pos.x * delta.x + pos.y * delta.y;
                if (nextIndex == m_CurrentIndex)
                {
                    break;
                }

                var nextElement = GetElement(nextIndex);
                if (nextElement != null && nextElement.gameObject.activeInHierarchy)
                {
                    SetCurrentIndex(nextIndex);
                    break;
                }
            }
        }

        /// <summary>
        /// クリック時
        /// </summary>
        private void OnClickElement(int index)
        {
            if (index != m_CurrentIndex)
            {
                // 選択インデックスを変更
                SetCurrentIndex(index);
            }
            else
            {
                // 選択決定を通知
                m_OnSelected.OnNext(GetElement(index));
            }
        }

        /// <summary>
        /// 選択決定
        /// </summary>
        public void Select()
        {
            var element = GetElement(m_CurrentIndex);
            if (element != null)
            {
                // 選択中要素の矢印の点滅を解除
                element.Arrow.SetAnimationType(Arrow.AnimationType.Show);

                // リストに触れなくする
                m_CanvasGroup.interactable = false;
            }
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            var element = GetElement(m_CurrentIndex);
            if (element != null)
            {
                // 選択中要素の矢印を点滅表示
                element.Arrow.SetAnimationType(Arrow.AnimationType.Blink);

                // リストに触れるようにする
                m_CanvasGroup.interactable = true;
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
            /// LayoutGroup
            /// </summary>
            private LayoutGroup m_LayoutGroup;

            /// <summary>
            /// ターゲット
            /// </summary>
            private SelectableListView Target => m_Target ??= target as SelectableListView;

            /// <summary>
            /// OnEnable
            /// </summary>
            protected virtual void OnEnable()
            {
                if (Target.Content != null)
                {
                    m_LayoutGroup = Target.Content.GetComponent<LayoutGroup>();
                }
            }

            /// <summary>
            /// OnInspectorGUI
            /// </summary>
            public override void OnInspectorGUI()
            {
                var content = Target.Content;

                serializedObject.Update();

                EditorGUI.BeginDisabledGroup(true);
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
                EditorGUI.EndDisabledGroup();

                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CanvasGroup"));
                EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Content"));

                if (m_LayoutGroup == null)
                {
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_CellCount"));
                    EditorGUILayout.PropertyField(serializedObject.FindProperty("m_StartAxis"));
                }

                serializedObject.ApplyModifiedProperties();

                if (Target.Content != content)
                {
                    if (Target.Content != null)
                    { 
                        m_LayoutGroup = Target.Content.GetComponent<LayoutGroup>();
                    }
                    else
                    {
                        m_LayoutGroup = null;
                    }
                }
            }
        }
#endif
    }
}
