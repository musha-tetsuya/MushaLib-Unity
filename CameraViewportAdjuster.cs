using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib
{
    /// <summary>
    /// カメラのViewport調整
    /// </summary>
    /// <remarks>
    /// 使用方法：
    /// TargetCameraにViewportを調整したいカメラを設定。
    /// ScreenSizeにキャンバスのサイズや画面比率などを設定。
    /// </remarks>
    [ExecuteAlways]
    public class CameraViewportAdjuster : MonoBehaviour
    {
        /// <summary>
        /// ターゲットカメラ
        /// </summary>
        [SerializeField]
        private Camera m_TargetCamera;

        /// <summary>
        /// スクリーンサイズ
        /// </summary>
        [SerializeField]
        private Vector2 m_ScreenSize = Vector2.one;

        /// <summary>
        /// セーフエリア内に収めるかどうか
        /// </summary>
        [SerializeField]
        private bool m_FitInSafeArea;

        /// <summary>
        /// アンカー
        /// </summary>
        [SerializeField, Range(0f, 1f)]
        private float m_Anchor = 0.5f;

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            if (Application.isPlaying)
            {
                RecalcViewport();
            }
        }

#if UNITY_EDITOR
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
            {
                RecalcViewport();
            }
        }
#endif

        /// <summary>
        /// Viewport矩形の再計算
        /// </summary>
        public void RecalcViewport()
        {
            if (m_TargetCamera == null)
            {
                return;
            }

            float screenWidth = Screen.width;
            float screenHeight = Screen.height;
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                var screenRes = UnityEditor.UnityStats.screenRes.Split("x");
                screenWidth = float.Parse(screenRes[0]);
                screenHeight = float.Parse(screenRes[1]);
            }
#endif
            var safeArea = Screen.safeArea;

            if (!m_FitInSafeArea)
            {
                safeArea.x = 0;
                safeArea.y = 0;
                safeArea.width = screenWidth;
                safeArea.height = screenHeight;
            }

            var scale = Mathf.Min(safeArea.width / m_ScreenSize.x, safeArea.height / m_ScreenSize.y);
            var scaledScreenSize = m_ScreenSize * scale;

            var rect = m_TargetCamera.rect;
            rect.x = (safeArea.x + (safeArea.width - scaledScreenSize.x) * m_Anchor) / screenWidth;
            rect.y = (safeArea.y + (safeArea.height - scaledScreenSize.y) * m_Anchor) / screenHeight;
            rect.width = scaledScreenSize.x / screenWidth;
            rect.height = scaledScreenSize.y / screenHeight;
            m_TargetCamera.rect = rect;
        }
    }
}
