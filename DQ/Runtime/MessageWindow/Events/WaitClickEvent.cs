using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウクリック待ちイベント
    /// </summary>
    public class WaitClickEvent : IMessageWindowEvent
    {
        /// <summary>
        /// 矢印の表示制御を自動で行うかどうか
        /// </summary>
        public bool AutoArrow { get; set; } = true;

        /// <summary>
        /// 実行
        /// </summary>
        public virtual async UniTask Run(MessageWindowView view, System.Threading.CancellationToken cancellationToken)
        {
            if (AutoArrow)
            {
                view.Arrow.gameObject.SetActive(true);
                view.Arrow.SetAnimationType(Arrow.AnimationType.Blink);
            }

            await GetOnClickObservable(view).ToUniTask(true, cancellationToken);

            if (AutoArrow)
            {
                view.Arrow.gameObject.SetActive(false);
            }
        }

        /// <summary>
        /// クリック監視取得
        /// </summary>
        protected virtual IObservable<Unit> GetOnClickObservable(MessageWindowView view)
        {
            return view.OnClick;
        }
    }
}
