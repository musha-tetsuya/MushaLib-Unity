using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
        public virtual UniTask Start(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
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
    public abstract class ValueStateBase<T> : StateBase
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

            if (stateManager is ValueStateManager<T> valueStateManager)
            {
                Value = valueStateManager.Value;
            }
        }
    }
}
