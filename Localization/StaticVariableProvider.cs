using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MushaLib.Localization
{
    /// <summary>
    /// 静的なIVariableを提供するクラス
    /// </summary>
    public class StaticVariableProvider : IVariableProvider
    {
        /// <summary>
        /// Variable
        /// </summary>
        private readonly IVariable m_Variable;

        /// <summary>
        /// construct
        /// </summary>
        public StaticVariableProvider(IVariable variable)
        {
            m_Variable = variable;
        }

        /// <summary>
        /// IVariableの非同期取得
        /// </summary>
        public UniTask<IVariable> GetVariableAsync(CancellationToken cancellationToken)
        {
            return UniTask.FromResult(m_Variable);
        }
    }
}
