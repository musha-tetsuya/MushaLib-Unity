using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.PlayerMovement
{
    /// <summary>
    /// プレイヤー移動状態
    /// </summary>
    public enum PlayerMovementState
    {
        /// <summary>
        /// 移動開始時
        /// </summary>
        Started,

        /// <summary>
        /// 移動中
        /// </summary>
        Moving,

        /// <summary>
        /// 移動完了時
        /// </summary>
        Finished,
    }
}
