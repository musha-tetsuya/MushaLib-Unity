using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステート管理
    /// </summary>
    public class StateManager
    {
        /// <summary>
        /// 現在のステート
        /// </summary>
        public IState CurrentState { get; private set; }

        /// <summary>
        /// ステートのスタック
        /// </summary>
        private Stack<(IState state, Action onPop)> m_StateStack = new();

        /// <summary>
        /// 次のステートに遷移する
        /// </summary>
        public virtual void PushState(IState nextState, Action onPop = null)
        {
            this.m_StateStack.Push((this.CurrentState, onPop));

            this.CurrentState = nextState;
            this.CurrentState?.Start();
        }

        /// <summary>
        /// 現在のステートを終了させて、次のステートに遷移する
        /// </summary>
        public virtual void ChangeState(IState nextState) 
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
}