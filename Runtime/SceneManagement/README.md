# SceneManagement

## GameStartSceneHelper
ゲームの開始シーンに関連する補助機能の提供。  
Editor再生時、BuildSettingsに登録されている一番最初のシーンを経由してから、開いていたシーンに遷移するようになります。  
作成中のシーンをテストしたいが、マスターデータの読み込み等が必要で、いちいちSetupシーンを経由していかなければいけない、というような手間を省くことが出来ます。

* **HandlePlayModeScene**  
  `EditorSceneManager.playModeStartScene`に初期化シーンを設定し、初期化シーン経由後に開くシーンとして現在のシーンを保存します。
  https://github.com/musha-tetsuya/MushaLib-Unity/blob/d3cc52650c1e9e94ceb07c6363271546fa728345/SceneManagement/Scripts/GameStartSceneHelper.cs#L28
```csharp
#if UNITY_EDITOR
    [InitializeOnLoadMethod]
    private static void SetPlayModeStateChange()
    {
        EditorApplication.playModeStateChange += GameStartSceneHelper.HandlePlayModeScene;
    }
#endif
```

* **LoadStartSceneAsync**  
  Editor時は`HandlePlayModeScene`で保存された初期化シーン経由後のシーンに遷移します。  
  Editorじゃないときは、BuildSettingsに登録されている2番目のシーンに遷移します。
  https://github.com/musha-tetsuya/MushaLib-Unity/blob/d3cc52650c1e9e94ceb07c6363271546fa728345/SceneManagement/Scripts/GameStartSceneHelper.cs#L84
```csharp
using MushaLib.SceneManagement;

/// <summary>
/// ゲーム起動時に絶対に通りたい画面
/// </summary>
public class MyGameStartup : MonoBehaviour
{
    private async void Start()
    {
        // マスターデータのロード等、ゲームの開始に必要なリソースを読み込む
        await LoadMasterData();

        // Editorの再生ボタンを押したときに開いていたシーンに遷移する
        // Editorじゃないときは、BuildSettingsに登録されている2番目のシーンに遷移する
        _ = GameStartSceneHelper.LoadStartSceneAsync();
    }
}
```
