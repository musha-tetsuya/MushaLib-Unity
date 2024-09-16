using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステート基底
    /// </summary>
    public abstract class StateBase
    {
        /// <summary>
        /// ステート管理
        /// </summary>
        public StateManager StateManager { get; private set; }

        /// <summary>
        /// StateManagerのセット
        /// </summary>
        public virtual void SetStateManager(StateManager stateManager)
        {
            this.StateManager = stateManager;
        }

        /// <summary>
        /// ステート開始時
        /// </summary>
        public virtual void Start()
        {
        }

        /// <summary>
        /// ステート終了時
        /// </summary>
        public virtual void End()
        {
        }
    }

    /// <summary>
    /// 値付きステート基底
    /// </summary>
    public abstract class StateBase<T> : StateBase
    {
        /// <summary>
        /// 値
        /// </summary>
        public T Value { get; private set; }

        /// <summary>
        /// StateManagerのセット
        /// </summary>
        public override void SetStateManager(StateManager stateManager)
        {
            base.SetStateManager(stateManager);

            if (this.StateManager is StateManager<T> valueStateManager)
            {
                Value = valueStateManager.Value;
            }
        }
    }
}
