using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
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
