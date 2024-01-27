# Localization
ローカライズ周りをサポートする機能です。
* [LocalizeStringBuilder](#localizestringbuilder)

## 導入方法
`Package Manager` で **Add package from git URL** を選択し、以下のURLを入力して下さい。
```
https://github.com/musha-tetsuya/MushaLib-Unity.git?path=Localization
```
* [Install a package from a Git URL](https://docs.unity3d.com/ja/2022.3/Manual/upm-ui-giturl.html)

## LocalizeStringBuilder
ローカライズ文字列の構築をサポートするコンポーネントです。
* 言語切り替え時にシーンの再読み込み無しに文字列を更新します

### 使用方法
1. 前提として、UnityのLocalization機能を利用します。  
   `LocalizationSettings`や`StringTable`などを準備して下さい。  
   参考リンク：https://www.hanachiru-blog.com/entry/2022/03/14/120000
2. TextMeshProUGUIなどのオブジェクトに`LocalizeStringBuilder`コンポーネントを付与します。
3. Inspectorで`UpdateString`のObjectにTextMeshProUGUIオブジェクトを設定し、FunctionにはTextMeshProUGUI.textを設定します。
4. スクリプトから`SetStringBuilder`メソッドで文字列構築処理を設定します。
   ```csharp
   using MushaLib.Localization;
   using MushaLib.Localization.Components;
   using UnityEngine.Localization;
   
   public class Sample : MonoBehaviour
   {
       [SerializeField]
       private LocalizeStringBuilder builder;

       private void Start()
       {
           List<LocalizedString> parameters = new();
           parameters.Add(new ConcatableLocalizedString { TableEntryReference = "KEY_01", ConcatenatingCharacter = "<br>" });
           parameters.Add(new ConcatableLocalizedString { TableEntryReference = "KEY_02", ConcatenatingCharacter = "<br>" });
           parameters.Add(new LocalizedString { TableEntryReference = "KEY_03", Arguments = new List<object>() { "Test", 100 } });

           builder.SetStringBuilder(parameters, destroyCancellationToken);
       }
   }
   ```
   `ConcatableLocalizedString`クラスを用いることで、文字列結合方法を設定出来ます。  
   サンプルでは`"<br>"`を指定し、改行して結合させています。

