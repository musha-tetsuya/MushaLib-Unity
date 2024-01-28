using Cysharp.Threading.Tasks;
using Cysharp.Threading.Tasks.Linq;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UniRx;
using UniRx.Triggers;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace MushaLib.InfiniteScrollView
{
    /// <summary>
    /// 無限スクロールビュー
    /// </summary>
    [RequireComponent(typeof(ScrollRect))]
    public class InfiniteScrollView : MonoBehaviour, IEventSystemHandler, IInitializePotentialDragHandler, IEndDragHandler
    {
        /// <summary>
        /// ScrollRect
        /// </summary>
        [SerializeField]
        private ScrollRect m_ScrollRect;

        /// <summary>
        /// 要素プレハブ
        /// </summary>
        [SerializeField]
        [Header("要素プレハブ")]
        private ScrollElement m_ElementPrefab;

        /// <summary>
        /// 要素数
        /// </summary>
        [SerializeField]
        [Header("要素数")]
        private int m_ElementCount;

        /// <summary>
        /// ページレイアウト
        /// </summary>
        [SerializeField]
        [Header("ページレイアウト")]
        private PageLayout m_PageLayout;

        /// <summary>
        /// ページ間スペース
        /// </summary>
        [SerializeField]
        [Header("ページ間スペース")]
        private Vector2 m_Spacing;

        /// <summary>
        /// ページの配置方向
        /// </summary>
        [SerializeField]
        [Header("ページの配置方向")]
        private GridLayoutGroup.Axis m_StartAxis;

        /// <summary>
        /// Viewport内側の余白
        /// </summary>
        [SerializeField]
        [Header("Viewport内側の余白")]
        private RectOffset m_Padding;

        /// <summary>
        /// Viewport外側の余白
        /// </summary>
        [SerializeField]
        [Header("Viewport外側の余白")]
        private RectOffset m_Margin;

        /// <summary>
        /// ページをループさせるかどうか
        /// </summary>
        [SerializeField]
        [Header("ページをループさせるかどうか")]
        private bool m_Loop;

        /// <summary>
        /// スナップタイプ
        /// </summary>
        [SerializeField]
        [Header("スナップタイプ")]
        private SnapType m_SnapType;

        /// <summary>
        /// スナップ時間
        /// </summary>
        [SerializeField]
        [Header("スナップ時間")]
        private float m_SnapDuration = 0.1f;

        /// <summary>
        /// 要素プレハブ
        /// </summary>
        private ScrollElement m_CurrentElementPrefab;

        /// <summary>
        /// ページ矩形サイズ
        /// </summary>
        private Vector2 m_PageRectSize;

        /// <summary>
        /// 縦横ページ数
        /// </summary>
        private Vector2Int m_PageLength;

        /// <summary>
        /// ページインデックス増加量
        /// </summary>
        private Vector2Int m_PageIndexDelta;

        /// <summary>
        /// Viewport矩形の最小位置
        /// </summary>
        private Vector2 m_ViewportCornerMin;

        /// <summary>
        /// Viewport矩形の最大位置
        /// </summary>
        private Vector2 m_ViewportCornerMax;

        /// <summary>
        /// スクロール要素配列長
        /// </summary>
        private Vector2Int m_ScrollElementLength;

        /// <summary>
        /// スクロール要素配列
        /// </summary>
        private IScrollElement[,] m_ScrollElements;

        /// <summary>
        /// スクロール要素配列の横軸のインデックスを格納した配列
        /// </summary>
        private int[] m_OriginalColumnIndices;

        /// <summary>
        /// スクロール要素配列の縦軸のインデックスを格納した配列
        /// </summary>
        private int[] m_OriginalRowIndices;

        /// <summary>
        /// スクロール要素配列の横軸のインデックスを逆順に格納した配列
        /// </summary>
        private int[] m_ReverseColumnIndices;

        /// <summary>
        /// スクロール要素配列の縦軸のインデックスを逆順に格納した配列
        /// </summary>
        private int[] m_ReverseRowIndices;

        /// <summary>
        /// content.anchoredPositionの初期値
        /// </summary>
        private Vector2 m_DefaultContentAnchoredPosition;

        /// <summary>
        /// content.anchoredPositionの前回の値
        /// </summary>
        private Vector2 m_PrevContentAnchoredPosition;

        /// <summary>
        /// Viewportから見たcontentの左上角の座標
        /// </summary>
        private Vector2 m_ContentTopLeftInViewport;

        /// <summary>
        /// 自動スクロール処理のキャンセルトークン
        /// </summary>
        private CancellationTokenSource m_AutoScrollCancellationTokenSource;

        /// <summary>
        /// 総ページ数
        /// </summary>
        private int TotalPageLength => m_PageLength.x * m_PageLength.y;

        /// <summary>
        /// 要素プレハブ
        /// </summary>
        public ScrollElement ElementPrefab
        {
            get => m_ElementPrefab;
            set => m_ElementPrefab = value;
        }

        /// <summary>
        /// 要素数
        /// </summary>
        public int ElementCount
        {
            get => m_ElementCount;
            set => m_ElementCount = value;
        }

        /// <summary>
        /// ページレイアウト
        /// </summary>
        public PageLayout PageLayout
        {
            get => m_PageLayout;
            set => m_PageLayout = value;
        }

        /// <summary>
        /// ページ間スペース
        /// </summary>
        public Vector2 Spacing
        {
            get => m_Spacing;
            set => m_Spacing = value;
        }

        /// <summary>
        /// ページの配置方向
        /// </summary>
        public GridLayoutGroup.Axis StartAxis
        {
            get => m_StartAxis;
            set => m_StartAxis = value;
        }

        /// <summary>
        /// Viewport内側の余白
        /// </summary>
        public RectOffset Padding
        {
            get => m_Padding;
            set => m_Padding = value;
        }

        /// <summary>
        /// Viewport外側の余白
        /// </summary>
        public RectOffset Margin
        {
            get => m_Margin;
            set => m_Margin = value;
        }

        /// <summary>
        /// ページをループさせるかどうか
        /// </summary>
        public bool Loop
        {
            get => m_Loop;
            set => m_Loop = value;
        }

        /// <summary>
        /// スナップタイプ
        /// </summary>
        public SnapType SnapType
        {
            get => m_SnapType;
            set => m_SnapType = value;
        }

        /// <summary>
        /// スナップ時間
        /// </summary>
        public float SnapDuration
        {
            get => m_SnapDuration;
            set => m_SnapDuration = value;
        }

        /// <summary>
        /// 要素更新時イベント
        /// </summary>
        public Action<IScrollElement, int> OnUpdateElement { get; set; }

        /// <summary>
        /// Reset
        /// </summary>
        private void Reset()
        {
            m_ScrollRect = GetComponent<ScrollRect>();
        }

        /// <summary>
        /// Awake
        /// </summary>
        protected virtual void Awake()
        {
            // スクロール時イベント購読
            m_ScrollRect.onValueChanged.AddListener(OnScroll);

            if (m_ScrollRect.horizontalScrollbar != null)
            {
                // 横スクロールバー操作時
                Observable
                    .Merge(
                        m_ScrollRect.horizontalScrollbar.OnInitializePotentialDragAsObservable().Where(eventData => eventData.rawPointerPress != m_ScrollRect.horizontalScrollbar.handleRect.gameObject),
                        m_ScrollRect.horizontalScrollbar.OnBeginDragAsObservable())
                    .Where(eventData => eventData.button == PointerEventData.InputButton.Left)
                    .Subscribe(_ => CancelAutoScroll())
                    .AddTo(destroyCancellationToken);
            }

            if (m_ScrollRect.verticalScrollbar != null)
            {
                // 縦スクロールバー操作時
                Observable
                    .Merge(
                        m_ScrollRect.verticalScrollbar.OnInitializePotentialDragAsObservable().Where(eventData => eventData.rawPointerPress != m_ScrollRect.verticalScrollbar.handleRect.gameObject),
                        m_ScrollRect.verticalScrollbar.OnBeginDragAsObservable())
                    .Where(eventData => eventData.button == PointerEventData.InputButton.Left)
                    .Subscribe(_ => CancelAutoScroll())
                    .AddTo(destroyCancellationToken);
            }
        }

        /// <summary>
        /// 初期化
        /// </summary>
        public void Initialize()
        {
            if (m_ElementPrefab == null)
            {
                Debug.LogError("要素プレハブが設定されていません。", this);
                return;
            }

            if (m_ElementCount < 0)
            {
                Debug.LogError("要素数が0未満です。", this);
                return;
            }

            if (m_PageLayout.CellCount.x <= 0 || m_PageLayout.CellCount.y <= 0)
            {
                Debug.LogError("ページ内のセル数が0以下です。", this);
                return;
            }

            // リサイクル可能要素プレハブ
            Queue<IScrollElement> recyclablePrefabs = m_ScrollElements == null ? new() : new(m_ScrollElements.Cast<IScrollElement>());

            // 要素プレハブが変更された
            if (m_ElementPrefab != m_CurrentElementPrefab)
            {
                m_CurrentElementPrefab = m_ElementPrefab;

                // 既存要素のクリア
                while (recyclablePrefabs.TryDequeue(out var prefabInstance))
                {
                    Destroy(prefabInstance.RectTransform.gameObject);
                }
            }

            // ページセルサイズ設定
            m_PageLayout.CellSize = (m_CurrentElementPrefab.transform as RectTransform).rect.size * m_CurrentElementPrefab.transform.localScale;

            // ページの縦横幅
            m_PageRectSize.x = (m_PageLayout.CellSize.x + m_PageLayout.Spacing.x) * m_PageLayout.CellCount.x - m_PageLayout.Spacing.x;
            m_PageRectSize.y = (m_PageLayout.CellSize.y + m_PageLayout.Spacing.y) * m_PageLayout.CellCount.y - m_PageLayout.Spacing.y;

            // 必要ページ数
            m_PageLength = Vector2Int.one * Mathf.Max(Mathf.CeilToInt((float)m_ElementCount / m_PageLayout.TotalCellCount), 1);

            // ページインデックス変化量
            m_PageIndexDelta = Vector2Int.one;

            if (m_ScrollRect.horizontal && !m_ScrollRect.vertical)
            {
                m_PageLength.y = 1;
            }
            else if (m_ScrollRect.vertical && !m_ScrollRect.horizontal)
            {
                m_PageLength.x = 1;
            }
            else if (m_StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                m_PageLength.x = Mathf.CeilToInt(Mathf.Sqrt(m_PageLength.x));
                m_PageLength.y = Mathf.CeilToInt((float)m_PageLength.y / m_PageLength.x);
                m_PageIndexDelta.y = m_PageLength.x;
            }
            else
            {
                m_PageLength.y = Mathf.CeilToInt(Mathf.Sqrt(m_PageLength.y));
                m_PageLength.x = Mathf.CeilToInt((float)m_PageLength.x / m_PageLength.y);
                m_PageIndexDelta.x = m_PageLength.y;
            }

            // AutoHideAndExpandViewportの場合、contentサイズが決まらないとviewportサイズが取得出来ない。
            // contentサイズ決定後、Layoutをリビルドしてviewportサイズを決定する。
            // そのため、一旦contentのanchorMinとanchorMaxを揃える。
            var contentAnchorMax = m_ScrollRect.content.anchorMax;
            m_ScrollRect.content.anchorMax = m_ScrollRect.content.anchorMin;

            // contentサイズ決定
            m_ScrollRect.content.sizeDelta = new(
                m_Padding.left + m_Padding.right + (m_PageRectSize.x + m_Spacing.x) * m_PageLength.x - m_Spacing.x,
                m_Padding.top + m_Padding.bottom + (m_PageRectSize.y + m_Spacing.y) * m_PageLength.y - m_Spacing.y
            );

            // Layoutをリビルドしてviewportサイズを決定
            if ((m_ScrollRect.horizontalScrollbar != null && m_ScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport) ||
                (m_ScrollRect.verticalScrollbar != null && m_ScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_ScrollRect.transform as RectTransform);
            }

            // contentのanchorMaxを元に戻す
            m_ScrollRect.content.anchorMax = contentAnchorMax;

            // スクロール方向に合わせてanchorを調整
            if (m_ScrollRect.horizontal)
            {
                m_ScrollRect.content.anchorMin = new(0f, m_ScrollRect.content.anchorMin.y);
                m_ScrollRect.content.anchorMax = new(0f, m_ScrollRect.content.anchorMax.y);
            }
            if (m_ScrollRect.vertical)
            {
                m_ScrollRect.content.anchorMin = new(m_ScrollRect.content.anchorMin.x, 1f);
                m_ScrollRect.content.anchorMax = new(m_ScrollRect.content.anchorMax.x, 1f);
            }

            // anchorに合わせてcontentサイズを調整
            m_ScrollRect.content.sizeDelta -= (m_ScrollRect.content.anchorMax - m_ScrollRect.content.anchorMin) * m_ScrollRect.viewport.rect.size;

            // PrevContentPositionをViewport + Marginの位置に合わせる
            var contentAnchor = m_ScrollRect.content.anchorMin + (m_ScrollRect.content.anchorMax - m_ScrollRect.content.anchorMin) * 0.5f;
            m_PrevContentAnchoredPosition.x = -m_Padding.left - m_Margin.left;
            m_PrevContentAnchoredPosition.y = m_Padding.top + m_Margin.top;
            m_PrevContentAnchoredPosition.x += m_ScrollRect.content.rect.width * m_ScrollRect.content.pivot.x - m_ScrollRect.viewport.rect.width * contentAnchor.x;
            m_PrevContentAnchoredPosition.y += -m_ScrollRect.content.rect.height * (1f - m_ScrollRect.content.pivot.y) + m_ScrollRect.viewport.rect.height * (1f - contentAnchor.y);

            // contentの初期anchoredPositionを決定
            m_DefaultContentAnchoredPosition = Vector2.zero;
            if (m_ScrollRect.horizontal)
            {
                if (m_ScrollRect.content.rect.width < m_ScrollRect.viewport.rect.width)
                {
                    m_DefaultContentAnchoredPosition.x = m_ScrollRect.viewport.rect.width * (m_ScrollRect.content.pivot.x - m_ScrollRect.content.anchorMin.x);
                }
                else
                {
                    m_DefaultContentAnchoredPosition.x = m_PrevContentAnchoredPosition.x + m_Margin.left + m_Padding.left;
                }
            }
            if (m_ScrollRect.vertical)
            {
                if (m_ScrollRect.content.rect.height < m_ScrollRect.viewport.rect.height)
                {
                    m_DefaultContentAnchoredPosition.y = m_ScrollRect.viewport.rect.height * (m_ScrollRect.content.pivot.y - m_ScrollRect.content.anchorMin.y);
                }
                else
                {
                    m_DefaultContentAnchoredPosition.y = m_PrevContentAnchoredPosition.y - m_Margin.top - m_Padding.top;
                }
            }

            // viewport取得
            var viewportCorners = new Vector3[4];
            m_ScrollRect.viewport.GetLocalCorners(viewportCorners);
            m_ViewportCornerMin = viewportCorners[0] - new Vector3(m_Margin.left, m_Margin.bottom);
            m_ViewportCornerMax = viewportCorners[2] + new Vector3(m_Margin.right, m_Margin.top);

            // ScrollElement配列長
            m_ScrollElementLength = m_PageLayout.CellCount;
            if (m_ScrollRect.horizontal)
            {
                // viewport範囲内に要素がいくつ入るか、viewport位置を少しずらしてチェックする
                float viewportMaxX = (m_ViewportCornerMax.x - m_ViewportCornerMin.x) + (m_PageLayout.CellSize.x - 1f);
                float fPageColumn = viewportMaxX / (m_PageRectSize.x + m_Spacing.x);
                int pageColumn = Mathf.FloorToInt(fPageColumn);
                float fLocalColumn = (fPageColumn - pageColumn) * (m_PageRectSize.x + m_Spacing.x) / (m_PageLayout.CellSize.x + m_PageLayout.Spacing.x);
                int localColumn = Mathf.CeilToInt(fLocalColumn);
                m_ScrollElementLength.x = pageColumn * m_PageLayout.CellCount.x + localColumn;
            }
            if (m_ScrollRect.vertical)
            {
                // viewport範囲内に要素がいくつ入るか、viewport位置を少しずらしてチェックする
                float viewportMaxY = (m_ViewportCornerMax.y - m_ViewportCornerMin.y) + (m_PageLayout.CellSize.y - 1f);
                float fPageRow = viewportMaxY / (m_PageRectSize.y + m_Spacing.y);
                int pageRow = Mathf.FloorToInt(fPageRow);
                float fLocalRow = (fPageRow - pageRow) * (m_PageRectSize.y + m_Spacing.y) / (m_PageLayout.CellSize.y + m_PageLayout.Spacing.y);
                int localRow = Mathf.CeilToInt(fLocalRow);
                m_ScrollElementLength.y = pageRow* m_PageLayout.CellCount.y + localRow;
            }

            // ScrollElement配列生成
            m_ScrollElements = new IScrollElement[m_ScrollElementLength.y, m_ScrollElementLength.x];
            m_OriginalColumnIndices = Enumerable.Range(0, m_ScrollElementLength.x).ToArray();
            m_OriginalRowIndices = Enumerable.Range(0, m_ScrollElementLength.y).ToArray();
            m_ReverseColumnIndices = m_OriginalColumnIndices.Reverse().ToArray();
            m_ReverseRowIndices = m_OriginalRowIndices.Reverse().ToArray();

            for (int y = 0; y < m_ScrollElementLength.y; y++)
            {
                for (int x = 0; x < m_ScrollElementLength.x; x++)
                {
                    if (!recyclablePrefabs.TryDequeue(out var prefabInstance))
                    {
                        prefabInstance = Instantiate(m_CurrentElementPrefab, m_ScrollRect.content);
                    }

                    var element = m_ScrollElements[y, x] = prefabInstance;
                    SetElementColumn(element, x);
                    SetElementRow(element, y);

                    element.LocalPosition = GetElementPosition(element.PageRow, element.PageColumn, element.LocalRow, element.LocalColumn);

                    var anchoredPosition = element.LocalPosition;
                    anchoredPosition.x += m_Padding.left + m_PageLayout.CellSize.x * element.RectTransform.pivot.x;
                    anchoredPosition.y -= m_Padding.top + m_PageLayout.CellSize.y * (1f - element.RectTransform.pivot.y);

                    element.RectTransform.anchorMin =
                    element.RectTransform.anchorMax = new(0f, 1f);
                    element.RectTransform.anchoredPosition = anchoredPosition;

                    UpdateElementIndex(element);
                }
            }

            // 使わなかったリサイクル要素を破棄
            while (recyclablePrefabs.TryDequeue(out var prefabInstance))
            {
                Destroy(prefabInstance.RectTransform.gameObject);
            }

            // contentのanchoredPositionをセットするとScrollRectのonValueChangedが走るので、一旦enabledを切る
            var prevScrollRectEnabled = m_ScrollRect.enabled;
            m_ScrollRect.enabled = false;

            // PrevContentAnchoredPositionから(0, 0)までスクロールしたということにして要素を更新
            m_ScrollRect.content.anchoredPosition = m_DefaultContentAnchoredPosition;
            OnScroll(default);

            // enabledを戻す
            m_ScrollRect.enabled = prevScrollRectEnabled;
        }

        /// <summary>
        /// スクロール時
        /// </summary>
        private void OnScroll(Vector2 value)
        {
            if (m_ElementCount <= 0 || m_ScrollElements == null)
            {
                return;
            }

            // スクロール量
            Vector2 delta = m_ScrollRect.content.anchoredPosition - m_PrevContentAnchoredPosition;
            m_PrevContentAnchoredPosition = m_ScrollRect.content.anchoredPosition;

            // Viewportから見たcontentの左上角の座標
            m_ContentTopLeftInViewport = m_ScrollRect.content.localPosition;
            m_ContentTopLeftInViewport.x -= m_ScrollRect.content.rect.width * m_ScrollRect.content.pivot.x;
            m_ContentTopLeftInViewport.y += m_ScrollRect.content.rect.height * m_ScrollRect.content.pivot.y;

            // 横スクロール時
            if (m_ScrollRect.horizontal && !Mathf.Approximately(delta.x, 0f))
            {
                // スクロールを進ませた？
                int sign = delta.x < 0f ? 1 : -1;
                int[] columnIndices = delta.x < 0f ? m_OriginalColumnIndices : m_ReverseColumnIndices;

                int firstX = columnIndices.First();
                int lastX = columnIndices.Last();

                // viewport範囲外になった？
                while (!OverlapHorizontal(m_ScrollElements[0, firstX]))
                {
                    int moveCount = sign;
                    Vector2? movePosition = null;

                    for (int y = 0; y < m_ScrollElementLength.y; y++)
                    {
                        var firstElement = m_ScrollElements[y, firstX];
                        var lastElement = m_ScrollElements[y, lastX];

                        // 最終要素の次の位置に移動
                        SetElementColumn(firstElement, lastElement.Column + moveCount);

                        while (!movePosition.HasValue)
                        {
                            // 移動後のローカル座標
                            var newLocalPosition = GetElementPosition(firstElement.PageRow, firstElement.PageColumn, firstElement.LocalRow, firstElement.LocalColumn);
                            float minX = m_ContentTopLeftInViewport.x + m_Padding.left + newLocalPosition.x;
                            float maxX = minX + m_PageLayout.CellSize.x;

                            // 移動の結果viewport範囲内になりそう？
                            if ((sign == 1 && m_ViewportCornerMin.x <= maxX) || (sign == -1 && minX <= m_ViewportCornerMax.x))
                            {
                                // 座標移動量決定
                                movePosition = newLocalPosition - firstElement.LocalPosition;
                            }
                            else
                            {
                                // もう一つ隣に移動してみる
                                moveCount += sign;
                                SetElementColumn(firstElement, lastElement.Column + moveCount);
                            }
                        }

                        // 座標移動
                        firstElement.LocalPosition += movePosition.Value;
                        firstElement.RectTransform.anchoredPosition += movePosition.Value;

                        // インデックス更新
                        UpdateElementIndex(firstElement);

                        // スクロール要素配列をシフト
                        foreach (int x in columnIndices.SkipLast(1))
                        {
                            m_ScrollElements[y, x] = m_ScrollElements[y, x + sign];
                        }

                        m_ScrollElements[y, lastX] = firstElement;
                    }
                }
            }

            // 縦スクロール時
            if (m_ScrollRect.vertical && !Mathf.Approximately(delta.y, 0f))
            {
                // スクロールを進ませた？
                int sign = delta.y > 0f ? 1 : -1;
                int[] rowIndices = delta.y > 0f ? m_OriginalRowIndices : m_ReverseRowIndices;

                int firstY = rowIndices.First();
                int lastY = rowIndices.Last();

                // viewport範囲外になった？
                while (!OverlapVertical(m_ScrollElements[firstY, 0]))
                {
                    int moveCount = sign;
                    Vector2? movePosition = null;

                    for (int x = 0; x < m_ScrollElementLength.x; x++)
                    {
                        var firstElement = m_ScrollElements[firstY, x];
                        var lastElement = m_ScrollElements[lastY, x];

                        // 最終要素の次の位置に移動
                        SetElementRow(firstElement, lastElement.Row + moveCount);

                        while (!movePosition.HasValue)
                        {
                            // 移動後のローカル座標
                            var newLocalPosition = GetElementPosition(firstElement.PageRow, firstElement.PageColumn, firstElement.LocalRow, firstElement.LocalColumn);
                            float maxY = m_ContentTopLeftInViewport.y - m_Padding.top + newLocalPosition.y;
                            float minY = maxY - m_PageLayout.CellSize.y;

                            // 移動の結果viewport範囲内になりそう？
                            if ((sign == 1 && minY <= m_ViewportCornerMax.y) || (sign == -1 && m_ViewportCornerMin.y <= maxY))
                            {
                                // 座標移動量決定
                                movePosition = newLocalPosition - firstElement.LocalPosition;
                            }
                            else
                            {
                                // もう一つ隣に移動してみる
                                moveCount += sign;
                                SetElementRow(firstElement, lastElement.Row + moveCount);
                            }
                        }

                        // 座標移動
                        firstElement.LocalPosition += movePosition.Value;
                        firstElement.RectTransform.anchoredPosition += movePosition.Value;

                        // インデックス更新
                        UpdateElementIndex(firstElement);

                        // スクロール要素配列をシフト
                        foreach (int y in rowIndices.SkipLast(1))
                        {
                            m_ScrollElements[y, x] = m_ScrollElements[y + sign, x];
                        }

                        m_ScrollElements[lastY, x] = firstElement;
                    }
                }
            }
        }

        /// <summary>
        /// ドラッグ開始時
        /// </summary>
        void IInitializePotentialDragHandler.OnInitializePotentialDrag(PointerEventData eventData)
        {
            if (eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            CancelAutoScroll();
        }

        /// <summary>
        /// ドラッグ終了時
        /// </summary>
        async void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (m_ElementCount <= 0 || m_SnapType == SnapType.None || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            // スナップ処理の開始、終了時のcontentのanchoredPosition
            Vector2 startAnchoredPosition = m_ScrollRect.content.anchoredPosition;
            Vector2 endAnchoredPosition = startAnchoredPosition;

            // 横スクロールする場合
            if (m_ScrollRect.horizontal)
            {
                // 余白を除いたコンテンツ幅
                float contentWidth = m_ScrollRect.content.rect.width - (m_Padding.left + m_Padding.right);

                // contentの座標
                float contentPositionX = Mathf.Repeat(-(m_ScrollRect.content.anchoredPosition.x - m_DefaultContentAnchoredPosition.x), contentWidth + m_Spacing.x);

                // contentの位置から現在ターゲット中の要素を割り出す
                float fPageColumn = contentPositionX / (m_PageRectSize.x + m_Spacing.x);
                int pageColumn = Mathf.FloorToInt(fPageColumn);
                float fLocalColumn = (fPageColumn - pageColumn) * (m_PageRectSize.x + m_Spacing.x) / (m_PageLayout.CellSize.x + m_PageLayout.Spacing.x);
                int localColumn = Mathf.FloorToInt(fLocalColumn);

                // 場合によっては隣の要素の方が近いかもしれないので、比較する要素を決定
                int nextPageColumn = pageColumn + 1;
                int nextLocalColumn = 0;

                if (localColumn < 0)
                {
                    localColumn = 0;
                    nextPageColumn = pageColumn - 1;
                    nextLocalColumn = m_SnapType == SnapType.Element ? m_PageLayout.CellCount.x - 1 : 0;
                }
                else if (localColumn >= m_PageLayout.CellCount.x)
                {
                    localColumn = m_SnapType == SnapType.Element ? m_PageLayout.CellCount.x - 1 : 0;
                    nextPageColumn = pageColumn + 1;
                    nextLocalColumn = 0;
                }
                else if (m_SnapType == SnapType.Element)
                {
                    nextPageColumn = pageColumn + (localColumn + 1) / m_PageLayout.CellCount.x;
                    nextLocalColumn = (localColumn + 1) % m_PageLayout.CellCount.x;
                }
                else
                {
                    localColumn = 0;
                }

                // スナップターゲット位置決定
                float targetPositionX = GetElementPosition(0, pageColumn, 0, localColumn).x;

                // 隣のターゲットの方が近いなら、スナップターゲット位置は一つ隣にする
                float nextTargetPositionX = GetElementPosition(0, nextPageColumn, 0, nextLocalColumn).x;

                if (Mathf.Abs(nextTargetPositionX - contentPositionX) < Mathf.Abs(targetPositionX - contentPositionX))
                {
                    targetPositionX = nextTargetPositionX;
                }

                // 現在位置からターゲットまでの移動量
                float dx = targetPositionX - contentPositionX;

                // スナップ終了時のcontentのanchoredPositionを決定
                endAnchoredPosition.x -= dx;

                if (m_ScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
                {
                    // 移動制限
                    float maxX = m_DefaultContentAnchoredPosition.x;
                    float minX = maxX - Mathf.Max(m_ScrollRect.content.rect.width - m_ScrollRect.viewport.rect.width, 0f);
                    endAnchoredPosition.x = Mathf.Clamp(endAnchoredPosition.x, minX, maxX);
                }
            }

            // 縦スクロールする場合
            if (m_ScrollRect.vertical)
            {
                // 余白を除いたコンテンツ高さ
                float contentHeight = m_ScrollRect.content.rect.height - (m_Padding.top + m_Padding.bottom);

                // contentの座標
                float contentPositionY = Mathf.Repeat(m_ScrollRect.content.anchoredPosition.y - m_DefaultContentAnchoredPosition.y, contentHeight + m_Spacing.y);

                // contentの位置から現在ターゲット中の要素を割り出す
                float fPageRow = contentPositionY / (m_PageRectSize.y + m_Spacing.y);
                int pageRow = Mathf.FloorToInt(fPageRow);
                float fLocalRow = (fPageRow - pageRow) * (m_PageRectSize.y + m_Spacing.y) / (m_PageLayout.CellSize.y + m_PageLayout.Spacing.y);
                int localRow = Mathf.FloorToInt(fLocalRow);

                // 場合によっては隣の要素の方が近いかもしれないので、比較する要素を決定
                int nextPageRow = pageRow + 1;
                int nextLocalRow = 0;

                if (localRow < 0)
                {
                    localRow = 0;
                    nextPageRow = pageRow - 1;
                    nextLocalRow = m_SnapType == SnapType.Element ? m_PageLayout.CellCount.y - 1 : 0;
                }
                else if (localRow >= m_PageLayout.CellCount.y)
                {
                    localRow = m_SnapType == SnapType.Element ? m_PageLayout.CellCount.y - 1 : 0;
                    nextPageRow = pageRow + 1;
                    nextLocalRow = 0;
                }
                else if (m_SnapType == SnapType.Element)
                {
                    nextPageRow = pageRow + (localRow + 1) / m_PageLayout.CellCount.y;
                    nextLocalRow = (localRow + 1) % m_PageLayout.CellCount.y;
                }
                else
                {
                    localRow = 0;
                }

                // スナップターゲット位置決定
                float targetPositionY = GetElementPosition(pageRow, 0, localRow, 0).y;

                // 隣のターゲットの方が近いなら、スナップターゲット位置は一つ隣にする
                float nextTargetPositionY = GetElementPosition(nextPageRow, 0, nextLocalRow, 0).y;

                if (Mathf.Abs(nextTargetPositionY + contentPositionY) < Mathf.Abs(targetPositionY + contentPositionY))
                {
                    targetPositionY = nextTargetPositionY;
                }

                // 現在位置からターゲットまでの移動量
                float dy = targetPositionY + contentPositionY;

                // スナップ終了時のcontentのanchoredPositionを決定
                endAnchoredPosition.y -= dy;

                if (m_ScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
                {
                    // 移動制限
                    float minY = m_DefaultContentAnchoredPosition.y;
                    float maxY = minY + Mathf.Max(m_ScrollRect.content.rect.height - m_ScrollRect.viewport.rect.height, 0f);
                    endAnchoredPosition.y = Mathf.Clamp(endAnchoredPosition.y, minY, maxY);
                }
            }

            // スナップターゲット位置まで徐々に移動
            await ScrollToPosition(endAnchoredPosition, m_SnapDuration);
        }

        /// <summary>
        /// 要素の列番号をセットする
        /// </summary>
        private void SetElementColumn(IScrollElement element, int column)
        {
            element.Column = column;

            element.LocalColumn = (int)Mathf.Repeat(column, m_PageLayout.CellCount.x);

            if (column >= 0)
            {
                element.PageColumn = column / m_PageLayout.CellCount.x;
            }
            else
            {
                element.PageColumn = (column + 1) / m_PageLayout.CellCount.x - 1;
            }
        }

        /// <summary>
        /// 要素の行番号をセットする
        /// </summary>
        private void SetElementRow(IScrollElement element, int row)
        {
            element.Row = row;

            element.LocalRow = (int)Mathf.Repeat(row, m_PageLayout.CellCount.y);

            if (row >= 0)
            {
                element.PageRow = row / m_PageLayout.CellCount.y;
            }
            else
            {
                element.PageRow = (row + 1) / m_PageLayout.CellCount.y - 1;
            }
        }

        /// <summary>
        /// 要素の行と列からインデックスを更新
        /// </summary>
        private void UpdateElementIndex(IScrollElement element)
        {
            element.LocalIndex = m_PageLayout.GetCellIndex(element.LocalRow, element.LocalColumn);

            element.PageIndex = m_PageIndexDelta.x * (int)Mathf.Repeat(element.PageColumn, m_PageLength.x) + m_PageIndexDelta.y * (int)Mathf.Repeat(element.PageRow, m_PageLength.y);

            element.Index = m_PageLayout.TotalCellCount * element.PageIndex + element.LocalIndex;

            bool isActiveElement = false;

            if (0 <= element.Index && element.Index < m_ElementCount)
            {
                if (m_Loop)
                {
                    isActiveElement = true;
                }
                else if (0 <= element.PageColumn && element.PageColumn < m_PageLength.x && 0 <= element.PageRow && element.PageRow < m_PageLength.y)
                {
                    isActiveElement = true;
                }
            }

            element.RectTransform.gameObject.SetActive(isActiveElement);

            if (isActiveElement)
            {
                OnUpdateElement?.Invoke(element, element.Index);
            }
        }

        /// <summary>
        /// 要素がViewport幅の範囲内かどうか
        /// </summary>
        private bool OverlapHorizontal(IScrollElement element)
        {
            float minX = m_ContentTopLeftInViewport.x + m_Padding.left + element.LocalPosition.x;
            float maxX = minX + m_PageLayout.CellSize.x;

            return (m_ViewportCornerMin.x <= minX && minX <= m_ViewportCornerMax.x)
                || (m_ViewportCornerMin.x <= maxX && maxX <= m_ViewportCornerMax.x);
        }

        /// <summary>
        /// 要素がViewport高さの範囲内かどうか
        /// </summary>
        private bool OverlapVertical(IScrollElement element)
        {
            float maxY = m_ContentTopLeftInViewport.y - m_Padding.top + element.LocalPosition.y;
            float minY = maxY - m_PageLayout.CellSize.y;

            return (m_ViewportCornerMin.y <= minY && minY <= m_ViewportCornerMax.y)
                || (m_ViewportCornerMin.y <= maxY && maxY <= m_ViewportCornerMax.y);
        }

        /// <summary>
        /// 指定ページの行と列の番号を取得
        /// </summary>
        private Vector2Int GetPageCoord(int pageIndex)
        {
            pageIndex = (int)Mathf.Repeat(pageIndex, TotalPageLength);

            if (m_StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                return new(pageIndex % m_PageLength.x, pageIndex / m_PageLength.x);
            }
            else
            {
                return new(pageIndex / m_PageLength.y, pageIndex % m_PageLength.y);
            }
        }

        /// <summary>
        /// ページのローカル座標取得
        /// </summary>
        private Vector2 GetPagePosition(int pageRow, int pageColumn)
        {
            return new((m_PageRectSize.x + m_Spacing.x) * pageColumn, (m_PageRectSize.y + m_Spacing.y) * pageRow * -1f);
        }

        /// <summary>
        /// 要素のローカル座標取得
        /// </summary>
        private Vector2 GetElementPosition(int pageRow, int pageColumn, int localRow, int localColumn)
        {
            return GetPagePosition(pageRow, pageColumn) + m_PageLayout.GetCellPosition(localRow, localColumn);
        }

        /// <summary>
        /// ローカル座標をcontentのanchoredPositionに変換する
        /// </summary>
        private Vector2 LocalToAnchoredPosition(Vector2 targetPosition)
        {
            // ターゲット位置をcontent座標と比較するため調整
            targetPosition *= -1f;

            // スクロール範囲
            var scrollRange = m_ScrollRect.content.rect.size;
            scrollRange.x -= (m_Padding.left + m_Padding.right) - m_Spacing.x;
            scrollRange.y -= (m_Padding.top + m_Padding.bottom) - m_Spacing.y;

            // contentの座標
            var contentPosition = m_ScrollRect.content.anchoredPosition - m_DefaultContentAnchoredPosition;
            if (m_ScrollRect.horizontal && scrollRange.x > 0f)
            {
                contentPosition.x %= scrollRange.x;
            }
            if (m_ScrollRect.vertical && scrollRange.y > 0f)
            {
                contentPosition.y %= scrollRange.y;
            }

            // 移動量
            var delta = targetPosition - contentPosition;
            if (!m_ScrollRect.horizontal)
            {
                delta.x = 0f;
            }
            if (!m_ScrollRect.vertical)
            {
                delta.y = 0f;
            }
            if (m_ScrollRect.movementType == ScrollRect.MovementType.Unrestricted && m_Loop)
            {
                // 隣の方が近いならそっちに移動する
                var nextDelta = delta;
                if (m_ScrollRect.horizontal)
                {
                    if (delta.x < 0f)
                    {
                        nextDelta.x += scrollRange.x;
                    }
                    else
                    {
                        nextDelta.x -= scrollRange.x;
                    }

                    if (Mathf.Abs(nextDelta.x) < Mathf.Abs(delta.x))
                    {
                        delta.x = nextDelta.x;
                    }
                }
                if (m_ScrollRect.vertical)
                {
                    if (delta.y < 0f)
                    {
                        nextDelta.y += scrollRange.y;
                    }
                    else
                    {
                        nextDelta.y -= scrollRange.y;
                    }

                    if (Mathf.Abs(nextDelta.y) < Mathf.Abs(delta.y))
                    {
                        delta.y = nextDelta.y;
                    }
                }
            }

            return m_ScrollRect.content.anchoredPosition + delta;
        }

        /// <summary>
        /// 指定ページのanchoredPositionを取得
        /// </summary>
        public Vector2 GetPageAnchoredPosition(int pageIndex)
        {
            var pageCoord = GetPageCoord(pageIndex);
            var pagePosition = GetPagePosition(pageCoord.y, pageCoord.x);
            return LocalToAnchoredPosition(pagePosition);
        }

        /// <summary>
        /// 指定要素のandhoredPositionを取得
        /// </summary>
        public Vector2 GetElementAnchoredPosition(int elementIndex)
        {
            elementIndex %= TotalPageLength * m_PageLayout.TotalCellCount;

            if (elementIndex < 0)
            {
                elementIndex += TotalPageLength * m_PageLayout.TotalCellCount;
            }

            int pageIndex = elementIndex / m_PageLayout.TotalCellCount;
            var pageCoord = GetPageCoord(pageIndex);

            int localIndex = elementIndex % m_PageLayout.TotalCellCount;
            var localCoord = m_PageLayout.GetCellCoord(localIndex);

            var elementPosition = GetElementPosition(pageCoord.y, pageCoord.x, localCoord.y, localCoord.x);
            return LocalToAnchoredPosition(elementPosition);
        }

        /// <summary>
        /// 指定座標にジャンプ
        /// </summary>
        public void JumpToPosition(Vector2 targetPosition)
        {
            m_ScrollRect.content.anchoredPosition = targetPosition;
        }

        /// <summary>
        /// 指定ページにジャンプ
        /// </summary>
        public void JumpToPage(int pageIndex)
        {
            JumpToPosition(GetPageAnchoredPosition(pageIndex));
        }

        /// <summary>
        /// 指定要素にジャンプ
        /// </summary>
        public void JumpToElement(int elementIndex)
        {
            JumpToPosition(GetElementAnchoredPosition(elementIndex));
        }

        /// <summary>
        /// 指定座標にスクロール
        /// </summary>
        public async UniTask ScrollToPosition(Vector2 targetPosition, float duration = 0.1f, CancellationToken cancellation = default)
        {
            CancelAutoScroll();
            m_AutoScrollCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken, cancellation);

            Vector2 startPosition = m_ScrollRect.content.anchoredPosition;
            float time = 0f;

            while (time <= duration)
            {
                time += Time.deltaTime;
                m_ScrollRect.velocity = Vector2.zero;
                m_ScrollRect.content.anchoredPosition = Vector2.Lerp(startPosition, targetPosition, time / duration);

                try
                {
                    await UniTask.Yield(m_AutoScrollCancellationTokenSource.Token);
                }
                catch
                {
                    return;
                }
            }
        }

        /// <summary>
        /// 指定ページにスクロール
        /// </summary>
        public async UniTask ScrollToPage(int pageIndex, float duration = 0.1f, CancellationToken cancellation = default)
        {
            await ScrollToPosition(GetPageAnchoredPosition(pageIndex), duration, cancellation);
        }

        /// <summary>
        /// 指定要素にスクロール
        /// </summary>
        public async UniTask ScrollToElement(int elementIndex, float duration = 0.1f, CancellationToken cancellation = default)
        {
            await ScrollToPosition(GetElementAnchoredPosition(elementIndex), duration, cancellation);
        }

        /// <summary>
        /// 自動スクロール処理のキャンセル
        /// </summary>
        private void CancelAutoScroll()
        {
            m_AutoScrollCancellationTokenSource?.Cancel();
            m_AutoScrollCancellationTokenSource?.Dispose();
            m_AutoScrollCancellationTokenSource = null;
        }

        /// <summary>
        /// 指定インデックス要素を取得する
        /// </summary>
        public IScrollElement GetElement(int elementIndex)
        {
            return m_ScrollElements.Cast<IScrollElement>().FirstOrDefault(x => x.Index == elementIndex);
        }
    }
}