using MushaLib.UI.VirtualPad;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// 選択可能リスト制御
    /// </summary>
    public class SelectableListPresenter : IDisposable
    {
        /// <summary>
        /// モデル
        /// </summary>
        private readonly SelectableListModel m_Model;

        /// <summary>
        /// ビュー
        /// </summary>
        private readonly SelectableListView m_View;

        /// <summary>
        /// クリック購読の破棄
        /// </summary>
        private IDisposable m_OnClickDisposable;

        /// <summary>
        /// 選択決定時
        /// </summary>
        private Subject<SelectableElement> m_OnSelected = new();

        /// <summary>
        /// 選択決定時
        /// </summary>
        public IObservable<SelectableElement> OnSelected => m_OnSelected;

        /// <summary>
        /// construct
        /// </summary>
        public SelectableListPresenter(SelectableListModel model, SelectableListView view)
        {
            m_Model = model;
            m_View = view;
        }

        /// <summary>
        /// 解放
        /// </summary>
        public virtual void Dispose()
        {
            m_OnClickDisposable?.Dispose();
            m_OnClickDisposable = null;

            m_OnSelected.Dispose();
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public virtual void Initialize()
        {
            // ビュー初期化
            m_View.Initialize();

            // 要素クリック時
            m_OnClickDisposable?.Dispose();
            m_OnClickDisposable = m_View.OnClick
                .Subscribe(element =>
                {
                    int index = m_View.Elements.IndexOf(element);
                    if (index != m_Model.CurrentIndex)
                    {
                        // 選択インデックスを変更
                        SetCurrentIndex(index);
                    }
                    else
                    {
                        // 選択決定を通知
                        InvokeOnSelected();
                    }
                });

            // 初期選択
            SetCurrentIndex(m_Model.StartIndex);
        }

        /// <summary>
        /// パッド操作時
        /// </summary>
        public virtual void OnPadPressed(ButtonType buttonType)
        {
            if (m_View.Interactable)
            {
                switch (buttonType)
                {
                    case ButtonType.Up:
                        MoveCurrentIndex(0, -1);
                        break;

                    case ButtonType.Down:
                        MoveCurrentIndex(0, 1);
                        break;

                    case ButtonType.Left:
                        MoveCurrentIndex(-1, 0);
                        break;

                    case ButtonType.Right:
                        MoveCurrentIndex(1, 0);
                        break;

                    case ButtonType.A:
                        InvokeOnSelected();
                        break;
                }
            }
        }

        /// <summary>
        /// 選択インデックスの変更
        /// </summary>
        protected void SetCurrentIndex(int index)
        {
            index = (int)Mathf.Repeat(index, m_View.Elements.Count);

            // 選択中要素の矢印を非表示に
            m_View.Elements[m_Model.CurrentIndex].Arrow.SetAnimationType(Arrow.AnimationType.Hide);

            // インデックス変更
            m_Model.CurrentIndex = index;

            // 新しく選択した要素の矢印を点滅表示
            m_View.Elements[m_Model.CurrentIndex].Arrow.SetAnimationType(Arrow.AnimationType.Blink);
        }

        /// <summary>
        /// 選択インデックスの移動
        /// </summary>
        private void MoveCurrentIndex(int moveX, int moveY)
        {
            var pos = Vector2Int.zero;
            var delta = Vector2Int.zero;

            if (m_View.StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                pos.x = m_Model.CurrentIndex % m_View.CellCount.x;
                pos.y = m_Model.CurrentIndex / m_View.CellCount.x;

                delta.x = 1;
                delta.y = m_View.CellCount.x;
            }
            else
            {
                pos.x = m_Model.CurrentIndex / m_View.CellCount.y;
                pos.y = m_Model.CurrentIndex % m_View.CellCount.y;

                delta.x = m_View.CellCount.y;
                delta.y = 1;
            }

            while (true)
            {
                pos.x = (int)Mathf.Repeat(pos.x + moveX, m_View.CellCount.x);
                pos.y = (int)Mathf.Repeat(pos.y + moveY, m_View.CellCount.y);

                var nextIndex = pos.x * delta.x + pos.y * delta.y;
                if (nextIndex == m_Model.CurrentIndex)
                {
                    break;
                }

                if (m_View.Elements[nextIndex].gameObject.activeInHierarchy)
                {
                    SetCurrentIndex(nextIndex);
                    break;
                }
            }
        }

        /// <summary>
        /// 選択決定を通知
        /// </summary>
        protected virtual void InvokeOnSelected()
        {
            m_OnSelected.OnNext(m_View.Elements[m_Model.CurrentIndex]);
        }

        /// <summary>
        /// 選択決定
        /// </summary>
        public void Select()
        {
            // 選択中要素の矢印の点滅を解除
            m_View.Elements[m_Model.CurrentIndex].Arrow.SetAnimationType(Arrow.AnimationType.Show);

            // リストに触れなくする
            m_View.Interactable = false;
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public void Deselect()
        {
            // 選択中要素の矢印を点滅表示
            m_View.Elements[m_Model.CurrentIndex].Arrow.SetAnimationType(Arrow.AnimationType.Blink);

            // リストに触れるようにする
            m_View.Interactable = true;
        }
    }
}
