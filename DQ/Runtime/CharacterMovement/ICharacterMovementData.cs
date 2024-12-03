using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.CharacterMovement
{
    /// <summary>
    /// キャラクター移動データインターフェース
    /// </summary>
    public interface ICharacterMovementData
    {
        /// <summary>
        /// 移動状況
        /// </summary>
        CharacterMovementState State { get; }

        /// <summary>
        /// 開始位置
        /// </summary>
        Vector2 StartPosition { get; }

        /// <summary>
        /// 終了位置
        /// </summary>
        Vector2 EndPosition { get; }

        /// <summary>
        /// 現在位置
        /// </summary>
        Vector2 CurrentPosition { get; }
    }
}
