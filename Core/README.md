# Core
コアとなる機能を提供します。
* [SingletonMonoBehaviour](#singletonmonobehaviour)

## SingletonMonoBehaviour
シングルトンパターンのMonoBehaviourを実装するための抽象クラス

### 使用方法
シングルトン化したいMonoBehaviourに`SingletonMonoBehaviour<T>`を継承させて、`Unity.VisualScripting.SingletonAttribute`を付与して下さい。
* Attributeの`Automatic`をtrueにすると、Instanceへのアクセス時にインスタンが存在しなかったら自動でインスタンスを作成します。
* Attributeの`Persistent`をtrueにすると、DontDestroyOnLoadになります。
```csharp
// MySingleton.cs
using MushaLib.Core;
using Unity.VisualScripting;

[Singleton(Automatic = true, Persistent = true)]
public class MySingleton : SingletonMonoBehaviour<MySingleton>
{
    public void Log()
    {
        Debug.Log("MySingleton");
    }
}
```
```csharp
// Sample.cs
public class Sample : MonoBehaviour
{
    private void Start()
    {
        MySingleton.Instance.Log();
    }
}
```
