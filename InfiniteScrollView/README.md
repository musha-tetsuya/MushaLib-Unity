# InfiniteScrollView
Unityのスクロールビューを拡張するパッケージです。

## 機能
* 大量のスクロール要素を少ないゲームオブジェクト数で実現
* ページレイアウト
* 無限ループ
* スナップスクロール
* 指定要素へのジャンプ
* 指定要素へのスクロール

## 導入方法
`Package Manager` で **Add package from git URL** を選択し、以下のURLを入力して下さい。
```
https://github.com/musha-tetsuya/MushaLib-Unity.git?path=InfiniteScrollView
```
* [Install a package from a Git URL](https://docs.unity3d.com/ja/2022.3/Manual/upm-ui-giturl.html)

## 使用方法
#### 1. Hierarchy内に右クリックで Scroll View を配置してください。
<img src="https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/1ef09270-ee2f-450c-9976-d66c0b766630" width="50%" height="50%">

#### 2. 配置したScroll ViewにInfiniteScrollViewをAddComponentします。
<img src="https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/6c5a183f-c26e-4401-8735-f92f1f299379" width="50%" height="50%">

#### 3. InfiniteScrollViewコンポーネントのInspectorを設定します。
| 項目 | 概要 |
| --- | --- |
| ElementPrefab | 要素プレハブ |
| ElementCount | 要素数 |
| PageLayout | ページレイアウト |
| Spacing | ページ間スペース |
| StartAxis | ページの配置方向 |
| Padding | Viewport内側の余白 |
| Margin | Viewport外側の余白 |
| Loop | ページをループさせるかどうか |
| SnapType | スナップタイプ |
| SnapThrethold | スナップ開始の閾値 |
| SnapDuration | スナップ時間 |

#### 4. スクリプトからInfiniteScrollViewのInitializeメソッドを呼び出して下さい。
```csharp
using MushaLib.InfiniteScrollView;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private InfiniteScrollView scrollView;

    private void Start()
    {
        scrollView.Initialize();
    }
}
```

#### 5. 要素更新時イベントにはOnUpdateElementを使います。
https://github.com/musha-tetsuya/MushaLib-Unity/blob/1989550c5fb6266b6aa444a9373c68940eaac83e/InfiniteScrollView/Runtime/InfiniteScrollView.cs#L289-L292

## サンプル
#### 基本

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/72f192a4-9f07-4411-a12b-d980b5d4e90e

* 設定値




