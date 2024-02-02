using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.MasterData
{
    /// <summary>
    /// マスターデータテーブルインターフェース
    /// </summary>
    public interface ITableBase
    {
        /// <summary>
        /// Json読み込み
        /// </summary>
        void Load(string json);
    }
}
