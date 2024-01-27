# StateManagement
ステートの管理に関する機能を提供します。
* [IState](#istate)
* [StateManager](#statemanager)

## IState
ステートのインターフェース

## StateManager
ステート遷移を管理するクラスです。

### サンプル
https://github.com/musha-tetsuya/MushaLib-Unity/assets/26340083/7e78af44-b9b7-4f5a-a938-c1a5e31940a5

```csharp
using MushaLib.StateManagement;
using UnityEngine;

public class SampleScene : MonoBehaviour
{
    private StateManager stateManager = new();

    private void OnGUI()
    {
        if (stateManager.CurrentState is MyState myState)
        {
            GUILayout.Label($"CurrentState is {myState.GetType().Name}");

            myState.OnGUI();
        }
        else
        {
            GUILayout.Label("CurrentState is null");

            if (GUILayout.Button("PushState -> MyStateA"))
            {
                stateManager.PushState(new MyStateA { scene = this });
            }
        }
    }

    private abstract class MyState : IState
    {
        public SampleScene scene { get; set; }

        public void Start()
        {
            Debug.Log($"Start {GetType().Name}");
        }

        public void End()
        {
            Debug.Log($"End {GetType().Name}");
        }

        public abstract void OnGUI();
    }

    private class MyStateA : MyState
    {
        public override void OnGUI()
        {
            if (GUILayout.Button("PushState -> MyStateB"))
            {
                scene.stateManager.PushState(new MyStateB { scene = this.scene });
            }

            if (GUILayout.Button("ChangeState -> Null"))
            {
                scene.stateManager.ChangeState(null);
            }

            if (GUILayout.Button("PopState -> Null"))
            {
                scene.stateManager.PopState();
            }
        }
    }

    private class MyStateB : MyState
    {
        public override void OnGUI()
        {
            if (GUILayout.Button("PopState -> MyStateA"))
            {
                scene.stateManager.PopState();
            }
        }
    }
}

```
