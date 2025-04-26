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
            self.TriggerMask |= LayerMask.GetMask("Ground");
            self.MainCamera = Camera.main;
        }
    }
    [EntitySystem]
    public class OperaComponentUpdateSystem : UpdateSystem<OperaComponent>
    {
        protected override void Update(OperaComponent self)
        {
            if (Input.GetMouseButtonDown(0))
            {
                Ray ray = self.MainCamera.ScreenPointToRay(Input.mousePosition);
                RaycastHit hit;
                
                if (Physics.Raycast(ray, out hit, Mathf.Infinity, self.TriggerMask))
                {
                    EventSystem.Instance.PublishAsync(self.Scene, new OperaTrigger()
                    {
                        X = hit.point.x,
                        Y = hit.point.y,
                        Z = hit.point.z
                    }).Coroutine();
                }
            }
        }
    }

    public static class OperaComponentSystem
    {
    }
}