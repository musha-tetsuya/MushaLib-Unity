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
    /// �����X�N���[���r���[
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
        /// �v�f�v���n�u
        /// </summary>
        [SerializeField]
        [Header("�v�f�v���n�u")]
        private ScrollElement m_ElementPrefab;

        /// <summary>
        /// �v�f��
        /// </summary>
        [SerializeField]
        [Header("�v�f��")]
        private int m_ElementCount;

        /// <summary>
        /// �y�[�W���C�A�E�g
        /// </summary>
        [SerializeField]
        [Header("�y�[�W���C�A�E�g")]
        private PageLayout m_PageLayout;

        /// <summary>
        /// �y�[�W�ԃX�y�[�X
        /// </summary>
        [SerializeField]
        [Header("�y�[�W�ԃX�y�[�X")]
        private Vector2 m_Spacing;

        /// <summary>
        /// �y�[�W�̔z�u����
        /// </summary>
        [SerializeField]
        [Header("�y�[�W�̔z�u����")]
        private GridLayoutGroup.Axis m_StartAxis;

        /// <summary>
        /// Viewport�����̗]��
        /// </summary>
        [SerializeField]
        [Header("Viewport�����̗]��")]
        private RectOffset m_Padding;

        /// <summary>
        /// Viewport�O���̗]��
        /// </summary>
        [SerializeField]
        [Header("Viewport�O���̗]��")]
        private RectOffset m_Margin;

        /// <summary>
        /// �y�[�W�����[�v�����邩�ǂ���
        /// </summary>
        [SerializeField]
        [Header("�y�[�W�����[�v�����邩�ǂ���")]
        private bool m_Loop;

        /// <summary>
        /// �X�i�b�v�^�C�v
        /// </summary>
        [SerializeField]
        [Header("�X�i�b�v�^�C�v")]
        private SnapType m_SnapType;

        /// <summary>
        /// �X�i�b�v�J�n��臒l
        /// </summary>
        [SerializeField]
        [Header("�X�i�b�v�J�n��臒l")]
        private float m_SnapThreshold = 100f;

        /// <summary>
        /// �X�i�b�v����
        /// </summary>
        [SerializeField]
        [Header("�X�i�b�v����")]
        private float m_SnapDuration = 0.1f;

        /// <summary>
        /// �v�f�v���n�u
        /// </summary>
        private ScrollElement m_CurrentElementPrefab;

        /// <summary>
        /// �y�[�W��`�T�C�Y
        /// </summary>
        private Vector2 m_PageRectSize;

        /// <summary>
        /// �c���y�[�W��
        /// </summary>
        private Vector2Int m_PageLength;

        /// <summary>
        /// �y�[�W�C���f�b�N�X������
        /// </summary>
        private Vector2Int m_PageIndexDelta;

        /// <summary>
        /// Viewport��`�̍ŏ��ʒu
        /// </summary>
        private Vector2 m_ViewportCornerMin;

        /// <summary>
        /// Viewport��`�̍ő�ʒu
        /// </summary>
        private Vector2 m_ViewportCornerMax;

        /// <summary>
        /// �X�N���[���v�f�z��
        /// </summary>
        private Vector2Int m_ScrollElementLength;

        /// <summary>
        /// �X�N���[���v�f�z��
        /// </summary>
        private IScrollElement[,] m_ScrollElements;

        /// <summary>
        /// �X�N���[���v�f�z��̉����̃C���f�b�N�X���i�[�����z��
        /// </summary>
        private int[] m_OriginalColumnIndices;

        /// <summary>
        /// �X�N���[���v�f�z��̏c���̃C���f�b�N�X���i�[�����z��
        /// </summary>
        private int[] m_OriginalRowIndices;

        /// <summary>
        /// �X�N���[���v�f�z��̉����̃C���f�b�N�X���t���Ɋi�[�����z��
        /// </summary>
        private int[] m_ReverseColumnIndices;

        /// <summary>
        /// �X�N���[���v�f�z��̏c���̃C���f�b�N�X���t���Ɋi�[�����z��
        /// </summary>
        private int[] m_ReverseRowIndices;

        /// <summary>
        /// content.anchoredPosition�̏����l
        /// </summary>
        private Vector2 m_DefaultContentAnchoredPosition;

        /// <summary>
        /// content.anchoredPosition�̑O��̒l
        /// </summary>
        private Vector2 m_PrevContentAnchoredPosition;

        /// <summary>
        /// Viewport���猩��content�̍���p�̍��W
        /// </summary>
        private Vector2 m_ContentTopLeftInViewport;

        /// <summary>
        /// �����X�N���[�������̃L�����Z���g�[�N��
        /// </summary>
        private CancellationTokenSource m_AutoScrollCancellationTokenSource;

        /// <summary>
        /// ���y�[�W��
        /// </summary>
        private int TotalPageLength => m_PageLength.x * m_PageLength.y;

        /// <summary>
        /// �v�f�v���n�u
        /// </summary>
        public ScrollElement ElementPrefab
        {
            get => m_ElementPrefab;
            set => m_ElementPrefab = value;
        }

        /// <summary>
        /// �v�f��
        /// </summary>
        public int ElementCount
        {
            get => m_ElementCount;
            set => m_ElementCount = value;
        }

        /// <summary>
        /// �y�[�W���C�A�E�g
        /// </summary>
        public PageLayout PageLayout
        {
            get => m_PageLayout;
            set => m_PageLayout = value;
        }

        /// <summary>
        /// �y�[�W�ԃX�y�[�X
        /// </summary>
        public Vector2 Spacing
        {
            get => m_Spacing;
            set => m_Spacing = value;
        }

        /// <summary>
        /// �y�[�W�̔z�u����
        /// </summary>
        public GridLayoutGroup.Axis StartAxis
        {
            get => m_StartAxis;
            set => m_StartAxis = value;
        }

        /// <summary>
        /// Viewport�����̗]��
        /// </summary>
        public RectOffset Padding
        {
            get => m_Padding;
            set => m_Padding = value;
        }

        /// <summary>
        /// Viewport�O���̗]��
        /// </summary>
        public RectOffset Margin
        {
            get => m_Margin;
            set => m_Margin = value;
        }

        /// <summary>
        /// �y�[�W�����[�v�����邩�ǂ���
        /// </summary>
        public bool Loop
        {
            get => m_Loop;
            set => m_Loop = value;
        }

        /// <summary>
        /// �X�i�b�v�^�C�v
        /// </summary>
        public SnapType SnapType
        {
            get => m_SnapType;
            set => m_SnapType = value;
        }

        /// <summary>
        /// �X�i�b�v�J�n��臒l
        /// </summary>
        public float SnapThrethold
        {
            get => m_SnapThreshold;
            set => m_SnapThreshold = value;
        }

        /// <summary>
        /// �X�i�b�v����
        /// </summary>
        public float SnapDuration
        {
            get => m_SnapDuration;
            set => m_SnapDuration = value;
        }

        /// <summary>
        /// �v�f�X�V���C�x���g
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
            // �X�N���[�����C�x���g�w��
            m_ScrollRect.onValueChanged.AddListener(OnScroll);

            if (m_ScrollRect.horizontalScrollbar != null)
            {
                // ���X�N���[���o�[���쎞
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
                // �c�X�N���[���o�[���쎞
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
        /// ������
        /// </summary>
        public void Initialize()
        {
            if (m_ElementPrefab == null)
            {
                Debug.LogError("�v�f�v���n�u���ݒ肳��Ă��܂���B", this);
                return;
            }

            if (m_ElementCount < 0)
            {
                Debug.LogError("�v�f����0�����ł��B", this);
                return;
            }

            if (m_PageLayout.CellCount.x <= 0 || m_PageLayout.CellCount.y <= 0)
            {
                Debug.LogError("�y�[�W���̃Z������0�ȉ��ł��B", this);
                return;
            }

            // ���T�C�N���\�v�f�v���n�u
            Queue<IScrollElement> recyclablePrefabs = m_ScrollElements == null ? new() : new(m_ScrollElements.Cast<IScrollElement>());

            // �v�f�v���n�u���ύX���ꂽ
            if (m_ElementPrefab != m_CurrentElementPrefab)
            {
                m_CurrentElementPrefab = m_ElementPrefab;

                // �����v�f�̃N���A
                while (recyclablePrefabs.TryDequeue(out var prefabInstance))
                {
                    Destroy(prefabInstance.RectTransform.gameObject);
                }
            }

            // �y�[�W�Z���T�C�Y�ݒ�
            m_PageLayout.CellSize = (m_CurrentElementPrefab.transform as RectTransform).rect.size * m_CurrentElementPrefab.transform.localScale;

            // �y�[�W�̏c����
            m_PageRectSize.x = (m_PageLayout.CellSize.x + m_PageLayout.Spacing.x) * m_PageLayout.CellCount.x - m_PageLayout.Spacing.x;
            m_PageRectSize.y = (m_PageLayout.CellSize.y + m_PageLayout.Spacing.y) * m_PageLayout.CellCount.y - m_PageLayout.Spacing.y;

            // �K�v�y�[�W��
            m_PageLength = Vector2Int.one * Mathf.Max(Mathf.CeilToInt((float)m_ElementCount / m_PageLayout.TotalCellCount), 1);

            // �y�[�W�C���f�b�N�X�ω���
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

            // AutoHideAndExpandViewport�̏ꍇ�Acontent�T�C�Y�����܂�Ȃ���viewport�T�C�Y���擾�o���Ȃ��B
            // content�T�C�Y�����ALayout�����r���h����viewport�T�C�Y�����肷��B
            // ���̂��߁A��Ucontent��anchorMin��anchorMax�𑵂���B
            var contentAnchorMax = m_ScrollRect.content.anchorMax;
            m_ScrollRect.content.anchorMax = m_ScrollRect.content.anchorMin;

            // content�T�C�Y����
            m_ScrollRect.content.sizeDelta = new(
                m_Padding.left + m_Padding.right + (m_PageRectSize.x + m_Spacing.x) * m_PageLength.x - m_Spacing.x,
                m_Padding.top + m_Padding.bottom + (m_PageRectSize.y + m_Spacing.y) * m_PageLength.y - m_Spacing.y
            );

            // Layout�����r���h����viewport�T�C�Y������
            if ((m_ScrollRect.horizontalScrollbar != null && m_ScrollRect.horizontalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport) ||
                (m_ScrollRect.verticalScrollbar != null && m_ScrollRect.verticalScrollbarVisibility == ScrollRect.ScrollbarVisibility.AutoHideAndExpandViewport))
            {
                LayoutRebuilder.ForceRebuildLayoutImmediate(m_ScrollRect.transform as RectTransform);
            }

            // content��anchorMax�����ɖ߂�
            m_ScrollRect.content.anchorMax = contentAnchorMax;

            // �X�N���[�������ɍ��킹��anchor�𒲐�
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

            // anchor�ɍ��킹��content�T�C�Y�𒲐�
            m_ScrollRect.content.sizeDelta -= (m_ScrollRect.content.anchorMax - m_ScrollRect.content.anchorMin) * m_ScrollRect.viewport.rect.size;

            // PrevContentPosition��Viewport + Margin�̈ʒu�ɍ��킹��
            var contentAnchor = m_ScrollRect.content.anchorMin + (m_ScrollRect.content.anchorMax - m_ScrollRect.content.anchorMin) * 0.5f;
            m_PrevContentAnchoredPosition.x = m_ScrollRect.content.rect.width * m_ScrollRect.content.pivot.x - m_ScrollRect.viewport.rect.width * contentAnchor.x - m_Margin.left;
            m_PrevContentAnchoredPosition.y = -m_ScrollRect.content.rect.height * (1f - m_ScrollRect.content.pivot.y) + m_ScrollRect.viewport.rect.height * (1f - contentAnchor.y) + m_Margin.top;

            // content�̏���anchoredPosition������
            m_DefaultContentAnchoredPosition = Vector2.zero;
            if (m_ScrollRect.horizontal)
            {
                if (m_ScrollRect.content.rect.width < m_ScrollRect.viewport.rect.width)
                {
                    m_DefaultContentAnchoredPosition.x = m_ScrollRect.viewport.rect.width * (m_ScrollRect.content.pivot.x - m_ScrollRect.content.anchorMin.x);
                }
                else
                {
                    m_DefaultContentAnchoredPosition.x = m_PrevContentAnchoredPosition.x + m_Margin.left;
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
                    m_DefaultContentAnchoredPosition.y = m_PrevContentAnchoredPosition.y - m_Margin.top;
                }
            }

            // viewport�擾
            var viewportCorners = new Vector3[4];
            m_ScrollRect.viewport.GetLocalCorners(viewportCorners);
            m_ViewportCornerMin = viewportCorners[0] - new Vector3(m_Margin.left, m_Margin.bottom);
            m_ViewportCornerMax = viewportCorners[2] + new Vector3(m_Margin.right, m_Margin.top);

            // ScrollElement�z��
            m_ScrollElementLength = m_PageLayout.CellCount;
            if (m_ScrollRect.horizontal)
            {
                // viewport�͈͓��ɗv�f���������邩�Aviewport�ʒu���������炵�ă`�F�b�N����
                var viewportMaxX = m_Padding.left + (m_ViewportCornerMax.x - m_ViewportCornerMin.x) + (m_PageLayout.CellSize.x - 1f);
                int pageColumn = Mathf.FloorToInt(viewportMaxX / (m_PageRectSize.x + m_Spacing.x));
                int localColumn = Mathf.Clamp(Mathf.FloorToInt((viewportMaxX - GetPagePosition(0, pageColumn).x) / (m_PageLayout.CellSize.x + m_PageLayout.Spacing.x)), 0, m_PageLayout.CellCount.x - 1);
                m_ScrollElementLength.x = m_PageLayout.CellCount.x * pageColumn + localColumn;
            }
            if (m_ScrollRect.vertical)
            {
                // viewport�͈͓��ɗv�f���������邩�Aviewport�ʒu���������炵�ă`�F�b�N����
                var viewportMinY = -m_Padding.top - (m_ViewportCornerMax.y - m_ViewportCornerMin.y) - (m_PageLayout.CellSize.y - 1f);
                int pageRow = Mathf.FloorToInt(-viewportMinY / (m_PageRectSize.y + m_Spacing.y));
                int localRow = Mathf.Clamp(Mathf.FloorToInt(-(viewportMinY - GetPagePosition(pageRow, 0).y) / (m_PageLayout.CellSize.y + m_PageLayout.Spacing.y)), 0, m_PageLayout.CellCount.y - 1);
                m_ScrollElementLength.y = m_PageLayout.CellCount.y * pageRow + localRow;
            }

            // ScrollElement�z�񐶐�
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
                    anchoredPosition.x += m_PageLayout.CellSize.x * element.RectTransform.pivot.x;
                    anchoredPosition.y -= m_PageLayout.CellSize.y * element.RectTransform.pivot.y;

                    element.RectTransform.anchorMin =
                    element.RectTransform.anchorMax = new(0f, 1f);
                    element.RectTransform.anchoredPosition = anchoredPosition;

                    UpdateElementIndex(element);
                }
            }

            // �g��Ȃ��������T�C�N���v�f��j��
            while (recyclablePrefabs.TryDequeue(out var prefabInstance))
            {
                Destroy(prefabInstance.RectTransform.gameObject);
            }

            // content��anchoredPosition���Z�b�g�����ScrollRect��onValueChanged������̂ŁA��Uenabled��؂�
            var prevScrollRectEnabled = m_ScrollRect.enabled;
            m_ScrollRect.enabled = false;

            // PrevContentAnchoredPosition����(0, 0)�܂ŃX�N���[�������Ƃ������Ƃɂ��ėv�f���X�V
            m_ScrollRect.content.anchoredPosition = m_DefaultContentAnchoredPosition;
            OnScroll(default);

            // enabled��߂�
            m_ScrollRect.enabled = prevScrollRectEnabled;
        }

        /// <summary>
        /// �X�N���[����
        /// </summary>
        private void OnScroll(Vector2 value)
        {
            if (m_ElementCount <= 0 || m_ScrollElements == null)
            {
                return;
            }

            // �X�N���[����
            Vector2 delta = m_ScrollRect.content.anchoredPosition - m_PrevContentAnchoredPosition;
            m_PrevContentAnchoredPosition = m_ScrollRect.content.anchoredPosition;

            // Viewport���猩��content�̍���p�̍��W
            m_ContentTopLeftInViewport = m_ScrollRect.content.localPosition;
            m_ContentTopLeftInViewport.x -= m_ScrollRect.content.rect.width * m_ScrollRect.content.pivot.x;
            m_ContentTopLeftInViewport.y += m_ScrollRect.content.rect.height * m_ScrollRect.content.pivot.y;

            // ���X�N���[����
            if (m_ScrollRect.horizontal && !Mathf.Approximately(delta.x, 0f))
            {
                // �X�N���[����i�܂����H
                int sign = delta.x < 0f ? 1 : -1;
                int[] columnIndices = delta.x < 0f ? m_OriginalColumnIndices : m_ReverseColumnIndices;

                int firstX = columnIndices.First();
                int lastX = columnIndices.Last();

                // viewport�͈͊O�ɂȂ����H
                while (!OverlapHorizontal(m_ScrollElements[0, firstX]))
                {
                    int moveCount = sign;
                    Vector2? movePosition = null;

                    for (int y = 0; y < m_ScrollElementLength.y; y++)
                    {
                        var firstElement = m_ScrollElements[y, firstX];
                        var lastElement = m_ScrollElements[y, lastX];

                        // �ŏI�v�f�̎��̈ʒu�Ɉړ�
                        SetElementColumn(firstElement, lastElement.Column + moveCount);

                        while (!movePosition.HasValue)
                        {
                            // �ړ���̃��[�J�����W
                            var newLocalPosition = GetElementPosition(firstElement.PageRow, firstElement.PageColumn, firstElement.LocalRow, firstElement.LocalColumn);
                            float minX = newLocalPosition.x + m_ContentTopLeftInViewport.x;
                            float maxX = minX + m_PageLayout.CellSize.x;

                            // �ړ��̌���viewport�͈͓��ɂȂ肻���H
                            if ((sign == 1 && m_ViewportCornerMin.x <= maxX) || (sign == -1 && minX <= m_ViewportCornerMax.x))
                            {
                                // ���W�ړ��ʌ���
                                movePosition = newLocalPosition - firstElement.LocalPosition;
                            }
                            else
                            {
                                // ������ׂɈړ����Ă݂�
                                moveCount += sign;
                                SetElementColumn(firstElement, lastElement.Column + moveCount);
                            }
                        }

                        // ���W�ړ�
                        firstElement.LocalPosition += movePosition.Value;
                        firstElement.RectTransform.anchoredPosition += movePosition.Value;

                        // �C���f�b�N�X�X�V
                        UpdateElementIndex(firstElement);

                        // �X�N���[���v�f�z����V�t�g
                        foreach (int x in columnIndices.SkipLast(1))
                        {
                            m_ScrollElements[y, x] = m_ScrollElements[y, x + sign];
                        }

                        m_ScrollElements[y, lastX] = firstElement;
                    }
                }
            }

            // �c�X�N���[����
            if (m_ScrollRect.vertical && !Mathf.Approximately(delta.y, 0f))
            {
                // �X�N���[����i�܂����H
                int sign = delta.y > 0f ? 1 : -1;
                int[] rowIndices = delta.y > 0f ? m_OriginalRowIndices : m_ReverseRowIndices;

                int firstY = rowIndices.First();
                int lastY = rowIndices.Last();

                // viewport�͈͊O�ɂȂ����H
                while (!OverlapVertical(m_ScrollElements[firstY, 0]))
                {
                    int moveCount = sign;
                    Vector2? movePosition = null;

                    for (int x = 0; x < m_ScrollElementLength.x; x++)
                    {
                        var firstElement = m_ScrollElements[firstY, x];
                        var lastElement = m_ScrollElements[lastY, x];

                        // �ŏI�v�f�̎��̈ʒu�Ɉړ�
                        SetElementRow(firstElement, lastElement.Row + moveCount);

                        while (!movePosition.HasValue)
                        {
                            // �ړ���̃��[�J�����W
                            var newLocalPosition = GetElementPosition(firstElement.PageRow, firstElement.PageColumn, firstElement.LocalRow, firstElement.LocalColumn);
                            float maxY = newLocalPosition.y + m_ContentTopLeftInViewport.y;
                            float minY = maxY - m_PageLayout.CellSize.y;

                            // �ړ��̌���viewport�͈͓��ɂȂ肻���H
                            if ((sign == 1 && minY <= m_ViewportCornerMax.y) || (sign == -1 && m_ViewportCornerMin.y <= maxY))
                            {
                                // ���W�ړ��ʌ���
                                movePosition = newLocalPosition - firstElement.LocalPosition;
                            }
                            else
                            {
                                // ������ׂɈړ����Ă݂�
                                moveCount += sign;
                                SetElementRow(firstElement, lastElement.Row + moveCount);
                            }
                        }

                        // ���W�ړ�
                        firstElement.LocalPosition += movePosition.Value;
                        firstElement.RectTransform.anchoredPosition += movePosition.Value;

                        // �C���f�b�N�X�X�V
                        UpdateElementIndex(firstElement);

                        // �X�N���[���v�f�z����V�t�g
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
        /// �h���b�O�J�n��
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
        /// �h���b�O�I����
        /// </summary>
        async void IEndDragHandler.OnEndDrag(PointerEventData eventData)
        {
            if (m_ElementCount <= 0 || m_SnapType == SnapType.None || eventData.button != PointerEventData.InputButton.Left)
            {
                return;
            }

            CancelAutoScroll();
            m_AutoScrollCancellationTokenSource = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);

            try
            {
                // �x���V�e�B��臒l�ȉ��ɂȂ�܂őҋ@
                var sqrThreshold = m_SnapThreshold * m_SnapThreshold;
                await UniTask.WaitUntil(() => m_ScrollRect.velocity.sqrMagnitude <= sqrThreshold, cancellationToken: m_AutoScrollCancellationTokenSource.Token);
            }
            catch
            {
                return;
            }

            // �X�i�b�v�����̊J�n�A�I������content��anchoredPosition
            Vector2 startAnchoredPosition = m_ScrollRect.content.anchoredPosition;
            Vector2 endAnchoredPosition = startAnchoredPosition;

            // ���X�N���[������ꍇ
            if (m_ScrollRect.horizontal)
            {
                // �]�����������R���e���c��
                float contentWidth = m_ScrollRect.content.rect.width - (m_Padding.left + m_Padding.right);

                // content�̍��W
                float contentPositionX = Mathf.Repeat(m_DefaultContentAnchoredPosition.x - m_ScrollRect.content.anchoredPosition.x, contentWidth + m_Spacing.x);

                // content�̈ʒu���猻�݃^�[�Q�b�g���̗v�f������o��
                int pageColumn = Mathf.FloorToInt(contentPositionX / (m_PageRectSize.x + m_Spacing.x));
                int localColumn = Mathf.FloorToInt((contentPositionX - GetPagePosition(0, pageColumn).x) / (m_PageLayout.CellSize.x + m_PageLayout.Spacing.x));

                // �ꍇ�ɂ���Ăׂ̗͗v�f�̕����߂���������Ȃ��̂ŁA��r����v�f������
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

                // �X�i�b�v�^�[�Q�b�g�ʒu����
                float targetPositionX = GetElementPosition(0, pageColumn, 0, localColumn).x;

                // �ׂ̃^�[�Q�b�g�̕����߂��Ȃ�A�X�i�b�v�^�[�Q�b�g�ʒu�͈�ׂɂ���
                float nextTargetPositionX = GetElementPosition(0, nextPageColumn, 0, nextLocalColumn).x;

                if (Mathf.Abs(nextTargetPositionX - contentPositionX) < Mathf.Abs(targetPositionX - contentPositionX))
                {
                    targetPositionX = nextTargetPositionX;
                }

                // ���݈ʒu����^�[�Q�b�g�܂ł̈ړ���
                float dx = (targetPositionX - m_Padding.left) - contentPositionX;

                // �X�i�b�v�I������content��anchoredPosition������
                endAnchoredPosition.x -= dx;

                if (m_ScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
                {
                    // �ړ�����
                    float maxX = m_DefaultContentAnchoredPosition.x;
                    float minX = maxX - Mathf.Max(m_ScrollRect.content.rect.width - m_ScrollRect.viewport.rect.width, 0f);
                    endAnchoredPosition.x = Mathf.Clamp(endAnchoredPosition.x, minX, maxX);
                }
            }

            // �c�X�N���[������ꍇ
            if (m_ScrollRect.vertical)
            {
                // �]�����������R���e���c����
                float contentHeight = m_ScrollRect.content.rect.height - (m_Padding.top + m_Padding.bottom);

                // content�̍��W
                float contentPositionY = Mathf.Repeat(m_DefaultContentAnchoredPosition.y - m_ScrollRect.content.anchoredPosition.y, contentHeight + m_Spacing.y);

                // content�̈ʒu���猻�݃^�[�Q�b�g���̗v�f������o��
                int pageRow = Mathf.FloorToInt(-contentPositionY / (m_PageRectSize.y + m_Spacing.y));
                int localRow = Mathf.FloorToInt(-(contentPositionY - GetPagePosition(pageRow, 0).y) / (m_PageLayout.CellSize.y + m_PageLayout.Spacing.y));

                // �ꍇ�ɂ���Ăׂ̗͗v�f�̕����߂���������Ȃ��̂ŁA��r����v�f������
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

                // �X�i�b�v�^�[�Q�b�g�ʒu����
                float targetPositionY = GetElementPosition(pageRow, 0, localRow, 0).y;

                // �ׂ̃^�[�Q�b�g�̕����߂��Ȃ�A�X�i�b�v�^�[�Q�b�g�ʒu�͈�ׂɂ���
                float nextTargetPositionY = GetElementPosition(nextPageRow, 0, nextLocalRow, 0).y;

                if (Mathf.Abs(nextTargetPositionY - contentPositionY) < Mathf.Abs(targetPositionY - contentPositionY))
                {
                    targetPositionY = nextTargetPositionY;
                }

                // ���݈ʒu����^�[�Q�b�g�܂ł̈ړ���
                float dy = (targetPositionY + m_Padding.top) - contentPositionY;

                // �X�i�b�v�I������content��anchoredPosition������
                endAnchoredPosition.y -= dy;

                if (m_ScrollRect.movementType != ScrollRect.MovementType.Unrestricted)
                {
                    // �ړ�����
                    float minY = m_DefaultContentAnchoredPosition.y;
                    float maxY = minY + Mathf.Max(m_ScrollRect.content.rect.height - m_ScrollRect.viewport.rect.height, 0f);
                    endAnchoredPosition.y = Mathf.Clamp(endAnchoredPosition.y, minY, maxY);
                }
            }

            // �X�i�b�v�^�[�Q�b�g�ʒu�܂ŏ��X�Ɉړ�
            await ScrollToPosition(endAnchoredPosition, m_SnapDuration);
        }

        /// <summary>
        /// �v�f�̗�ԍ����Z�b�g����
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
        /// �v�f�̍s�ԍ����Z�b�g����
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
        /// �v�f�̍s�Ɨ񂩂�C���f�b�N�X���X�V
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
        /// �v�f��Viewport���͈͓̔����ǂ���
        /// </summary>
        private bool OverlapHorizontal(IScrollElement element)
        {
            float minX = m_ContentTopLeftInViewport.x + element.LocalPosition.x;
            float maxX = minX + m_PageLayout.CellSize.x;

            return (m_ViewportCornerMin.x <= minX && minX <= m_ViewportCornerMax.x)
                || (m_ViewportCornerMin.x <= maxX && maxX <= m_ViewportCornerMax.x);
        }

        /// <summary>
        /// �v�f��Viewport�����͈͓̔����ǂ���
        /// </summary>
        private bool OverlapVertical(IScrollElement element)
        {
            float maxY = m_ContentTopLeftInViewport.y + element.LocalPosition.y;
            float minY = maxY - m_PageLayout.CellSize.y;

            return (m_ViewportCornerMin.y <= minY && minY <= m_ViewportCornerMax.y)
                || (m_ViewportCornerMin.y <= maxY && maxY <= m_ViewportCornerMax.y);
        }

        /// <summary>
        /// �w��y�[�W�̍s�Ɨ�̔ԍ����擾
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
        /// �y�[�W�̃��[�J�����W�擾
        /// </summary>
        private Vector2 GetPagePosition(int pageRow, int pageColumn)
        {
            return new(
                x: m_Padding.left + (m_PageRectSize.x + m_Spacing.x) * pageColumn,
                y: -m_Padding.top - (m_PageRectSize.y + m_Spacing.y) * pageRow
            );
        }

        /// <summary>
        /// �v�f�̃��[�J�����W�擾
        /// </summary>
        private Vector2 GetElementPosition(int pageRow, int pageColumn, int localRow, int localColumn)
        {
            return GetPagePosition(pageRow, pageColumn) + m_PageLayout.GetCellPosition(localRow, localColumn);
        }

        /// <summary>
        /// ���[�J�����W��content��anchoredPosition�ɕϊ�����
        /// </summary>
        private Vector2 LocalToAnchoredPosition(Vector2 targetPosition)
        {
            // �^�[�Q�b�g�ʒu��content���W�Ɣ�r���邽�ߒ���
            targetPosition.x -= m_Padding.left;
            targetPosition.y += m_Padding.top;
            targetPosition *= -1f;

            // �X�N���[���͈�
            var scrollRange = m_ScrollRect.content.rect.size;
            scrollRange.x -= (m_Padding.left + m_Padding.right) - m_Spacing.x;
            scrollRange.y -= (m_Padding.top + m_Padding.bottom) - m_Spacing.y;

            // content�̍��W
            var contentPosition = m_ScrollRect.content.anchoredPosition - m_DefaultContentAnchoredPosition;
            if (m_ScrollRect.horizontal && scrollRange.x > 0f)
            {
                contentPosition.x %= scrollRange.x;
            }
            if (m_ScrollRect.vertical && scrollRange.y > 0f)
            {
                contentPosition.y %= scrollRange.y;
            }

            // �ړ���
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
                // �ׂ̕����߂��Ȃ炻�����Ɉړ�����
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
        /// �w��y�[�W��anchoredPosition���擾
        /// </summary>
        public Vector2 GetPageAnchoredPosition(int pageIndex)
        {
            var pageCoord = GetPageCoord(pageIndex);
            var pagePosition = GetPagePosition(pageCoord.y, pageCoord.x);
            return LocalToAnchoredPosition(pagePosition);
        }

        /// <summary>
        /// �w��v�f��andhoredPosition���擾
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
        /// �w����W�ɃW�����v
        /// </summary>
        public void JumpToPosition(Vector2 targetPosition)
        {
            m_ScrollRect.content.anchoredPosition = targetPosition;
        }

        /// <summary>
        /// �w��y�[�W�ɃW�����v
        /// </summary>
        public void JumpToPage(int pageIndex)
        {
            JumpToPosition(GetPageAnchoredPosition(pageIndex));
        }

        /// <summary>
        /// �w��v�f�ɃW�����v
        /// </summary>
        public void JumpToElement(int elementIndex)
        {
            JumpToPosition(GetElementAnchoredPosition(elementIndex));
        }

        /// <summary>
        /// �w����W�ɃX�N���[��
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
        /// �w��y�[�W�ɃX�N���[��
        /// </summary>
        public async UniTask ScrollToPage(int pageIndex, float duration = 0.1f, CancellationToken cancellation = default)
        {
            await ScrollToPosition(GetPageAnchoredPosition(pageIndex), duration, cancellation);
        }

        /// <summary>
        /// �w��v�f�ɃX�N���[��
        /// </summary>
        public async UniTask ScrollToElement(int elementIndex, float duration = 0.1f, CancellationToken cancellation = default)
        {
            await ScrollToPosition(GetElementAnchoredPosition(elementIndex), duration, cancellation);
        }

        /// <summary>
        /// �����X�N���[�������̃L�����Z��
        /// </summary>
        private void CancelAutoScroll()
        {
            m_AutoScrollCancellationTokenSource?.Cancel();
            m_AutoScrollCancellationTokenSource?.Dispose();
            m_AutoScrollCancellationTokenSource = null;
        }
    }
}