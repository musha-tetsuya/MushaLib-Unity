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
        /// Disposable
        /// </summary>
        private static CompositeDisposable m_Disposable;

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
            m_Disposable?.Dispose();
            m_Disposable = new CompositeDisposable();

            m_ScreenInfo = new ReactiveProperty<UI.ScreenInfo>(UI.ScreenInfo.GetCurrent()).AddTo(m_Disposable);
            Width = m_ScreenInfo.Select(x => x.Width).ToReadOnlyReactiveProperty().AddTo(m_Disposable);
            Height = m_ScreenInfo.Select(x => x.Height).ToReadOnlyReactiveProperty().AddTo(m_Disposable);
            SafeArea = m_ScreenInfo.Select(x => x.SafeArea).ToReadOnlyReactiveProperty().AddTo(m_Disposable);

            // 毎フレームスクリーン情報の更新
            UniTaskAsyncEnumerable
                .EveryUpdate()
#if UNITY_EDITOR
                .Where(_ => EditorApplication.isPlaying)
#endif
                .Subscribe(_ =>
                {
                    UpdateScreenInfo();
                })
                .AddTo(m_Disposable);

#if UNITY_EDITOR
            EditorApplication.update -= EditorUpdate;
            EditorApplication.update += EditorUpdate;
#endif
        }

#if UNITY_EDITOR
        /// <summary>
        /// Editorモード時Update
        /// </summary>
        private static void EditorUpdate()
        {
            if (!EditorApplication.isPlaying)
            {
                UpdateScreenInfo();
            }
        }
#endif

        /// <summary>
        /// スクリーン情報の更新
        /// </summary>
        private static void UpdateScreenInfo()
        {
            m_ScreenInfo.Value = UI.ScreenInfo.GetCurrent();
        }
    }
}
