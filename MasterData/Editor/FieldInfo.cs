using System.Collections;
using System.Collections.Generic;

namespace MushaLib.MasterData.Editor
{
    /// <summary>
    /// 変数情報
    /// </summary>
    internal class FieldInfo
    {
        /// <summary>
        /// データ型
        /// </summary>
        public string type;

        /// <summary>
        /// 変数名
        /// </summary>
        public string name;

        /// <summary>
        /// コメント
        /// </summary>
        public string summary;

        /// <summary>
        /// データ位置X
        /// </summary>
        public int posX;
    }
}
