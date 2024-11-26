using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.SelectableList
{
    /// <summary>
    /// 選択可能リストモデル
    /// </summary>
    public class SelectableListModel
    {
        /// <summary>
        /// 開始時インデックス
        /// </summary>
        public int StartIndex { get; set; }

        /// <summary>
        /// 現在選択中のインデックス
        /// </summary>
        public int CurrentIndex { get; set; }
    }
}
