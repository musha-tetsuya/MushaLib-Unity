using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.DQ.UI
{
    /// <summary>
    /// 選択可能リストウィンドウ
    /// </summary>
    public class SelectableListWindow : Window
    {
        /// <summary>
        /// GridLayoutGroup
        /// </summary>
        [SerializeField]
        private GridLayoutGroup m_GridLayoutGroup;

        /// <summary>
        /// GridLayoutGroup
        /// </summary>
        public GridLayoutGroup GridLayoutGroup => m_GridLayoutGroup;

        /// <summary>
        /// 現在のインデックス
        /// </summary>
        public int CurrentIndex { get; protected set; }

        /// <summary>
        /// 選択時イベント
        /// </summary>
        public event Action OnSelected;

        /// <summary>
        /// Start
        /// </summary>
        protected virtual void Start()
        {
            for (int i = 0; i < m_GridLayoutGroup.transform.childCount; i++)
            {
                var element = m_GridLayoutGroup.transform.GetChild(i).GetComponent<SelectableText>();

                element.Arrow.SetAnimationType(i == CurrentIndex ? Arrow.AnimationType.Blink : Arrow.AnimationType.Hide);
            };
        }

        /// <summary>
        /// 要素クリック時
        /// </summary>
        public void OnClickElement(SelectableText element)
        {
            int elementIndex = element.transform.GetSiblingIndex();

            if (CurrentIndex == elementIndex)
            {
                Select();
            }
            else
            {
                SetCurrentIndex(elementIndex);
            }
        }

        /// <summary>
        /// 現在選択中の要素
        /// </summary>
        protected SelectableText GetCurrent()
        {
            return m_GridLayoutGroup.transform.GetChild(CurrentIndex).GetComponent<SelectableText>();
        }

        /// <summary>
        /// インデックス変更
        /// </summary>
        public virtual void SetCurrentIndex(int index)
        {
            index = (int)Mathf.Repeat(index, m_GridLayoutGroup.transform.childCount);

            if (index != CurrentIndex)
            {
                GetCurrent().Arrow.SetAnimationType(Arrow.AnimationType.Hide);

                CurrentIndex = index;

                GetCurrent().Arrow.SetAnimationType(Arrow.AnimationType.Blink);
            }
        }

        /// <summary>
        /// 選択
        /// </summary>
        public virtual void Select()
        {
            GetCurrent().Arrow.SetAnimationType(Arrow.AnimationType.Show);

            CanvasGroup.interactable = false;

            OnSelected?.Invoke();
        }

        /// <summary>
        /// 選択解除
        /// </summary>
        public virtual void Deselect()
        {
            GetCurrent().Arrow.SetAnimationType(Arrow.AnimationType.Blink);

            CanvasGroup.interactable = true;
        }
    }
}