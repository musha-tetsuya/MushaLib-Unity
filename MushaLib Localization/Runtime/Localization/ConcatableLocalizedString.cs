using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Tables;

namespace MushaLib.Localization
{
    /// <summary>
    /// 連結可能ローカライズ文字列
    /// </summary>
    public class ConcatableLocalizedString : LocalizedString
    {
        /// <summary>
        /// 連結時文字
        /// </summary>
        public string ConcatenatingCharacter { get; set; }

        /// <summary>
        /// construct
        /// </summary>
        public ConcatableLocalizedString()
            : base()
        {
        }

        /// <summary>
        /// construct
        /// </summary>
        public ConcatableLocalizedString(TableReference tableReference, TableEntryReference entryReference, string concatenatingCharacter)
            : base(tableReference, entryReference)
        {
            ConcatenatingCharacter = concatenatingCharacter;
        }
    }
}