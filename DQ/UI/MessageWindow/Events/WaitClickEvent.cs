using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウクリック待ちイベント
    /// </summary>
    public class WaitClickEvent : IEvent
    {
        /// <summary>
        /// 追加入力監視
        /// </summary>
        private IObservable<Unit> m_AdditionalObservable;

        /// <summary>
        /// construct
        /// </summary>
        public WaitClickEvent(IObservable<Unit> additionalObservable = null)
        {
            m_AdditionalObservable = additionalObservable;
        }

        /// <summary>
        /// 実行
        /// </summary>
        public virtual async UniTask Run(MessageWindow messageWindow, System.Threading.CancellationToken cancellationToken)
        {
            messageWindow.Arrow.gameObject.SetActive(true);
            messageWindow.Arrow.SetAnimationType(Arrow.AnimationType.Blink);

            if (m_AdditionalObservable == null)
            {
                await messageWindow.OnClick.ToUniTask(true, cancellationToken);
            }
            else
            {
                await messageWindow.OnClick.Merge(m_AdditionalObservable).ToUniTask(true, cancellationToken);
            }

            messageWindow.Arrow.gameObject.SetActive(false);
        }
    }
}
