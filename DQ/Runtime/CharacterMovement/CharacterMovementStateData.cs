using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.CharacterMovement
{
    /// <summary>
    /// キャラクターの移動状態に関するデータ
    /// </summary>
    public class CharacterMovementStateData
    {
        /// <summary>
        /// 移動状況
        /// </summary>
        public CharacterMovementState State { get; internal set; }

        /// <summary>
        /// 開始位置
        /// </summary>
        public Vector2 StartPosition { get; internal set; }

        /// <summary>
        /// 終了位置
        /// </summary>
        public Vector2 EndPosition { get; internal set; }

        /// <summary>
        /// 現在位置
        /// </summary>
        public Vector2 CurrentPosition { get; internal set; }
    }
}
