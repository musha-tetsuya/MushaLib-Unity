using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステートインターフェース
    /// </summary>
    public interface IState<T>
    {
        /// <summary>
        /// ステート開始時Start前に呼ばれる
        /// </summary>
        void PreStart(StateManager<T> stateManager);

        /// <summary>
        /// ステート開始時に呼ばれる
        /// </summary>
        void Start();

        /// <summary>
        /// ステート終了時に呼ばれる
        /// </summary>
        void End();
    }
}