using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.SceneManagement
{
    /// <summary>
    /// シーンインターフェース
    /// </summary>
    public interface IScene
    {
        /// <summary>
        /// シーンの再アクティブ時
        /// </summary>
        void OnSceneReactivated();
    }
}
