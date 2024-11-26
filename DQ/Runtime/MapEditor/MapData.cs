using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.MapEditor
{
    /// <summary>
    /// マップデータ
    /// </summary>
    public class MapData
    {
        /// <summary>
        /// サイズ
        /// </summary>
        public Vector2Int Size { get; set; }

        /// <summary>
        /// ページ内セルサイズ
        /// </summary>
        public Vector2 PageCellSize { get; set; }

        /// <summary>
        /// ページ内セル数
        /// </summary>
        public Vector2Int PageCellCount { get; set; }

        /// <summary>
        /// 使用しているアトラスのguidリスト
        /// </summary>
        public List<string> AtlasKeys { get; set; }

        /// <summary>
        /// マップチップデータ群
        /// </summary>
        public MapChipData[] ChipDatas { get; set; }
    }
}
