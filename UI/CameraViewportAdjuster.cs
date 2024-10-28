using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace MushaLib.UI
{
    /// <summary>
    /// カメラのビューポート調整
    /// </summary>
    /// <remarks>
    /// 使用方法：Viewportを調整したいカメラに付与する
    /// </remarks>
    [ExecuteAlways]
    [RequireComponent(typeof(Camera))]
    public class CameraViewportAdjuster : MonoBehaviour
    {
        /// <summary>
        /// アスペクト比
        /// </summary>
        [SerializeField]
        private Vector2 m_AspectRatio = Vector2.one;

        /// <summary>
        /// アンカー
        /// </summary>
        [SerializeField]
        private Vector2 m_Anchor = Vector2.one * 0.5f;

        /// <summary>
        /// セーフエリア内に収めるかどうか
        /// </summary>
        [SerializeField]
        private bool m_FitInSafeArea = true;

        /// <summary>
        /// カメラ
        /// </summary>
        private Camera m_Camera;

        /// <summary>
        /// Disposable
        /// </summary>
        private CompositeDisposable m_Disposable;

        /// <summary>
        /// カメラ
        /// </summary>
        private Camera Camera => m_Camera ?? (m_Camera = GetComponent<Camera>());

        /// <summary>
        /// アスペクト比
        /// </summary>
        public Vector2 AspectRatio
        {
            get => m_AspectRatio;
            set
            {
                if (m_AspectRatio != value)
                {
                    m_AspectRatio = value;
                    RecalcViewport();
                }
            }
        }

        /// <summary>
        /// アンカー
        /// </summary>
        public Vector2 Anchor
        {
            get => m_Anchor;
            set
            {
                if (m_Anchor != value)
                {
                    m_Anchor = value;
                    RecalcViewport();
                }
            }
        }

        /// <summary>
        /// セーフエリア内に収めるかどうか
        /// </summary>
        public bool FitInSafeArea
        {
            get => m_FitInSafeArea;
            set
            {
                if (m_FitInSafeArea != value)
                {
                    m_FitInSafeArea = value;
                    RecalcViewport();
                }
            }
        }

        /// <summary>
        /// OnEnable
        /// </summary>
        private void OnEnable()
        {
            m_Disposable?.Dispose();
            m_Disposable = new CompositeDisposable();

            // 画面サイズに変更があったらViewport矩形を再計算する
            ScreenObserver.ScreenInfo
                .SkipLatestValueOnSubscribe()
                .Subscribe(_ =>
                {
                    RecalcViewport();
                })
                .AddTo(m_Disposable);

            // 初回矩形計算
            RecalcViewport();
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
        /// Viewport矩形の再計算
        /// </summary>
        private void RecalcViewport()
        {
            float screenWidth = ScreenObserver.Width.Value;
            float screenHeight = ScreenObserver.Height.Value;
            var safeArea = ScreenObserver.SafeArea.Value;

            if (!m_FitInSafeArea)
            {
                safeArea.x = 0;
                safeArea.y = 0;
                safeArea.width = screenWidth;
                safeArea.height = screenHeight;
            }

            var scale = Mathf.Min(safeArea.width / m_AspectRatio.x, safeArea.height / m_AspectRatio.y);
            var scaledResolution = m_AspectRatio * scale;

            var viewportRect = Camera.rect;
            viewportRect.x = (safeArea.x + (safeArea.width - scaledResolution.x) * m_Anchor.x) / screenWidth;
            viewportRect.y = (safeArea.y + (safeArea.height - scaledResolution.y) * m_Anchor.y) / screenHeight;
            viewportRect.width = scaledResolution.x / screenWidth;
            viewportRect.height = scaledResolution.y / screenHeight;
            Camera.rect = viewportRect;

            var additionalCameraData = Camera.GetUniversalAdditionalCameraData();
            if (additionalCameraData.renderType == CameraRenderType.Base)
            {
                for (int i = 0; i < additionalCameraData.cameraStack.Count; i++)
                {
                    if (additionalCameraData.cameraStack[i] != null)
                    {
                        additionalCameraData.cameraStack[i].rect = viewportRect;
                    }
                }
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// カスタムインスペクター
        /// </summary>
        [CustomEditor(typeof(CameraViewportAdjuster))]
        private class CameraViewportAdjusterInspector : Editor
        {
            /// <summary>
            /// ターゲット
            /// </summary>
            private new CameraViewportAdjuster target => base.target as CameraViewportAdjuster;

            /// <summary>
            /// OnInspectorGUI
            /// </summary>
            public override void OnInspectorGUI()
            {
                var aspectRatio = target.m_AspectRatio;
                var anchor = target.m_Anchor;
                var fitInSafeArea = target.m_FitInSafeArea;

                base.OnInspectorGUI();

                // 値に変化があったらViewport矩形再計算
                if (target.m_AspectRatio != aspectRatio ||
                    target.m_Anchor != anchor ||
                    target.m_FitInSafeArea != fitInSafeArea)
                {
                    target.RecalcViewport();
                }
            }
        }
#endif
    }
}
