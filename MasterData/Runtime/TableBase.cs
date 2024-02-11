using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.MasterData
{
    /// <summary>
    /// マスターデータテーブル基底
    /// </summary>
    public abstract class TableBase<TInstance, TKey, TValue> : Dictionary<TKey, TValue>, ITableBase
        where TInstance : TableBase<TInstance, TKey, TValue>, new()
        where TValue : ModelBase<TKey>
    {
        /// <summary>
        /// インスタンス
        /// </summary>
        public static TInstance Instance { get; } = new();

        /// <summary>
        /// Json読み込み
        /// </summary>
        public virtual void Load(string json)
        {
            foreach (var model in JsonConvert.DeserializeObject<TValue[]>(json))
            {
                Add(model.id, model);
            }
        }
    }
}
