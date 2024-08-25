using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.UI
{
    /// <summary>
    /// セーフエリアに基づいて、RectTransformの位置とサイズを調整するクラス
    /// </summary>
    /// <remarks>
    /// 使用方法：
    /// Canvas直下に設置したオブジェクトにコンポーネントを付与する
    /// </remarks>
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        private RectTransform m_RectTransform;

        /// <summary>
        /// キャンバスのスケール
        /// </summary>
        private ReactiveProperty<Vector3> m_CanvasScale;

        /// <summary>
        /// Disposable
        /// </summary>
        private CompositeDisposable m_Disposable;

        /// <summary>
        /// DrivenRectTransformTracker
        /// </summary>
        private DrivenRectTransformTracker m_Tracker;

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            if (m_RectTransform == null)
            {
                m_RectTransform = transform as RectTransform;
            }

            m_Disposable?.Dispose();
            m_Disposable = new CompositeDisposable();

            m_CanvasScale = new ReactiveProperty<Vector3>(m_RectTransform.parent.lossyScale).AddTo(m_Disposable);

            // グローバルスケールを毎フレーム監視
            UniTaskAsyncEnumerable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    m_CanvasScale.Value = m_RectTransform.parent.lossyScale;
                })
                .AddTo(m_Disposable);

            // 画面サイズ、キャンバスサイズに変更があったらセーフエリア矩形を再計算する
            Observable
                .Merge(
                    ScreenObserver.ScreenInfo.SkipLatestValueOnSubscribe().AsUnitObservable(),
                    m_CanvasScale.SkipLatestValueOnSubscribe().AsUnitObservable())
                .Subscribe(_ =>
                {
                    RecalcRect();
                })
                .AddTo(m_Disposable);

            // 初回矩形計算
            RecalcRect();
        }

        /// <summary>
        /// OnDisable
        /// </summary>
        private void OnDisable()
        {
            m_Disposable?.Dispose();
            m_Disposable = null;
        }

        /// <summary>
        /// セーフエリア矩形の再計算
        /// </summary>
        private void RecalcRect()
        {
            var canvasScale = m_CanvasScale.Value.x;
            if (canvasScale > 0f)
            {
                var safeArea = ScreenObserver.SafeArea.Value;
                var canvasSafeAreaSize = safeArea.size / canvasScale;
                var canvasSafeAreaPosition = safeArea.position / canvasScale;

                m_Tracker.Clear();

                m_Tracker.Add(this, m_RectTransform, DrivenTransformProperties.All);
                m_RectTransform.anchoredPosition = canvasSafeAreaPosition;
                m_RectTransform.sizeDelta = canvasSafeAreaSize;
                m_RectTransform.anchorMin =
                m_RectTransform.anchorMax =
                m_RectTransform.pivot = Vector2.zero;
                m_RectTransform.eulerAngles = Vector3.zero;
                m_RectTransform.localScale = Vector3.one;
            }
        }
    }
}