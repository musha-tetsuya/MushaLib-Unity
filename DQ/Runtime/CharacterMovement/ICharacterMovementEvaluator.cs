using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.CharacterMovement
{
    /// <summary>
    /// キャラクターの移動の評価を行い、移動データを提供するインターフェース
    /// </summary>
    public interface ICharacterMovementEvaluator
    {
        /// <summary>
        /// 移動の可否を評価し、移動データを生成する
        /// </summary>
        bool EvaluateMovement(CharacterMovementSystem movementSystem, Vector2 direction, out CharacterMovementData movementData);
    }
}
