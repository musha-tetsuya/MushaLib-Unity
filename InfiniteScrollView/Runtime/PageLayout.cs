using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace MushaLib.InfiniteScrollView
{
    /// <summary>
    /// ページレイアウト
    /// </summary>
    [Serializable]
    public class PageLayout
    {
        /// <summary>
        /// セルサイズ
        /// </summary>
        [SerializeField]
        private Vector2 m_CellSize = Vector2.one * 100;

        /// <summary>
        /// セル数
        /// </summary>
        [SerializeField]
        private Vector2Int m_CellCount = Vector2Int.one;

        /// <summary>
        /// セル間スペース
        /// </summary>
        [SerializeField]
        private Vector2 m_Spacing;

        /// <summary>
        /// ページ内の最初の要素が配置される角
        /// </summary>
        [SerializeField]
        private GridLayoutGroup.Corner m_StartCorner;

        /// <summary>
        /// ページ内要素の配置方向
        /// </summary>
        [SerializeField]
        private GridLayoutGroup.Axis m_StartAxis;

        /// <summary>
        /// セルサイズ
        /// </summary>
        public Vector2 CellSize
        {
            get => m_CellSize;
            set => m_CellSize = value;
        }

        /// <summary>
        /// セル数
        /// </summary>
        public Vector2Int CellCount
        {
            get => m_CellCount;
            set => m_CellCount = value;
        }

        /// <summary>
        /// 総セル数
        /// </summary>
        public int TotalCellCount => m_CellCount.x * m_CellCount.y;

        /// <summary>
        /// セル間スペース
        /// </summary>
        public Vector2 Spacing
        {
            get => m_Spacing;
            set => m_Spacing = value;
        }

        /// <summary>
        /// ページ内の最初の要素が配置される角
        /// </summary>
        public GridLayoutGroup.Corner StartCorner
        {
            get => m_StartCorner;
            set => m_StartCorner = value;
        }

        /// <summary>
        /// ページ内要素の配置方向
        /// </summary>
        public GridLayoutGroup.Axis StartAxis => m_StartAxis;

        /// <summary>
        /// 指定セルのローカル座標の取得
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
        /// 指定インデックスセルの行と列の番号を取得
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
        /// 指定セルのローカルインデックスの取得
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
