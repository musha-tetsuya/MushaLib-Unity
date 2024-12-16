using Cysharp.Threading.Tasks;
using MushaLib.UI.VirtualPad;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.CharacterMovement
{
    /// <summary>
    /// キャラクター移動システム
    /// </summary>
    public class CharacterMovementSystem : IDisposable
    {
        /// <summary>
        /// 移動設定
        /// </summary>
        private readonly ICharacterMovementSettings m_Settings;

        /// <summary>
        /// 移動可否判定
        /// </summary>
        private readonly ICharacterMovementEvaluator m_MovementEvaluator;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource = new();

        /// <summary>
        /// 移動通知
        /// </summary>
        private Subject<(CharacterMovementState state, CharacterMovementData movementData, Vector2 currentPosition)> m_OnMove = new();

        /// <summary>
        /// 移動失敗通知
        /// </summary>
        private Subject<CharacterMovementData> m_OnFailed = new();

        /// <summary>
        /// 押下中のボタンタイプリスト
        /// </summary>
        private List<ButtonType> m_PressedButtonTypes = new();

        /// <summary>
        /// 移動中かどうか
        /// </summary>
        private bool m_IsMoving;

        /// <summary>
        /// 移動通知
        /// </summary>
        public IObservable<(CharacterMovementState state, CharacterMovementData movementData, Vector2 currentPosition)> OnMove => m_OnMove;

        /// <summary>
        /// 移動失敗通知
        /// </summary>
        public IObservable<CharacterMovementData> OnFailed => m_OnFailed;

        /// <summary>
        /// construct
        /// </summary>
        public CharacterMovementSystem(ICharacterMovementSettings settings, ICharacterMovementEvaluator movementEvaluator)
        {
            m_Settings = settings;
            m_MovementEvaluator = movementEvaluator;
        }

        /// <summary>
        /// 破棄
        /// </summary>
        public void Dispose()
        {
            m_CancellationTokenSource?.Cancel();
            m_CancellationTokenSource?.Dispose();
            m_CancellationTokenSource = null;

            m_OnMove.Dispose();
            m_OnFailed.Dispose();
        }

        /// <summary>
        /// パッド押下時
        /// </summary>
        public void OnPadPress(ButtonType buttonType)
        {
            if (buttonType is ButtonType.Up or ButtonType.Down or ButtonType.Left or ButtonType.Right)
            {
                if (!m_PressedButtonTypes.Contains(buttonType))
                {
                    m_PressedButtonTypes.Insert(0, buttonType);

                    if (!m_IsMoving)
                    {
                        MoveAsync(m_CancellationTokenSource.Token).Forget();
                    }
                }
            }
        }

        /// <summary>
        /// パッド離脱時
        /// </summary>
        public void OnPadRelease(ButtonType buttonType)
        {
            m_PressedButtonTypes.Remove(buttonType);
        }

        /// <summary>
        /// 移動
        /// </summary>
        private async UniTask MoveAsync(CancellationToken cancellationToken)
        {
            m_IsMoving = true;

            while (m_PressedButtonTypes.Count > 0)
            {
                var buttonType = m_PressedButtonTypes[0];

                // 移動不可なら待機後リトライ
                if (!m_MovementEvaluator.EvaluateMovement(this, buttonType, out var movementData))
                {
                    // 移動失敗通知
                    m_OnFailed.OnNext(movementData);

                    await UniTask.Delay((int)(m_Settings.MoveRetryInterval * 1000), cancellationToken: cancellationToken);
                    continue;
                }

                // 移動時間
                var duration = (movementData.EndPosition - movementData.StartPosition).magnitude / movementData.Speed;
                var time = 0f;

                // 移動開始通知
                m_OnMove.OnNext((CharacterMovementState.Started, movementData, movementData.StartPosition));

                while (time < duration)
                {
                    // 移動通知
                    m_OnMove.OnNext((CharacterMovementState.Moving, movementData, Vector2.Lerp(movementData.StartPosition, movementData.EndPosition, time / duration)));

                    time += Time.deltaTime;

                    await UniTask.NextFrame(cancellationToken);
                }

                // 移動完了通知
                m_OnMove.OnNext((CharacterMovementState.Finished, movementData, movementData.EndPosition));
            }

            m_IsMoving = false;
        }
    }
}
