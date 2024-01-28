using System.Collections;
using System.Collections.Generic;

namespace MushaLib.MasterData.Editor
{
    /// <summary>
    /// ヘッダー
    /// </summary>
    internal enum Header
    {
        Description = 1,    //概要
        IsOutputCs,         //cs出力するかどうか
        IsOutputJson,       //json出力するかどうか
        IsOutputCsv,        //csv出力するかどうか
        DataType,           //データ型
        Summary,            //コメント
        Length
    }
}
