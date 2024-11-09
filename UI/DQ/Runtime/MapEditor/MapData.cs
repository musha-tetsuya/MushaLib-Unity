using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace MushaLib.UI.DQ.MapEditor
{
    /// <summary>
    /// マップデータ
    /// </summary>
    public class MapData : ScriptableObject
    {
        /// <summary>
        /// マップサイズ
        /// </summary>
        [SerializeField]
        public Vector2Int Size = new(16, 16);

        /// <summary>
        /// スプライト
        /// </summary>
        [SerializeField]
        public AssetReferenceSprite[] Sprites;
    }
}
