using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MushaLib.SceneManagement
{
    /// <summary>
    /// シーン制御
    /// </summary>
    public static class SceneController
    {
        /// <summary>
        /// シーン遷移
        /// </summary>
        public static async UniTask ChangeScene(string sceneName, bool unloadCurrentScene = false)
        {
            Scene emptyScene = default;

            if (unloadCurrentScene)
            {
                if (SceneManager.sceneCount == 1)
                {
                    // シーンが一つしかないなら、空シーンを作成
                    emptyScene = SceneManager.CreateScene("Empty");
                }

                var currentScene = SceneManager.GetActiveScene();

                // 現在のシーンをアンロード
                await SceneManager.UnloadSceneAsync(currentScene);
            }

            var nextScene = SceneManager.GetSceneByName(sceneName);

            // 遷移先のシーンがロード済みなら
            if (nextScene.IsValid())
            {
                SceneManager.SetActiveScene(nextScene);

                foreach (var gobj in nextScene.GetRootGameObjects())
                {
                    if (gobj.TryGetComponent<IScene>(out var scene))
                    {
                        // 再アクティブ化されたことを通知
                        scene.OnSceneReactivated();
                    }
                }
            }
            else
            {
                // シーンをロード
                await SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);

                nextScene = SceneManager.GetSceneByName(sceneName);

                SceneManager.SetActiveScene(nextScene);
            }

            if (emptyScene.IsValid())
            {
                // 遷移完了したので空シーンは破棄
                await SceneManager.UnloadSceneAsync(emptyScene);
            }
        }
    }
}
