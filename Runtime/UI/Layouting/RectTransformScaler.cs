using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UniRx.Triggers;
using UnityEngine;

namespace MushaLib.UI.Layouting
{
    /// <summary>
    /// ターゲットのサイズに合わせてスケーリングするクラス
    /// </summary>
    /// <remarks>
    /// 使用方法：
    /// スケーリングしたいRectTransformにこのコンポーネントを付与。
    /// 自身のRectTransformとターゲットのRectTransformをInspectorでセットする。
    /// </remarks>
    [ExecuteAlways]
    public class RectTransformScaler : MonoBehaviour
    {
        /// <summary>
        /// 自身のRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_RectTransform;

        /// <summary>
        /// スケーリング対象となるRectTransform
        /// </summary>
        [SerializeField]
        private RectTransform m_TargetRectTransform;

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
            // ターゲットのRectTransformに変更があったらスケールを再計算
            this.m_TargetRectTransform
                .OnRectTransformDimensionsChangeAsObservable()
                .Subscribe(_ => RecalcScale())
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
                RecalcScale();
            }
        }
#endif

        /// <summary>
        /// スケールの再計算
        /// </summary>
        private void RecalcScale()
        {
            if (this.m_RectTransform == null || this.m_TargetRectTransform == null)
            {
                return;
            }

            var mySize = this.m_RectTransform.sizeDelta;
            var targetSize = this.m_TargetRectTransform.sizeDelta;
            var scale = 1f;

            if (mySize.x < targetSize.x || mySize.y < targetSize.y)
            {
                // ターゲットのサイズに合わせてスケーリング係数を計算
                scale = Mathf.Min(targetSize.x / mySize.x, targetSize.y / mySize.y, 1f);
            }

            // スケーリングを適用
            this.m_RectTransform.localScale = Vector3.one * scale;
        }
    }
}