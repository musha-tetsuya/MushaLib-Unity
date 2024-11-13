using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.Utilities
{
    /// <summary>
    /// デリゲートを利用してカスタマイズ可能なJSONコンバーター
    /// </summary>
    public class DelegateJsonConverter<T> : JsonConverter
    {
        /// <summary>
        /// JSON書き込み時
        /// </summary>
        public Action<JsonWriter, T> OnWriteJson { get; set; }

        /// <summary>
        /// JSON読み込み時
        /// </summary>
        public Func<JsonReader, T> OnReadJson { get; set; }

        /// <summary>
        /// 書き込み可能かどうか
        /// </summary>
        public override bool CanWrite => OnWriteJson != null;

        /// <summary>
        /// 読み込み可能かどうか
        /// </summary>
        public override bool CanRead => OnReadJson != null;

        /// <summary>
        /// このコンバーターが適用可能かどうか
        /// </summary>
        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(T);
        }

        /// <summary>
        /// JSON書き込み
        /// </summary>
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (!CanWrite)
            {
                throw new NotImplementedException();
            }

            OnWriteJson(writer, (T)value);
        }

        /// <summary>
        /// JSON読み込み
        /// </summary>
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (!CanRead)
            {
                throw new NotImplementedException();
            }

            return OnReadJson(reader);
        }
    }
}
