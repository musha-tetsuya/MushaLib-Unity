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
        /// 現在のステート
        /// </summary>
        public StateBase CurrentState { get; private set; }

        /// <summary>
        /// CancellationToken
        /// </summary>
        public CancellationToken CancellationToken => this.m_CancellationTokenSource.Token;

        /// <summary>
        /// CancellationTokenSource
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource;

        /// <summary>
        /// ステートのスタック
        /// </summary>
        private Stack<(StateBase state, Action onPop)> m_StateStack = new();

        /// <summary>
        /// construct
        /// </summary>
        public StateManager(CancellationToken cancellation = default)
        {
            this.m_CancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(cancellation);
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public virtual void Dispose()
        {
            this.m_CancellationTokenSource?.Cancel();
            this.m_CancellationTokenSource?.Dispose();
            this.m_CancellationTokenSource = null;
        }

        /// <summary>
        /// 次のステートに遷移する
        /// </summary>
        public virtual void PushState(StateBase nextState, Action onPop = null)
        {
            this.m_StateStack.Push((this.CurrentState, onPop));

            this.CurrentState = nextState;
            this.CurrentState?.SetStateManager(this);
            this.CurrentState?.Start();
        }

        /// <summary>
        /// 現在のステートを終了させて、次のステートに遷移する
        /// </summary>
        public virtual void ChangeState(StateBase nextState) 
        {
            this.CurrentState?.End();
            this.CurrentState = nextState;
            this.CurrentState?.Start();
        }

        /// <summary>
        /// 前のステートに戻る
        /// </summary>
        public void PopState()
        {
            this.CurrentState?.End();
            this.CurrentState = null;

            if (this.m_StateStack.TryPop(out var item))
            {
                this.CurrentState = item.state;
                item.onPop?.Invoke();
            }
        }
    }

    /// <summary>
    /// 値付きステート管理
    /// </summary>
    public class StateManager<T> : StateManager
    {
        /// <summary>
        /// 値
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// construct
        /// </summary>
        public StateManager(T value, CancellationToken cancellation = default)
            : base(cancellation)
        {
            Value = value;
        }
    }
}