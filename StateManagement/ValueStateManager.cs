using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// 値付きステート管理
    /// </summary>
    public class ValueStateManager<T> : StateManager
    {
        /// <summary>
        /// 値
        /// </summary>
        public T Value { get; }

        /// <summary>
        /// construct
        /// </summary>
        public ValueStateManager(T value)
        {
            Value = value;
        }
    }
}
