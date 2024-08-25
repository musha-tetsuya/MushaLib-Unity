using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace MushaLib.UI
{
    /// <summary>
    /// スクリーン情報
    /// </summary>
    public struct ScreenInfo
    {
        /// <summary>
        /// 幅
        /// </summary>
        public int Width { get; set; }

        /// <summary>
        /// 高さ
        /// </summary>
        public int Height { get; set; }

        /// <summary>
        /// セーフエリア
        /// </summary>
        public Rect SafeArea { get; set; }

        /// <summary>
        /// Equals
        /// </summary>
        public override bool Equals(object obj)
        {
            return obj is ScreenInfo other
                && other.Width == Width
                && other.Height == Height
                && other.SafeArea == SafeArea;
        }

        /// <summary>
        /// GetHashCode
        /// </summary>
        public override int GetHashCode()
        {
            return HashCode.Combine(Width, Height, SafeArea);
        }

        /// <summary>
        ///現在のスクリーン情報を取得 
        /// </summary>
        public static ScreenInfo GetCurrent()
        {
            var screenWidth = Screen.width;
            var screenHeight = Screen.height;
            var safeArea = Screen.safeArea;

#if UNITY_EDITOR
            var screenRes = UnityStats.screenRes.Split('x');
            screenWidth = int.Parse(screenRes[0]);
            screenHeight = int.Parse(screenRes[1]);

            if (!UnityEngine.Device.Application.isMobilePlatform)
            {
                safeArea.x = 0f;
                safeArea.y = 0f;
                safeArea.width = screenWidth;
                safeArea.height = screenHeight;
            }
#endif

            screenWidth = Mathf.Max(screenWidth, 1);
            screenHeight = Mathf.Max(screenHeight, 1);
            safeArea.width = Mathf.Max(safeArea.width, 1f);
            safeArea.height = Mathf.Max(safeArea.height, 1f);

            return new ScreenInfo { Width = screenWidth, Height = screenHeight, SafeArea = safeArea };
        }
    }
}
