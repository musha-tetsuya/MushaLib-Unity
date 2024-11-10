using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor
{
    /// <summary>
    /// マップデータ
    /// </summary>
    public class MapData
    {
        /// <summary>
        /// サイズX
        /// </summary>
        public int SizeX { get; set; }

        /// <summary>
        /// サイズY
        /// </summary>
        public int SizeY { get; set; }

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
