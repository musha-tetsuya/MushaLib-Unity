using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor
{
    /// <summary>
    /// マップチップデータ
    /// </summary>
    public class MapChipData
    {
        /// <summary>
        /// インデックス
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// アトラスID：アトラスに含まれているならMapDataのAtlasKeysのIdが入っている
        /// </summary>
        public int? AtlasId { get; set; }

        /// <summary>
        /// スプライトキー：アトラスに含まれているならスプライト名、含まれていないならguid
        /// </summary>
        public string SpriteKey { get; set; }
    }
}
