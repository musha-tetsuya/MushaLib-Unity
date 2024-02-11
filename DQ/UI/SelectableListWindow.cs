using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace MushaLib.DQ.UI
{
    /// <summary>
    /// 選択可能リストウィンドウ
    /// </summary>
    public abstract class SelectableListWindow : ListWindow
    {
        /// <summary>
        /// 現在のインデックス
        /// </summary>
        public int CurrentIndex { get; protected set; }

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            ScrollView.OnUpdateElement += (element, index) =>
            {
                (element as SelectableText).Arrow.SetAnimationType(index == CurrentIndex ? Arrow.AnimationType.Blink : Arrow.AnimationType.Hide);
            };
        }

        /// <summary>
        /// 
        /// </summary>
        protected SelectableText GetCurrent()
        {
            return ScrollView.ScrollElements.Where(x => x.Index == CurrentIndex).OfType<SelectableText>().FirstOrDefault(x => x.gameObject.activeInHierarchy);
        }

        /// <summary>
        /// インデックス変更
        /// </summary>
        public virtual void SetCurrentIndex(int index)
        {
            index = (int)Mathf.Repeat(index, ScrollView.ElementCount);

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