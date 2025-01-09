using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization.SmartFormat.PersistentVariables;

namespace MushaLib.Localization
{
    /// <summary>
    /// IVariableを提供するインターフェース
    /// </summary>
    public interface IVariableProvider
    {
        /// <summary>
        /// IVariableの非同期取得
        /// </summary>
        UniTask<IVariable> GetVariableAsync(CancellationToken cancellationToken);
    }
}