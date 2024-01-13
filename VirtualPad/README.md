# VirtualPad
画面上に仮想ゲームパッドを提供するパッケージです。

# 機能
* ボタンを押した時、長押し成立時、リピート時、離した時にイベントを通知
* 画面内に収まらない場合にスケール縮小

# 導入方法
`Package Manager` で **Add package from git URL** を選択し、以下のURLを入力して下さい。
```
https://github.com/musha-tetsuya/MushaLib-Unity.git?path=VirtualPad
```
* [Install a package from a Git URL](https://docs.unity3d.com/ja/2022.3/Manual/upm-ui-giturl.html)

# 使用方法
#### 1. Hierarchy内に `VirtualPad.prefab` を配置して下さい。
![virtualpad_prefab](https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/7d47648e-768f-48b8-b0be-c30f7f0c2551)

#### 2. ボタンを押した時のイベントにはOnPress、離した時のイベントにはOnReleaseを使います。
https://github.com/musha-tetsuya/MushaLib-Unity/blob/a58100d68f6c1bb055b856b5a0dc5830c323cee4/VirtualPad/Runtime/Scripts/VirtualPad.cs#L72-L75
https://github.com/musha-tetsuya/MushaLib-Unity/blob/a58100d68f6c1bb055b856b5a0dc5830c323cee4/VirtualPad/Runtime/Scripts/VirtualPad.cs#L77-L80
```csharp
// Sample.cs
using Cysharp.Threading.Tasks;
using MushaLib.VirtualPad;
using UniRx;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private VirtualPad virtualPad;

    void Start()
    {
        // ボタンを押した時のイベントを購読
        virtualPad.OnPress
            .Subscribe(x =>
            {
                Color color = x.pressPhase switch
                {
                    // 押した瞬間
                    ButtonPressPhase.Pressed => Color.green,
                    // 長押し成立時
                    ButtonPressPhase.LongPressed => Color.yellow,
                    // リピート時
                    ButtonPressPhase.Repeat => Color.red,
                    _ => Color.clear
                };

                Debug.Log($"OnPress : {x.buttonType} : <color=#{ColorUtility.ToHtmlStringRGB(color)}>{x.pressPhase}</color>");
            })
            .AddTo(destroyCancellationToken);

        // ボタンを離した時のイベントを購読
        virtualPad.OnRelease
            .Subscribe(buttonType =>
            {
                Debug.Log($"OnRelease : {buttonTyoe}");
            })
            .AddTo(destroyCancellationToken);
    }
}
```

# サンプル
#### 1. ボタンを押した時、長押し成立時、リピート時、離した時にイベントを通知

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/23df610d-c266-424e-ae14-598c1dca0b07

#### 2. 画面内に収まらない場合にスケール縮小

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/8bfbffb0-4cf4-4035-8d45-7ef0c56278b7





