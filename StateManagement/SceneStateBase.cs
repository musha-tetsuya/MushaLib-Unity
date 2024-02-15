using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// シーンステート基底
    /// </summary>
    public abstract class SceneStateBase<T> : StateBase<T>
    {
        /// <summary>
        /// シーン
        /// </summary>
        public T Scene => StateManager.Value;
    }
}
