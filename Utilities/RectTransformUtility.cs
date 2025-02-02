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
        /// 指定したピボットを基準としたアンカー座標のオフセットを取得する
        /// </summary>
        public static Vector2 GetAnchoredOffset(this RectTransform rectTransform, Vector2 pivot)
        {
            return new(rectTransform.sizeDelta.x * (pivot.x - rectTransform.pivot.x), rectTransform.sizeDelta.y * (pivot.y - rectTransform.pivot.y));
        }

        /// <summary>
        /// アンカーと座標を設定する
        /// </summary>
        public static void SetAnchorAndPosition(this RectTransform rectTransform, Vector2 anchor, Vector2 position)
        {
            rectTransform.anchorMin =
            rectTransform.anchorMax =
            rectTransform.pivot = anchor;
            rectTransform.anchoredPosition = position;
        }
    }
}
