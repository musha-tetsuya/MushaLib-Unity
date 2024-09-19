using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace MushaLib.UI
{
    /// <summary>
    /// アトラスのスプライトをキャッシュするクラス
    /// </summary>
    public class SpriteAtlasCache
    {
        /// <summary>
        /// アトラス
        /// </summary>
        private SpriteAtlas m_Atlas;

        /// <summary>
        /// スプライトのテーブル
        /// </summary>
        private Dictionary<string, Sprite> m_SpriteTable = new();
        
        /// <summary>
        /// アトラス
        /// </summary>
        public SpriteAtlas Atlas
        {
            get => m_Atlas;
            set
            {
                if (m_Atlas != value)
                {
                    Clear();
                    m_Atlas = value;
                }
            }
        }

        /// <summary>
        /// スプライトの取得
        /// </summary>
        public Sprite GetSprite(string spriteName)
        {
            if (!m_SpriteTable.TryGetValue(spriteName, out var sprite))
            {
                if (m_Atlas != null)
                {
                    sprite = m_SpriteTable[spriteName] = m_Atlas.GetSprite(spriteName);
                }
            }
            return sprite;
        }

        /// <summary>
        /// クリア
        /// </summary>
        public void Clear()
        {
            m_SpriteTable.Clear();
        }
    }
}
