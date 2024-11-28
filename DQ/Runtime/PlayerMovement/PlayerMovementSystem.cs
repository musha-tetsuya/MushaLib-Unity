using Cysharp.Threading.Tasks;
using MushaLib.UI.VirtualPad;
using MushaLib.Utilities;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UniRx;
using UnityEngine;

namespace MushaLib.DQ.PlayerMovement
{
    /// <summary>
    /// プレイヤー移動システム
    /// </summary>
    public class PlayerMovementSystem : IDisposable
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
        /// プレイヤーのRectTransform
        /// </summary>
        private readonly RectTransform m_PlayerRectTransform;

        /// <summary>
        /// 移動設定
        /// </summary>
        private readonly IPlayerMovementSettings m_Settings;

        /// <summary>
        /// コリジョンデータ提供
        /// </summary>
        private readonly ICollisionDataProvider m_CollisionDataProvider;

        /// <summary>
        /// プレイヤーのワールドでの矩形サイズ
        /// </summary>
        private readonly Vector3 m_PlayerWorldRectSize;

        /// <summary>
        /// プレイヤーのワールド座標オフセット
        /// </summary>
        private readonly Vector3 m_PlayerWorldPositionOffset;

        /// <summary>
        /// キャンセルトークン
        /// </summary>
        private CancellationTokenSource m_CancellationTokenSource = new();

        /// <summary>
        /// 移動通知
        /// </summary>
        private Subject<Vector2> m_OnMove = new();

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
        /// 移動通知
        /// </summary>
        public IObservable<Vector2> OnMove => m_OnMove;

        /// <summary>
        /// 移動失敗通知
        /// </summary>
        public IObservable<Unit> OnFailed => m_OnFailed;

        /// <summary>
        /// construct
        /// </summary>
        public PlayerMovementSystem(RectTransform playerRectTransform, IPlayerMovementSettings settings, ICollisionDataProvider collisionDataProvider)
        {
            m_PlayerRectTransform = playerRectTransform;
            m_Settings = settings;
            m_CollisionDataProvider = collisionDataProvider;

            var corners = new Vector3[4];
            m_PlayerRectTransform.GetWorldCorners(corners);
            m_PlayerWorldRectSize = corners[2] - corners[0];
            m_PlayerWorldPositionOffset.x = m_PlayerWorldRectSize.x * (0.5f - m_PlayerRectTransform.pivot.x);
            m_PlayerWorldPositionOffset.y = m_PlayerWorldRectSize.y * (0.5f - m_PlayerRectTransform.pivot.y);
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
                var currentLocalPoint = m_PlayerRectTransform.GetRectCenter();
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
                var startPosition = m_PlayerRectTransform.anchoredPosition;
                var endPosition = startPosition + direction * m_Settings.StepDistance;
                var duration = (endPosition - startPosition).magnitude / m_Settings.MoveSpeedPerSec;
                var time = 0f;

                while (time < duration)
                {
                    m_PlayerRectTransform.anchoredPosition = Vector2.Lerp(startPosition, endPosition, time / duration);

                    // 移動通知
                    m_OnMove.OnNext(m_PlayerRectTransform.anchoredPosition);

                    time += Time.deltaTime;

                    await UniTask.NextFrame(cancellationToken);
                }

                m_PlayerRectTransform.anchoredPosition = endPosition;

                // 移動通知
                m_OnMove.OnNext(m_PlayerRectTransform.anchoredPosition);
            }

            m_IsMoving = false;
        }
    }
}
