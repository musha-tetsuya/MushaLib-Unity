using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.CharacterMovement
{
    /// <summary>
    /// キャラクター移動データ
    /// </summary>
    public struct CharacterMovementData
    {
        /// <summary>
        /// 開始位置
        /// </summary>
        public Vector2 StartPosition { get; set; }

        /// <summary>
        /// 終了位置
        /// </summary>
        public Vector2 EndPosition { get; set; }

        /// <summary>
        /// 秒速
        /// </summary>
        public float Speed { get; set; }
    }
}
