using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UnityEngine;

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
        protected TSelectableList View { get; }

        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource;

        /// <summary>
        /// 選択中インデックス
        /// </summary>
        protected int CurrentIndex { get; private set; }

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
            View = view;
            CurrentIndex = -1;
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

            foreach (var x in View.GetElements().Select((element, i) => (element, i)))
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
            if (index != CurrentIndex)
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
        protected void SetCurrentIndex(int index)
        {
            index = (int)Mathf.Repeat(index, View.Count);

            if (index != CurrentIndex)
            {
                // 選択中要素の矢印を非表示に
                View.GetElement(CurrentIndex)?.Arrow.SetAnimationType(Arrow.AnimationType.Hide);

                // インデックス変更
                CurrentIndex = index;

                // 新しく選択した要素の矢印を点滅表示
                View.GetElement(CurrentIndex)?.Arrow.SetAnimationType(Arrow.AnimationType.Blink);
            }
        }

        /// <summary>
        /// 選択決定
        /// </summary>
        public void Select()
        {
            var element = View.GetElement(CurrentIndex);
            if (element != null)
            {
                // 選択中要素の矢印の点滅を解除
                element.Arrow.SetAnimationType(Arrow.AnimationType.Show);

                // リストに触れなくする
                View.SetInteractable(false);
            }
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            var element = View.GetElement(CurrentIndex);
            if (element != null)
            {
                // 選択中要素の矢印を点滅表示
                element.Arrow.SetAnimationType(Arrow.AnimationType.Blink);

                // リストに触れるようにする
                View.SetInteractable(true);
            }
        }
    }
}
