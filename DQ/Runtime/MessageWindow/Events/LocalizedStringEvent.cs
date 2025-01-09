using Cysharp.Threading.Tasks;
using MushaLib.Localization;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.SmartFormat.PersistentVariables;
using UnityEngine.Localization.Tables;

namespace MushaLib.DQ.MessageWindow.Events
{
    /// <summary>
    /// メッセージウィンドウローカライズ文字列イベント
    /// </summary>
    public class LocalizedStringEvent : StringEventBase
    {
        /// <summary>
        /// ローカライズ文字列
        /// </summary>
        private readonly LocalizedString m_LocalizedString;

        /// <summary>
        /// Variableリスト
        /// </summary>
        private readonly List<IVariableProvider> m_VariableProviders = new();

        /// <summary>
        /// Variableテーブル
        /// </summary>
        private readonly Dictionary<string, IVariableProvider> m_VariableProviderTable = new();

        /// <summary>
        /// construct
        /// </summary>
        public LocalizedStringEvent(LocalizedString localizedString)
        {
            m_LocalizedString = localizedString;
        }

        /// <summary>
        /// construct
        /// </summary>
        public LocalizedStringEvent(TableReference tableReference, TableEntryReference tableEntryReference)
            : this(new LocalizedString(tableReference, tableEntryReference))
        {
        }

        /// <summary>
        /// 文字列取得
        /// </summary>
        public override async UniTask<string> GetString(CancellationToken cancellationToken)
        {
            var tasks = new List<UniTask>();

            if (m_VariableProviders.Count > 0)
            {
                m_LocalizedString.Arguments = null;

                tasks.Add(UniTask.Create(async () =>
                {
                    var results = await UniTask.WhenAll(m_VariableProviders.Select(async (x, i) => (variable: await x.GetVariableAsync(cancellationToken), index: i)));

                    m_LocalizedString.Arguments = results.OrderBy(x => x.index).Select(x => x.variable.GetSourceValue(null)).ToList();
                }));
            }

            if (m_VariableProviderTable.Count > 0)
            {
                m_LocalizedString.Clear();

                tasks.AddRange(m_VariableProviderTable.Select(async x =>
                {
                    var variable = await x.Value.GetVariableAsync(cancellationToken);

                    m_LocalizedString.Add(x.Key, variable);
                }));
            }

            if (tasks.Count > 0)
            {
                await UniTask.WhenAll(tasks);
            }

            return await m_LocalizedString.GetLocalizedStringAsync().ToUniTask(cancellationToken: cancellationToken);
        }

        /// <summary>
        /// VariableProvider追加
        /// </summary>
        public void AddVariableProvider(IVariableProvider variableProvider)
        {
            m_VariableProviders.Add(variableProvider);
        }

        /// <summary>
        /// VariableProvider追加
        /// </summary>
        public void AddVariableProvider(string name, IVariableProvider variableProvider)
        {
            m_VariableProviderTable[name] = variableProvider;
        }

        /// <summary>
        /// Variable追加
        /// </summary>
        public void AddVariable(IVariable variable)
        {
            AddVariableProvider(new StaticVariableProvider(variable));
        }

        /// <summary>
        /// Variable追加
        /// </summary>
        public void AddVariable(string name, IVariable variable)
        {
            AddVariableProvider(name, new StaticVariableProvider(variable));
        }
    }
}