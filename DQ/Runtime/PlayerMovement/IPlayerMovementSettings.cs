using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.PlayerMovement
{
    /// <summary>
    /// プレイヤー移動設定
    /// </summary>
    public interface IPlayerMovementSettings
    {
        /// <summary>
        /// 移動処理のリトライ間隔
        /// </summary>
        float MoveRetryInterval { get; }

        /// <summary>
        /// 一歩の移動距離
        /// </summary>
        float StepDistance { get; }

        /// <summary>
        /// 移動速度（秒速）
        /// </summary>
        float MoveSpeedPerSec { get; }

        /// <summary>
        /// コリジョン閾値
        /// </summary>
        int CollisionThreshold { get; }
    }
}
