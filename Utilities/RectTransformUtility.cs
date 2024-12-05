using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.Utilities
{
    /// <summary>
    /// RectTransformユーティリティ
    /// </summary>
    public static class RectTransformUtility
    {
        /// <summary>
        /// 矩形の中心のローカル座標を取得
        /// </summary>
        public static Vector2 GetLocalCenter(this RectTransform rectTransform)
        {
            var center = rectTransform.anchoredPosition;
            center.x += rectTransform.sizeDelta.x * (0.5f - rectTransform.pivot.x);
            center.y += rectTransform.sizeDelta.y * (0.5f - rectTransform.pivot.y);
            return center;
        }

        /// <summary>
        /// 矩形の左上のローカル座標を取得
        /// </summary>
        public static Vector2 GetLocalLeftTop(this RectTransform rectTransform)
        {
            var leftTop = rectTransform.anchoredPosition;
            leftTop.x += rectTransform.sizeDelta.x * (0f - rectTransform.pivot.x);
            leftTop.y += rectTransform.sizeDelta.y * (1f - rectTransform.pivot.y);
            return leftTop;
        }

        /// <summary>
        /// 矩形の右下のローカル座標を取得
        /// </summary>
        public static Vector2 GetLocalRightBottom(this RectTransform rectTransform)
        {
            var rightBottom = rectTransform.anchoredPosition;
            rightBottom.x += rectTransform.sizeDelta.x * (1f - rectTransform.pivot.x);
            rightBottom.y += rectTransform.sizeDelta.y * (0f - rectTransform.pivot.y);
            return rightBottom;
        }
    }
}
