using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MushaLib.SceneManagement
{
    /// <summary>
    /// ゲームの開始シーンに関連する補助機能を提供
    /// </summary>
    public static class GameStartSceneHelper
    {
        /// <summary>
        /// 起動シーンの名前の保存先
        /// </summary>
        private static readonly string startSceneNameFilePath = $"Temp/{typeof(GameStartSceneHelper).FullName.Replace(".", "-")}-StartSceneName.txt";

        /// <summary>
        /// 再生開始シーンの制御
        /// </summary>
        /// <remarks>
        /// 使用方法：
        /// InitializeOnLoadMethodでEditorApplication.playModeStateChangedにセットして使用する
        /// </remarks>
        public static void HandlePlayModeScene(PlayModeStateChange state)
        {
#if UNITY_EDITOR
            // 再生後は受け付けない
            if (state == PlayModeStateChange.EnteredPlayMode)
            {
                return;
            }

            // 再生開始シーンと起動シーンをクリア
            EditorSceneManager.playModeStartScene = null;
            File.Delete(startSceneNameFilePath);

            // 再生開始時のみ処理
            if (state != PlayModeStateChange.ExitingEditMode)
            {
                return;
            }

            // 現在のシーン
            var currentScene = SceneManager.GetActiveScene();

            // 現在のシーンがEditorBuildSettingsに含まれていない場合、初期化シーンを経由したくないのでスルーする
            if (!EditorBuildSettings.scenes.Select(x => x.path).Contains(currentScene.path))
            {
                return;
            }

            // EditorBuildSettings.scenesに一つもシーンが登録されていない場合、初期化シーンを取得出来ないのでスルーする
            var initialScene = EditorBuildSettings.scenes.FirstOrDefault();
            if (initialScene == null)
            {
                return;
            }

            // 現在のシーンが初期化シーンの場合、次のシーンはデフォルトなのでスルーする
            if (currentScene.path == initialScene.path)
            {
                return;
            }

            // 初期化シーンを再生開始シーンとして設定
            EditorSceneManager.playModeStartScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(initialScene.path);

            // 初期化シーン経由後に開くシーンとして、現在のシーンの名前を保存
            File.WriteAllText(startSceneNameFilePath, currentScene.name);
#endif
        }

        /// <summary>
        /// 起動シーンを読み込む
        /// </summary>
        /// <remarks>
        /// 使用方法：
        /// ゲームの初期化画面（スプラッシュ画面等）の終了後に呼び、開始画面（タイトル画面等）に遷移させるときに使う
        /// </remarks>
        public static AsyncOperation LoadStartSceneAsync(string startSceneName = null)
        {
#if UNITY_EDITOR
            if (File.Exists(startSceneNameFilePath))
            {
                var op = SceneManager.LoadSceneAsync(File.ReadAllText(startSceneNameFilePath));
                if (op != null)
                {
                    return op;
                }
            }
#endif
            if (!string.IsNullOrEmpty(startSceneName))
            {
                var op = SceneManager.LoadSceneAsync(startSceneName);
                if (op != null)
                {
                    return op;
                }
            }

            for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
            {
                // BuildSettingsの番号が最も若いシーンに遷移
                if (i != SceneManager.GetActiveScene().buildIndex)
                {
                    var op = SceneManager.LoadSceneAsync(i);
                    return op;
                }
            }

            return null;
        }
    }
}