# InfiniteScrollView
Unityのスクロールビューを拡張する機能です。
* 大量のスクロール要素を少ないゲームオブジェクト数で実現
* ページレイアウト
* 無限ループ
* スナップスクロール
* 指定要素へのジャンプ
* 指定要素へのスクロール

## 使用方法
#### 1. Hierarchy内に右クリックで Scroll View を配置してください。
<img src="https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/1ef09270-ee2f-450c-9976-d66c0b766630" width="50%" height="50%">

#### 2. 配置したScroll ViewにInfiniteScrollViewをAddComponentします。
<img src="https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/6c5a183f-c26e-4401-8735-f92f1f299379" width="50%" height="50%">

#### 3. InfiniteScrollViewコンポーネントのInspectorを設定します。
| 項目          | 概要
| ---           | ---
| ElementPrefab | 要素プレハブ
| ElementCount  | 要素数
| PageLayout    | ページレイアウト
| Spacing       | ページ間スペース
| StartAxis     | ページの配置方向
| Padding       | Viewport内側の余白
| Margin        | Viewport外側の余白
| Loop          | ページをループさせるかどうか
| SnapType      | スナップタイプ
| SnapDuration  | スナップ時間

`ElementPrefab`には[ScrollElement](https://github.com/musha-tetsuya/MushaLib-Unity/blob/master/InfiniteScrollView/Runtime/ScrollElement.cs)を継承したコンポーネントが付与されたプレハブをセットして下さい。

#### 4. スクリプトからInfiniteScrollViewのInitializeメソッドを呼び出して下さい。
```csharp
using MushaLib.UI.InfiniteScrollView;

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

```csharp
// SampleElement.cs
using MushaLib.UI.InfiniteScrollView;
using TMPro;

public class SampleElement : ScrollElement
{
    [SerializeField]
    private TextMeshProUGUI nameText;

    public TextMeshProUGUI NameText => nameText;
}
```

```csharp
// Sample.cs
using MushaLib.UI.InfiniteScrollView;

public class Sample : MonoBehaviour
{
    [SerializeField]
    private InfiniteScrollView scrollView;

    [SerializeField]
    private SampleElement elementPrefab;

    private string[] elementNames =
    {
        "aaa",
        "bbb",
        "ccc",
    };

    private void Start()
    {
        scrollView.ElementPrefab = elementPrefab;

        scrollView.ElementCount = elementNames.Length;

        scrollView.OnUpdateElement = (element, index) =>
        {
            (element as SampleElement).NameText.text = elementNames[index];
        };

        scrollView.Initialize();
    }
}
```

## サンプル
#### 1. 基本

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/72f192a4-9f07-4411-a12b-d980b5d4e90e

* 設定値

<table align="left">
    <tr><th>ScrollRect</th></tr>
    <tr><td>Horizontal = true</td></tr>
    <tr><td>Vertical = false</td></tr>
    <tr><td>MovementType = Elastic</td></tr>
    <tr><td><details>
        <summary>Content</summary>
        &emsp;AnchorMin = (0, 0.5)<br>
        &emsp;AnchorMax = (0, 0.5)<br>
        &emsp;Pivot = (0.5, 0.5)
    </details></td></tr>
</table>

<table clear="all">
    <tr><th>InfiniteScrollView</th></tr>
    <tr><td>ElementCount = 100</td></tr>
    <tr><td><details>
        <summary>PageLayout</summary>
        &emsp;CellCount = (1, 1)<br>
        &emsp;Spacing = (0, 0)<br>
        &emsp;StartCorner = UpperLeft<br>
        &emsp;StartAxis = Horizontal
    </details></td></tr>
    <tr><td>Spacing = (50, 50)</td></tr>
    <tr><td>StartAxis = Horizontal</td></tr>
    <tr><td><details>
        <summary>Padding</summary>
        &emsp;Left = 20<br>
        &emsp;Right = 20<br>
        &emsp;Top = 20<br>
        &emsp;Bottom = 20
    </details></td></tr>
    <tr><td><details>
        <summary>Margin</summary>
        &emsp;Left = 0<br>
        &emsp;Right = 0<br>
        &emsp;Top = 0<br>
        &emsp;Bottom = 0
    </details></td></tr>
    <tr><td>Loop = false</td></tr>
    <tr><td>SnapType = None</td></tr>
</table>

#### 2. ページレイアウト、無限スクロール

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/b8a2b5c0-9d7c-4493-8c77-2f7461a1c7e1

* 設定値

<table align="left">
    <tr><th>ScrollRect</th></tr>
    <tr><td>Horizontal = false</td></tr>
    <tr><td>Vertical = true</td></tr>
    <tr><td>MovementType = Unrestricted</td></tr>
    <tr><td><details>
        <summary>Content</summary>
        &emsp;AnchorMin = (0, 1)<br>
        &emsp;AnchorMax = (0, 1)<br>
        &emsp;Pivot = (0, 0.5)
    </details></td></tr>
</table>

<table clear="all">
    <tr><th>InfiniteScrollView</th></tr>
    <tr><td>ElementCount = 100</td></tr>
    <tr><td><details>
        <summary>PageLayout</summary>
        &emsp;CellCount = (3, 2)<br>
        &emsp;Spacing = (10, 10)<br>
        &emsp;StartCorner = UpperLeft<br>
        &emsp;StartAxis = Vertical
    </details></td></tr>
    <tr><td>Spacing = (50, 50)</td></tr>
    <tr><td>StartAxis = Horizontal</td></tr>
    <tr><td><details>
        <summary>Padding</summary>
        &emsp;Left = 20<br>
        &emsp;Right = 20<br>
        &emsp;Top = 20<br>
        &emsp;Bottom = 20
    </details></td></tr>
    <tr><td><details>
        <summary>Margin</summary>
        &emsp;Left = 0<br>
        &emsp;Right = 0<br>
        &emsp;Top = 0<br>
        &emsp;Bottom = 0
    </details></td></tr>
    <tr><td>Loop = true</td></tr>
    <tr><td>SnapType = None</td></tr>
</table>

#### 3. 縦横無限スクロール

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/8735f074-1ee2-4b9d-894f-38662e915db7

* 設定値

<table align="left">
    <tr><th>ScrollRect</th></tr>
    <tr><td>Horizontal = true</td></tr>
    <tr><td>Vertical = true</td></tr>
    <tr><td>MovementType = Unrestricted</td></tr>
    <tr><td><details>
        <summary>Content</summary>
        &emsp;AnchorMin = (0, 1)<br>
        &emsp;AnchorMax = (0, 1)<br>
        &emsp;Pivot = (0, 1)
    </details></td></tr>
</table>

<table clear="all">
    <tr><th>InfiniteScrollView</th></tr>
    <tr><td>ElementCount = 100</td></tr>
    <tr><td><details>
        <summary>PageLayout</summary>
        &emsp;CellCount = (3, 3)<br>
        &emsp;Spacing = (10, 10)<br>
        &emsp;StartCorner = UpperLeft<br>
        &emsp;StartAxis = Horizontal
    </details></td></tr>
    <tr><td>Spacing = (50, 50)</td></tr>
    <tr><td>StartAxis = Horizontal</td></tr>
    <tr><td><details>
        <summary>Padding</summary>
        &emsp;Left = 20<br>
        &emsp;Right = 20<br>
        &emsp;Top = 20<br>
        &emsp;Bottom = 20
    </details></td></tr>
    <tr><td><details>
        <summary>Margin</summary>
        &emsp;Left = 0<br>
        &emsp;Right = 0<br>
        &emsp;Top = 0<br>
        &emsp;Bottom = 0
    </details></td></tr>
    <tr><td>Loop = true</td></tr>
    <tr><td>SnapType = None</td></tr>
</table>

#### 4. スナップスクロール、指定ページへのジャンプ、指定ページへのスクロール

https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/e7f27761-e4ed-4804-bab1-041adf1fcb6e

* 設定値

<table align="left">
    <tr><th>ScrollRect</th></tr>
    <tr><td>Horizontal = true</td></tr>
    <tr><td>Vertical = false</td></tr>
    <tr><td>MovementType = Elastic</td></tr>
    <tr><td><details>
        <summary>Content</summary>
        &emsp;AnchorMin = (0, 0.5)<br>
        &emsp;AnchorMax = (0, 0.5)<br>
        &emsp;Pivot = (0.5, 0.5)
    </details></td></tr>
</table>

<table clear="all">
    <tr><th>InfiniteScrollView</th></tr>
    <tr><td>ElementCount = 100</td></tr>
    <tr><td><details>
        <summary>PageLayout</summary>
        &emsp;CellCount = (2, 4)<br>
        &emsp;Spacing = (10, 10)<br>
        &emsp;StartCorner = UpperLeft<br>
        &emsp;StartAxis = Horizontal
    </details></td></tr>
    <tr><td>Spacing = (50, 50)</td></tr>
    <tr><td>StartAxis = Horizontal</td></tr>
    <tr><td><details>
        <summary>Padding</summary>
        &emsp;Left = 295<br>
        &emsp;Right = 295<br>
        &emsp;Top = 20<br>
        &emsp;Bottom = 20
    </details></td></tr>
    <tr><td><details>
        <summary>Margin</summary>
        &emsp;Left = 0<br>
        &emsp;Right = 0<br>
        &emsp;Top = 0<br>
        &emsp;Bottom = 0
    </details></td></tr>
    <tr><td>Loop = false</td></tr>
    <tr><td>SnapType = Page</td></tr>
    <tr><td>SnapDuration = 0.1</td></tr>
</table>

* 指定ページへジャンプする関数
https://github.com/musha-tetsuya/MushaLib-Unity/blob/d9a88db43781c814d716946f1729b9c6a3b0eff9/InfiniteScrollView/Runtime/InfiniteScrollView.cs#L1095-L1098

* 指定ページへスクロールする関数
https://github.com/musha-tetsuya/MushaLib-Unity/blob/d9a88db43781c814d716946f1729b9c6a3b0eff9/InfiniteScrollView/Runtime/InfiniteScrollView.cs#L1139-L1142
