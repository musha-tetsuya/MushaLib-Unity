using Newtonsoft.Json;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.MasterData
{
    /// <summary>
    /// マスターデータテーブル基底
    /// </summary>
    public abstract class TableBase<TKey, TValue> : IReadOnlyDictionary<TKey, TValue>, ITableBase
        where TValue : ModelBase<TKey>
    {
        /// <summary>
        /// テーブル
        /// </summary>
        protected readonly Dictionary<TKey, TValue> m_Table = new();

        /// <summary>
        /// 指定されたキーに対応する値を取得する
        /// </summary>
        public TValue this[TKey key] => m_Table[key];

        /// <summary>
        /// テーブル内の全てのキーを取得する
        /// </summary>
        public IEnumerable<TKey> Keys => m_Table.Keys;

        /// <summary>
        /// テーブル内の全ての値を取得する
        /// </summary>
        public IEnumerable<TValue> Values => m_Table.Values;

        /// <summary>
        /// テーブル内の要素数を取得する
        /// </summary>
        public int Count => m_Table.Count;

        /// <summary>
        /// 指定されたキーがテーブル内に存在するかを確認する
        /// </summary>
        public bool ContainsKey(TKey key)
        {
            return m_Table.ContainsKey(key);
        }

        /// <summary>
        /// テーブルの列挙子を取得する
        /// </summary>
        public IEnumerator<KeyValuePair<TKey, TValue>> GetEnumerator()
        {
            return m_Table.GetEnumerator();
        }

        /// <summary>
        /// 指定されたキーに関連付けられている値を取得する
        /// </summary>
        public bool TryGetValue(TKey key, out TValue value)
        {
            return m_Table.TryGetValue(key, out value);
        }

        /// <summary>
        /// テーブルの列挙子を取得する（非ジェネリック版）
        /// </summary>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Table.GetEnumerator();
        }

        /// <summary>
        /// Json読み込み
        /// </summary>
        public virtual void Load(string json)
        {
            foreach (var model in JsonConvert.DeserializeObject<TValue[]>(json))
            {
                m_Table.Add(model.id, model);
            }
        }
    }
}
