using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

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
    [RequireComponent(typeof(RectTransform))]
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
        /// Reset
        /// </summary>
        private void Reset()
        {
            this.m_RectTransform = this.transform as RectTransform;
        }

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
            if (this.m_RectTransform == null || this.m_CanvasRectTransform == null)
            {
                return;
            }

            var scale = Mathf.Max(this.m_CanvasRectTransform.rect.width / Screen.width, this.m_CanvasRectTransform.rect.height / Screen.height);

            this.m_RectTransform.anchoredPosition = Screen.safeArea.position * scale;
            this.m_RectTransform.sizeDelta = Screen.safeArea.size * scale;
            this.m_RectTransform.anchorMin =
            this.m_RectTransform.anchorMax =
            this.m_RectTransform.pivot = Vector2.zero;
        }
    }
}