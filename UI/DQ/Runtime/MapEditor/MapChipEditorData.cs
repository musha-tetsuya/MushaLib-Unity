using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor
{
    /// <summary>
    /// マップチップ編集データ
    /// </summary>
    [Serializable]
    public class MapChipEditorData
    {
        /// <summary>
        /// スプライト
        /// </summary>
        [SerializeField]
        private Sprite m_Sprite;

        /// <summary>
        /// コリジョン
        /// </summary>
        [SerializeField]
        private int m_CollisionNum;

        /// <summary>
        /// スプライト
        /// </summary>
        public Sprite Sprite
        {
            get => m_Sprite;
            set => m_Sprite = value;
        }

        /// <summary>
        /// コリジョン
        /// </summary>
        public int CollisionNum
        {
            get => m_CollisionNum;
            set => m_CollisionNum = value;
        }
    }
}
