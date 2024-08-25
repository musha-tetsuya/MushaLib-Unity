using Cysharp.Threading.Tasks.Linq;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace MushaLib.UI
{
    /// <summary>
    /// スクリーン監視
    /// </summary>
    public class ScreenObserver
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        private static ScreenObserver m_Instance;

        /// <summary>
        /// インスタンス
        /// </summary>
        private static ScreenObserver Instance => m_Instance ?? (m_Instance = new ScreenObserver());

        /// <summary>
        /// スクリーン情報
        /// </summary>
        public static IReadOnlyReactiveProperty<ScreenInfo> ScreenInfo => Instance.m_ScreenInfo;

        /// <summary>
        /// 幅
        /// </summary>
        public static IReadOnlyReactiveProperty<int> Width => Instance.m_Width;

        /// <summary>
        /// 高さ
        /// </summary>
        public static IReadOnlyReactiveProperty<int> Height => Instance.m_Height;

        /// <summary>
        /// セーフエリア
        /// </summary>
        public static IReadOnlyReactiveProperty<Rect> SafeArea => Instance.m_SafeArea;

        /// <summary>
        /// Disposable
        /// </summary>
        private CompositeDisposable m_Disposable;

        /// <summary>
        /// スクリーン情報
        /// </summary>
        private ReactiveProperty<ScreenInfo> m_ScreenInfo;

        /// <summary>
        /// 幅
        /// </summary>
        private ReadOnlyReactiveProperty<int> m_Width;

        /// <summary>
        /// 高さ
        /// </summary>
        private ReadOnlyReactiveProperty<int> m_Height;

        /// <summary>
        /// セーフエリア
        /// </summary>
        private ReadOnlyReactiveProperty<Rect> m_SafeArea;

        /// <summary>
        /// construct
        /// </summary>
        private ScreenObserver()
        {
            m_Disposable = new CompositeDisposable();
            m_ScreenInfo = new ReactiveProperty<UI.ScreenInfo>(UI.ScreenInfo.GetCurrent()).AddTo(m_Disposable);
            m_Width = m_ScreenInfo.Select(x => x.Width).ToReadOnlyReactiveProperty().AddTo(m_Disposable);
            m_Height = m_ScreenInfo.Select(x => x.Height).ToReadOnlyReactiveProperty().AddTo(m_Disposable);
            m_SafeArea = m_ScreenInfo.Select(x => x.SafeArea).ToReadOnlyReactiveProperty().AddTo(m_Disposable);

            // 毎フレームスクリーン情報の更新
            UniTaskAsyncEnumerable
                .EveryUpdate()
                .Subscribe(_ =>
                {
                    m_ScreenInfo.Value = UI.ScreenInfo.GetCurrent();
                })
                .AddTo(m_Disposable);
        }

        /// <summary>
        /// destruct
        /// </summary>
        ~ScreenObserver()
        {
            m_Disposable.Dispose();
        }
    }
}
