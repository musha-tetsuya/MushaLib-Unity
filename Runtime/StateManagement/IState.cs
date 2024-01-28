using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.StateManagement
{
    /// <summary>
    /// ステートインターフェース
    /// </summary>
    public interface IState
    {
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