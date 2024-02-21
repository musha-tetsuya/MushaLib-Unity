using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.UI.MessageWindow
{
    /// <summary>
    /// メッセージウィンドウクリック待ちイベント
    /// </summary>
    public class WaitClickEvent : IEvent
    {
        /// <summary>
        /// 矢印の表示制御を自動で行うかどうか
        /// </summary>
        public bool AutoArrow { get; set; } = true;

        /// <summary>
        /// 実行
        /// </summary>
        public virtual async UniTask Run(MessageWindow messageWindow, System.Threading.CancellationToken cancellationToken)
        {
            if (AutoArrow)
            {
                messageWindow.Arrow.gameObject.SetActive(true);
                messageWindow.Arrow.SetAnimationType(Arrow.AnimationType.Blink);
            }

            await GetOnClickObservable(messageWindow).ToUniTask(true, cancellationToken);

            if (AutoArrow)
            {
                messageWindow.Arrow.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// クリック監視取得
        /// </summary>
        protected virtual IObservable<Unit> GetOnClickObservable(MessageWindow messageWindow)
        {
            return messageWindow.OnClick;
        }
    }
}
