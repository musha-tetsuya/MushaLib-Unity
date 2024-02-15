using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステート基底
    /// </summary>
    public abstract class StateBase<T> : IState<T>
    {
        /// <summary>
        /// ステート管理
        /// </summary>
        public StateManager<T> StateManager { get; private set; }

        /// <summary>
        /// ステート開始時
        /// </summary>
        public virtual void Start(StateManager<T> stateManager)
        {
            StateManager = stateManager;
        }

        /// <summary>
        /// ステート終了時
        /// </summary>
        public virtual void End()
        {
        }
    }
}
