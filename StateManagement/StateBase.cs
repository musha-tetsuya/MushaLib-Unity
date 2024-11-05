using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステートインターフェース
    /// </summary>
    public interface IStateBase
    {
        /// <summary>
        /// ステート開始時
        /// </summary>
        UniTask Start(CancellationToken cancellationToken);

        /// <summary>
        /// ステート終了時
        /// </summary>
        void End();
    }

    /// <summary>
    /// ステート基底
    /// </summary>
    public abstract class StateBase<TStateManager> : IStateBase where TStateManager : StateManager
    {
        /// <summary>
        /// ステート管理
        /// </summary>
        public TStateManager StateManager { get; private set; }

        /// <summary>
        /// StateManagerのセット
        /// </summary>
        public virtual void SetStateManager(TStateManager stateManager)
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
    public abstract class ValueStateBase<TValue> : StateBase<ValueStateManager<TValue>>
    {
        /// <summary>
        /// 値
        /// </summary>
        public TValue Value { get; private set; }

        /// <summary>
        /// StateManagerのセット
        /// </summary>
        public override void SetStateManager(ValueStateManager<TValue> stateManager)
        {
            base.SetStateManager(stateManager);
            Value = stateManager.Value;
        }
    }
}
