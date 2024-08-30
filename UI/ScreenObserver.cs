using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEditor;
using UnityEngine;

namespace MushaLib.UI
{
    /// <summary>
    /// スクリーン監視
    /// </summary>
    public static class ScreenObserver
    {
        /// <summary>
        /// スクリーン情報
        /// </summary>
        private static ReactiveProperty<ScreenInfo> m_ScreenInfo;

        /// <summary>
        /// スクリーン情報
        /// </summary>
        public static IReadOnlyReactiveProperty<ScreenInfo> ScreenInfo => m_ScreenInfo;

        /// <summary>
        /// 幅
        /// </summary>
        public static IReadOnlyReactiveProperty<int> Width { get; private set; }

        /// <summary>
        /// 高さ
        /// </summary>
        public static IReadOnlyReactiveProperty<int> Height { get; private set; }

        /// <summary>
        /// セーフエリア
        /// </summary>
        public static IReadOnlyReactiveProperty<Rect> SafeArea { get; private set; }

        /// <summary>
        /// static construct
        /// </summary>
        static ScreenObserver()
        {
            m_ScreenInfo = new ReactiveProperty<UI.ScreenInfo>(UI.ScreenInfo.GetCurrent());
            Width = m_ScreenInfo.Select(x => x.Width).ToReadOnlyReactiveProperty();
            Height = m_ScreenInfo.Select(x => x.Height).ToReadOnlyReactiveProperty();
            SafeArea = m_ScreenInfo.Select(x => x.SafeArea).ToReadOnlyReactiveProperty();

            // 毎フレームスクリーン情報の更新
            UniTaskAsyncEnumerable
                .EveryUpdate()
#if UNITY_EDITOR
                .Where(_ => EditorApplication.isPlaying)
#endif
                .Subscribe(_ =>
                {
                    UpdateScreenInfo();
                });

#if UNITY_EDITOR
            EditorApplication.update += () =>
            {
                if (!EditorApplication.isPlaying)
                {
                    UpdateScreenInfo();
                }
            };
#endif
        }

        /// <summary>
        /// スクリーン情報の更新
        /// </summary>
        private static void UpdateScreenInfo()
        {
            m_ScreenInfo.Value = UI.ScreenInfo.GetCurrent();
        }
    }
}
