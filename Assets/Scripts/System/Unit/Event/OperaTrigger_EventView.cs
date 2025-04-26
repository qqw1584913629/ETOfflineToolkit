using System;

namespace MH
{
    [Event(SceneType.Current)]
    public class OperaTrigger_EventView : AEvent<Scene, OperaTrigger>
    {
        protected override async ETTask Run(Scene scene, OperaTrigger a)
        {
            var unit = UnitHelper.GetUnitByZoneScene(scene.Root);
            unit.GetComponent<MoveComponent>().MoveTo(a.X, a.Y, a.Z);
            await ETTask.CompletedTask;
        }
    }
}
