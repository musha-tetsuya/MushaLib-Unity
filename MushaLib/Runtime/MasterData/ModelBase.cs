using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.MasterData
{
    /// <summary>
    /// マスターデータモデル基底
    /// </summary>
    public abstract class ModelBase<T>
    {
        /// <summary>
        /// ID
        /// </summary>
        public T id;
    }
}
