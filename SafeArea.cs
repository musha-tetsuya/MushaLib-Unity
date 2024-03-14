using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MushaLib
{
    /// <summary>
    /// セーフエリア
    /// </summary>
    /// <remarks>
    /// 使用方法：
    /// Canvas直下にRectTransformを作成し、TargetRectTransformに設定する。
    /// </remarks>
    [ExecuteAlways]
    public class SafeArea : MonoBehaviour
    {
        /// <summary>
        /// ターゲットのRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_TargetRectTransform;

        /// <summary>
        /// CanvasのRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_CanvasRectTransform;

        /// <summary>
        /// 再計算のトリガーとなるRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_TriggeredRectTransform;

        /// <summary>
        /// Awake
        /// </summary>
        private void Awake()
        {
            if (Application.isPlaying)
            {
                if (this.m_TriggeredRectTransform != null)
                {
                    // トリガーのRectTransformに変更があったらセーフエリア矩形を再計算する
                    this.m_TriggeredRectTransform
                        .OnRectTransformDimensionsChangeAsObservable()
                        .Subscribe(_ => RecalcRect())
                        .AddTo(this.destroyCancellationToken);
                }
            }    
        }

        /// <summary>
        /// Start
        /// </summary>
        private void Start()
        {
            if (Application.isPlaying)
            {
                // 初回矩形計算
                RecalcRect();
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
                RecalcRect();
            }
        }
#endif

        /// <summary>
        /// セーフエリア矩形の再計算
        /// </summary>
        public void RecalcRect()
        {
            if (this.m_TargetRectTransform == null || this.m_CanvasRectTransform == null)
            {
                return;
            }

            var scale = Mathf.Max(this.m_CanvasRectTransform.rect.width / Screen.width, this.m_CanvasRectTransform.rect.height / Screen.height);

            this.m_TargetRectTransform.anchoredPosition = Screen.safeArea.position * scale;
            this.m_TargetRectTransform.sizeDelta = Screen.safeArea.size * scale;
            this.m_TargetRectTransform.anchorMin =
            this.m_TargetRectTransform.anchorMax =
            this.m_TargetRectTransform.pivot = Vector2.zero;
        }
    }
}