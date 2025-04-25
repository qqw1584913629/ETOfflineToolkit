using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class OperaComponentDestroySystem : DestroySystem<OperaComponent>
    {
        protected override void Destroy(OperaComponent self)
        {
        }
    }
    [EntitySystem]
    public class OperaComponentAwakeSystem : AwakeSystem<OperaComponent>
    {
        protected override void Awake(OperaComponent self)
        {
        }
    }
    [EntitySystem]
    public class OperaComponentUpdateSystem : UpdateSystem<OperaComponent>
    {
        protected override void Update(OperaComponent self)
        {
            if (Input.GetMouseButtonDown(0))
            {
            }
        }
    }

    public static class OperaComponentSystem
    {
    }
}