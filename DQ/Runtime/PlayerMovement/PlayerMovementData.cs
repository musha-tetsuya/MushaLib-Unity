using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.PlayerMovement
{
    /// <summary>
    /// プレイヤー移動データ
    /// </summary>
    public class PlayerMovementData
    {
        /// <summary>
        /// 移動状況
        /// </summary>
        public PlayerMovementState State { get; set; }

        /// <summary>
        /// 開始位置
        /// </summary>
        public Vector2 StartPosition { get; set; }

        /// <summary>
        /// 終了位置
        /// </summary>
        public Vector2 EndPosition { get; set; }

        /// <summary>
        /// 現在位置
        /// </summary>
        public Vector2 CurrentPosition { get; set; }
    }
}
