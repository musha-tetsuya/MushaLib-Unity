using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウ入力待ちイベント
    /// </summary>
    public class WaitInputEvent : IEvent
    {
        /// <summary>
        /// 入力監視
        /// </summary>
        private IObservable<Unit> m_InputObservable;

        /// <summary>
        /// construct
        /// </summary>
        public WaitInputEvent(IObservable<Unit> inputObservable)
        {
            m_InputObservable = inputObservable;
        }

        /// <summary>
        /// 実行
        /// </summary>
        public virtual async UniTask Run(MessageWindow messageWindow, System.Threading.CancellationToken cancellationToken)
        {
            messageWindow.Arrow.gameObject.SetActive(true);
            messageWindow.Arrow.SetAnimationType(Arrow.AnimationType.Blink);

            await m_InputObservable.ToUniTask(true, cancellationToken);

            messageWindow.Arrow.gameObject.SetActive(false);
        }
    }
}
