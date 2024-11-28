using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MushaLib.DQ.PlayerMovement
{
    /// <summary>
    /// コリジョンデータ提供インターフェース
    /// </summary>
    public interface ICollisionDataProvider
    {
        /// <summary>
        /// 指定ローカル座標のコリジョンデータ取得
        /// </summary>
        int GetCollisionDataAtLocalPoint(Vector2 localPoint);
    }
}
