using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステート管理
    /// </summary>
    public class StateManager : IDisposable
    {
        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource = new();

        /// <summary>
        /// ステートのスタック
        /// </summary>
        private Stack<(StateBase state, Action onPop)> m_StateStack = new();

        /// <summary>
        /// 現在のステート
        /// </summary>
        public StateBase CurrentState { get; private set; }

        /// <summary>
        /// 破棄
        /// </summary>
        public virtual void Dispose()
        {
            this.m_CancellationTokenSource?.Cancel();
            this.m_CancellationTokenSource?.Dispose();
            this.m_CancellationTokenSource = null;

            while (this.m_StateStack.TryPop(out var item))
            {
                item.state?.Dispose();
            }
        }

        /// <summary>
        /// 次のステートに遷移する
        /// </summary>
        public virtual async UniTask PushState(StateBase nextState, Action onPop = null, CancellationToken cancellationToken = default)
        {
            this.m_StateStack.Push((this.CurrentState, onPop));
            this.CurrentState = nextState;

            if (this.CurrentState != null)
            {
                this.CurrentState.SetStateManager(this);

                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.m_CancellationTokenSource.Token, cancellationToken);
                 
                    await this.CurrentState.StartAsync(cts.Token);
                }
                catch
                {
                    await ChangeState(null, cancellationToken);
                    throw;
                }
            }
        }

        /// <summary>
        /// 現在のステートを終了させて、次のステートに遷移する
        /// </summary>
        public virtual async UniTask ChangeState(StateBase nextState, CancellationToken cancellationToken = default) 
        {
            this.CurrentState?.Dispose();
            this.CurrentState = nextState;

            if (this.CurrentState != null)
            {
                this.CurrentState.SetStateManager(this);

                try
                {
                    using var cts = CancellationTokenSource.CreateLinkedTokenSource(this.m_CancellationTokenSource.Token, cancellationToken);

                    await this.CurrentState.StartAsync(cts.Token);
                }
                catch
                {
                    await ChangeState(null, cancellationToken);
                    throw;
                }
            }
        }

        /// <summary>
        /// 前のステートに戻る
        /// </summary>
        public void PopState()
        {
            this.CurrentState?.Dispose();
            this.CurrentState = null;

            if (this.m_StateStack.TryPop(out var item))
            {
                this.CurrentState = item.state;
                item.onPop?.Invoke();
            }
        }
    }
}