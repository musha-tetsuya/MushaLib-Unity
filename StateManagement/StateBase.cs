using Cysharp.Threading.Tasks;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステート基底
    /// </summary>
    public abstract class StateBase : IDisposable
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
        public virtual UniTask StartAsync(CancellationToken cancellationToken)
        {
            return UniTask.CompletedTask;
        }

        /// <summary>
        /// ステート終了時
        /// </summary>
        public virtual void Dispose()
        {
        }
    }
}
