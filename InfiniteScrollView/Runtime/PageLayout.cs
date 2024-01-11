using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.InfiniteScrollView
{
    /// <summary>
    /// �y�[�W���C�A�E�g
    /// </summary>
    [Serializable]
    public class PageLayout
    {
        /// <summary>
        /// �Z���T�C�Y
        /// </summary>
        [SerializeField]
        private Vector2 m_CellSize = Vector2.one * 100;

        /// <summary>
        /// �Z����
        /// </summary>
        [SerializeField]
        private Vector2Int m_CellCount = Vector2Int.one;

        /// <summary>
        /// �Z���ԃX�y�[�X
        /// </summary>
        [SerializeField]
        private Vector2 m_Spacing;

        /// <summary>
        /// �y�[�W���̍ŏ��̗v�f���z�u�����p
        /// </summary>
        [SerializeField]
        private GridLayoutGroup.Corner m_StartCorner;

        /// <summary>
        /// �y�[�W���v�f�̔z�u����
        /// </summary>
        [SerializeField]
        private GridLayoutGroup.Axis m_StartAxis;

        /// <summary>
        /// �Z���T�C�Y
        /// </summary>
        public Vector2 CellSize
        {
            get => m_CellSize;
            set => m_CellSize = value;
        }

        /// <summary>
        /// �Z����
        /// </summary>
        public Vector2Int CellCount
        {
            get => m_CellCount;
            set => m_CellCount = value;
        }

        /// <summary>
        /// ���Z����
        /// </summary>
        public int TotalCellCount => m_CellCount.x * m_CellCount.y;

        /// <summary>
        /// �Z���ԃX�y�[�X
        /// </summary>
        public Vector2 Spacing
        {
            get => m_Spacing;
            set => m_Spacing = value;
        }

        /// <summary>
        /// �y�[�W���̍ŏ��̗v�f���z�u�����p
        /// </summary>
        public GridLayoutGroup.Corner StartCorner
        {
            get => m_StartCorner;
            set => m_StartCorner = value;
        }

        /// <summary>
        /// �y�[�W���v�f�̔z�u����
        /// </summary>
        public GridLayoutGroup.Axis StartAxis => m_StartAxis;

        /// <summary>
        /// �w��Z���̃��[�J�����W�̎擾
        /// </summary>
        public Vector2 GetCellPosition(int y, int x)
        {
            x %= m_CellCount.x;

            if (x < 0)
            {
                x += m_CellCount.x;
            }

            y %= m_CellCount.y;

            if (y < 0)
            {
                y += m_CellCount.y;
            }

            return new(
                (m_CellSize.x + m_Spacing.x) * x,
                (m_CellSize.y + m_Spacing.y) * y * -1f
            );
        }

        /// <summary>
        /// �w��C���f�b�N�X�Z���̍s�Ɨ�̔ԍ����擾
        /// </summary>
        public Vector2Int GetCellCoord(int index)
        {
            index %= TotalCellCount;

            if (index < 0)
            {
                index += TotalCellCount;
            }

            if (m_StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                return new(index % m_CellCount.x, index / m_CellCount.x);
            }
            else
            {
                return new(index / m_CellCount.y, index % m_CellCount.y);
            }
        }

        /// <summary>
        /// �w��Z���̃��[�J���C���f�b�N�X�̎擾
        /// </summary>
        public int GetCellIndex(int y, int x)
        {
            if (m_StartAxis == GridLayoutGroup.Axis.Horizontal)
            {
                switch (m_StartCorner)
                {
                    case GridLayoutGroup.Corner.UpperRight:
                        return m_CellCount.x * y + (m_CellCount.x - 1 - x);

                    case GridLayoutGroup.Corner.LowerLeft:
                        return m_CellCount.x * (m_CellCount.y - 1 - y) + x;

                    case GridLayoutGroup.Corner.LowerRight:
                        return m_CellCount.x * (m_CellCount.y - 1 - y) + (m_CellCount.x - 1 - x);

                    default:
                        return m_CellCount.x * y + x;
                }
            }
            else
            {
                switch (m_StartCorner)
                {
                    case GridLayoutGroup.Corner.UpperRight:
                        return m_CellCount.y * (m_CellCount.x - 1 - x) + y;

                    case GridLayoutGroup.Corner.LowerLeft:
                        return m_CellCount.y * x  + (m_CellCount.y - 1 - y);

                    case GridLayoutGroup.Corner.LowerRight:
                        return m_CellCount.y * (m_CellCount.x - 1 - x) + (m_CellCount.y - 1 - y);

                    default:
                        return m_CellCount.y * x + y;
                }
            }
        }
    }
}
