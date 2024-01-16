using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.UI.Layouting
{
    /// <summary>
    /// セーフエリア
    /// </summary>
    /// <remarks>
    /// 使用方法：
    /// Canvas直下にGameObjectを作成し、このコンポーネントを付与する。
    /// </remarks>
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        /// <summary>
        /// セーフエリア自身のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// CanvasのRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_CanvasRectTransform;

        /// <summary>
        /// CanvasScaler
        /// </summary>
        [SerializeField]
        private CanvasScaler m_CanvasScaler;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
#if UNITY_EDITOR
            if (!Application.isPlaying)
            {
                return;
            }
#endif
            // CanvasのRectTransformに変更があったらセーフエリア矩形を再計算する
            this.m_CanvasRectTransform
                .OnRectTransformDimensionsChangeAsObservable()
                .Subscribe(_ => RecalcRect())
                .AddTo(this.destroyCancellationToken);
        }

#if UNITY_EDITOR
        /// <summary>
        /// Update
        /// </summary>
        private void Update()
        {
            if (!Application.isPlaying)
            {
                RecalcRect();
            }
        }
#endif

        /// <summary>
        /// セーフエリア矩形の再計算
        /// </summary>
        private void RecalcRect()
        {
            if (this.m_RectTransform == null || this.m_CanvasRectTransform == null || this.m_CanvasScaler == null)
            {
                return;
            }

            var scale = 1f / this.m_CanvasRectTransform.localScale.x;

            this.m_RectTransform.anchoredPosition = Screen.safeArea.position * scale;
            this.m_RectTransform.sizeDelta = new Vector2(Screen.safeArea.width * scale, Screen.safeArea.height * scale);
            this.m_RectTransform.anchorMin =
            this.m_RectTransform.anchorMax =
            this.m_RectTransform.pivot = Vector2.zero;
        }
    }
}