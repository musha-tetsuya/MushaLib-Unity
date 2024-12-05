using Cysharp.Threading.Tasks;
using MushaLib.UI.VirtualPad;
using MushaLib.Utilities;
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
        /// 入力方向から移動方向へのテーブル
        /// </summary>
        private static readonly IReadOnlyDictionary<ButtonType, Vector2> DirectionTable = new Dictionary<ButtonType, Vector2>
        {
            { ButtonType.Up, Vector2.up },
            { ButtonType.Down, Vector2.down },
            { ButtonType.Left, Vector2.left },
            { ButtonType.Right, Vector2.right },
        };

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
        private Subject<CharacterMovementStateData> m_OnMove = new();

        /// <summary>
        /// 移動失敗通知
        /// </summary>
        private Subject<Unit> m_OnFailed = new();

        /// <summary>
        /// 押下中のボタンタイプリスト
        /// </summary>
        private List<ButtonType> m_PressedButtonTypes = new();

        /// <summary>
        /// 移動中かどうか
        /// </summary>
        private bool m_IsMoving;

        /// <summary>
        /// 移動データ
        /// </summary>
        private CharacterMovementStateData m_MovementStateData = new();

        /// <summary>
        /// キャラクターのRectTransform
        /// </summary>
        public RectTransform CharacterRectTransform { get; }

        /// <summary>
        /// 移動通知
        /// </summary>
        public IObservable<CharacterMovementStateData> OnMove => m_OnMove;

        /// <summary>
        /// 移動失敗通知
        /// </summary>
        public IObservable<Unit> OnFailed => m_OnFailed;

        /// <summary>
        /// construct
        /// </summary>
        public CharacterMovementSystem(RectTransform characterRectTransform, ICharacterMovementSettings settings, ICharacterMovementEvaluator movementEvaluator)
        {
            CharacterRectTransform = characterRectTransform;
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
            if (DirectionTable.ContainsKey(buttonType))
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
                var direction = DirectionTable[buttonType];

                // 移動不可なら待機後リトライ
                if (!m_MovementEvaluator.EvaluateMovement(this, direction, out var movementData))
                {
                    // 移動失敗通知
                    m_OnFailed.OnNext(Unit.Default);

                    await UniTask.Delay((int)(m_Settings.MoveRetryInterval * 1000), cancellationToken: cancellationToken);
                    continue;
                }

                // 移動時間
                var duration = (movementData.EndPosition - movementData.StartPosition).magnitude / movementData.Speed;
                var time = 0f;

                // 移動開始通知
                m_MovementStateData.State = CharacterMovementState.Started;
                m_MovementStateData.StartPosition = movementData.StartPosition;
                m_MovementStateData.EndPosition = movementData.EndPosition;
                m_MovementStateData.CurrentPosition = CharacterRectTransform.anchoredPosition;
                m_OnMove.OnNext(m_MovementStateData);

                while (time < duration)
                {
                    // 移動通知
                    m_MovementStateData.State = CharacterMovementState.Moving;
                    m_MovementStateData.CurrentPosition = CharacterRectTransform.anchoredPosition = Vector2.Lerp(movementData.StartPosition, movementData.EndPosition, time / duration);
                    m_OnMove.OnNext(m_MovementStateData);

                    time += Time.deltaTime;

                    await UniTask.NextFrame(cancellationToken);
                }

                // 移動完了通知
                m_MovementStateData.State = CharacterMovementState.Finished;
                m_MovementStateData.CurrentPosition = CharacterRectTransform.anchoredPosition = movementData.EndPosition;
                m_OnMove.OnNext(m_MovementStateData);
            }

            m_IsMoving = false;
        }
    }
}
