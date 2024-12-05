using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.CharacterMovement
{
    /// <summary>
    /// キャラクター移動設定
    /// </summary>
    public interface ICharacterMovementSettings
    {
        /// <summary>
        /// 移動処理のリトライ間隔
        /// </summary>
        float MoveRetryInterval { get; }
    }
}
