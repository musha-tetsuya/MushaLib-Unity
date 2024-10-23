using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.DQ
{
    /// <summary>
    /// 選択可能リスト制御
    /// </summary>
    public class SelectableListPresenter<TSelectableList> : IDisposable where TSelectableList : ISelectableList
    {
        /// <summary>
        /// 選択可能リストのビュー
        /// </summary>
        private TSelectableList m_View;

        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource;

        /// <summary>
        /// 選択中インデックス
        /// </summary>
        private int m_CurrentIndex;

        /// <summary>
        /// インデックス変化量
        /// </summary>
        private Vector2Int m_IndexDelta;

        /// <summary>
        /// 選択決定時
        /// </summary>
        private Subject<int> m_OnSelected = new Subject<int>();

        /// <summary>
        /// 選択決定時
        /// </summary>
        public IObservable<int> OnSelected => m_OnSelected;

        /// <summary>
        /// construct
        /// </summary>
        public SelectableListPresenter(TSelectableList view)
        {
            m_View = view;
            m_CurrentIndex = -1;

            if (view.Axis == GridLayoutGroup.Axis.Horizontal)
            {
                m_IndexDelta.x = 1;
                m_IndexDelta.y = view.CellCount.x;
            }
            else
            {
                m_IndexDelta.x = view.CellCount.y;
                m_IndexDelta.y = 1;
            }
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = null;

            m_OnSelected.Dispose();
        }

        /// <summary>
        /// 開始
        /// </summary>
        public void Start(int index = 0, CancellationToken cancellationToken = default)
        {
            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken);

            foreach (var x in m_View.GetElements().Select((element, i) => (element, i)))
            {
                var (element, i) = x;

                // 要素クリック時
                element.Button
                    .OnClickAsObservable()
                    .Subscribe(_ =>
                    {
                        OnClickElement(i);
                    })
                    .AddTo(m_CancellationTokenSource.Token);
            }

            // 開始時インデックスをセット
            SetCurrentIndex(index);
        }

        /// <summary>
        /// 要素クリック時
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
                m_OnSelected.OnNext(index);
            }
        }

        /// <summary>
        /// 選択インデックスの変更
        /// </summary>
        private void SetCurrentIndex(int index)
        {
            index = (int)Mathf.Repeat(index, m_View.Count);

            if (index != m_CurrentIndex)
            {
                // 選択中要素の矢印を非表示に
                m_View.GetElement(m_CurrentIndex)?.Arrow.SetAnimationType(Arrow.AnimationType.Hide);

                // インデックス変更
                m_CurrentIndex = index;

                // 新しく選択した要素の矢印を点滅表示
                m_View.GetElement(m_CurrentIndex)?.Arrow.SetAnimationType(Arrow.AnimationType.Blink);
            }
        }

        /// <summary>
        /// 選択決定
        /// </summary>
        public void Select()
        {
            var element = m_View.GetElement(m_CurrentIndex);
            if (element != null)
            {
                // 選択中要素の矢印の点滅を解除
                element.Arrow.SetAnimationType(Arrow.AnimationType.Show);

                // リストに触れなくする
                m_View.SetInteractable(false);
            }
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            var element = m_View.GetElement(m_CurrentIndex);
            if (element != null)
            {
                // 選択中要素の矢印を点滅表示
                element.Arrow.SetAnimationType(Arrow.AnimationType.Blink);

                // リストに触れるようにする
                m_View.SetInteractable(true);
            }
        }

        /// <summary>
        /// パッド操作時
        /// </summary>
        public void OnPadPressed(SelectableListButtonType buttonType)
        {
            int x, y;

            if (m_View.Axis == GridLayoutGroup.Axis.Horizontal)
            {
                x = m_CurrentIndex % m_IndexDelta.y;
                y = m_CurrentIndex / m_IndexDelta.x;
            }
            else
            {
                x = m_CurrentIndex / m_IndexDelta.y;
                y = m_CurrentIndex % m_IndexDelta.x;
            }

            switch (buttonType)
            {
                case SelectableListButtonType.Up:
                    SetCurrentIndex(m_IndexDelta.x * x + m_IndexDelta.y * (int)Mathf.Repeat(y - 1, m_View.CellCount.y));
                    break;

                case SelectableListButtonType.Down:
                    SetCurrentIndex(m_IndexDelta.x * x + m_IndexDelta.y * (int)Mathf.Repeat(y + 1, m_View.CellCount.y));
                    break;

                case SelectableListButtonType.Left:
                    SetCurrentIndex(m_IndexDelta.x * (int)Mathf.Repeat(x - 1, m_View.CellCount.x) + m_IndexDelta.y * y);
                    break;

                case SelectableListButtonType.Right:
                    SetCurrentIndex(m_IndexDelta.x * (int)Mathf.Repeat(x + 1, m_View.CellCount.x) + m_IndexDelta.y * y);
                    break;

                case SelectableListButtonType.Submit:
                    m_OnSelected.OnNext(m_CurrentIndex);
                    break;
            }
        }
    }
}
