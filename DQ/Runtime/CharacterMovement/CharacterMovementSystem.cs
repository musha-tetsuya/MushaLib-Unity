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
        /// キャラクターのRectTransform
        /// </summary>
        private readonly RectTransform m_CharacterRectTransform;

        /// <summary>
        /// 移動設定
        /// </summary>
        private readonly ICharacterMovementSettings m_Settings;

        /// <summary>
        /// コリジョンデータ提供
        /// </summary>
        private readonly ICollisionDataProvider m_CollisionDataProvider;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource = new();

        /// <summary>
        /// 移動通知
        /// </summary>
        private Subject<ICharacterMovementData> m_OnMove = new();

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
        private CharacterMovementData m_CharacterMovementData = new();

        /// <summary>
        /// 移動通知
        /// </summary>
        public IObservable<ICharacterMovementData> OnMove => m_OnMove;

        /// <summary>
        /// 移動失敗通知
        /// </summary>
        public IObservable<Unit> OnFailed => m_OnFailed;

        /// <summary>
        /// construct
        /// </summary>
        public CharacterMovementSystem(RectTransform characterRectTransform, ICharacterMovementSettings settings, ICollisionDataProvider collisionDataProvider)
        {
            m_CharacterRectTransform = characterRectTransform;
            m_Settings = settings;
            m_CollisionDataProvider = collisionDataProvider;
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

                // 現在のローカル座標とコリジョンレベル
                var currentLocalPoint = m_CharacterRectTransform.GetRectCenter();
                var currentCollisionLevel = m_CollisionDataProvider.GetCollisionDataAtLocalPoint(currentLocalPoint);

                // 移動先のローカル座標とコリジョンレベル
                var nextLocalPoint = currentLocalPoint + direction * m_Settings.StepDistance;
                var nextCollisionLevel = m_CollisionDataProvider.GetCollisionDataAtLocalPoint(nextLocalPoint);

                // 移動不可なら待機後リトライ
                if (Mathf.Abs(nextCollisionLevel - currentCollisionLevel) > m_Settings.CollisionThreshold)
                {
                    // 移動失敗通知
                    m_OnFailed.OnNext(Unit.Default);

                    await UniTask.Delay((int)(m_Settings.MoveRetryInterval * 1000), cancellationToken: cancellationToken);
                    continue;
                }

                // 移動
                var startPosition = m_CharacterRectTransform.anchoredPosition;
                var endPosition = startPosition + direction * m_Settings.StepDistance;
                var duration = (endPosition - startPosition).magnitude / m_Settings.MoveSpeedPerSec;
                var time = 0f;

                // 移動開始通知
                m_CharacterMovementData.State = CharacterMovementState.Started;
                m_CharacterMovementData.StartPosition = startPosition;
                m_CharacterMovementData.EndPosition = endPosition;
                m_CharacterMovementData.CurrentPosition = m_CharacterRectTransform.anchoredPosition;
                m_OnMove.OnNext(m_CharacterMovementData);

                while (time < duration)
                {
                    // 移動通知
                    m_CharacterMovementData.State = CharacterMovementState.Moving;
                    m_CharacterMovementData.CurrentPosition = m_CharacterRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, time / duration);
                    m_OnMove.OnNext(m_CharacterMovementData);

                    time += Time.deltaTime;

                    await UniTask.NextFrame(cancellationToken);
                }

                // 移動完了通知
                m_CharacterMovementData.State = CharacterMovementState.Finished;
                m_CharacterMovementData.CurrentPosition = m_CharacterRectTransform.anchoredPosition = endPosition;
                m_OnMove.OnNext(m_CharacterMovementData);
            }

            m_IsMoving = false;
        }
    }
}
