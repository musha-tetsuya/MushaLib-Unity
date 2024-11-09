using Cysharp.Threading.Tasks;
using MushaLib.StateManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.UI.DQ.MapEditor.State
{
    /// <summary>
    /// 新規作成orデータロードの選択
    /// </summary>
    internal class SelectNewOrLoadState : ValueStateBase<MapEditor>, IGUIState
    {
        /// <summary>
        /// OnGUI
        /// </summary>
        void IGUIState.OnGUI()
        {
            // 新規作成
            if (GUILayout.Button("New"))
            {
                StateManager.ChangeState(new SelectEditElementState(null)).Forget();
            }

            // データロード
            if (GUILayout.Button("Load"))
            {
                var selectLoadDataState = new SelectLoadDataState();
                
                // ロードするデータの選択ステートへ
                StateManager
                    .PushState(selectLoadDataState, () =>
                    {
                        // データが選択された？
                        if (selectLoadDataState.SelectedMapData != null)
                        {
                            // 編集する要素の選択ステートへ
                            StateManager.ChangeState(new SelectEditElementState(selectLoadDataState.SelectedMapData)).Forget();
                        }
                    })
                    .Forget();
            }
        }
    }
}
