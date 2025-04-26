using System;

namespace MH
{
    [Event(SceneType.Current)]
    public class MoveStart_EventView : AEvent<Scene, MoveStart>
    {
        protected override async ETTask Run(Scene scene, MoveStart a)
        {
            var unit = UnitHelper.GetUnitByZoneScene(scene.Root);
            unit.GetComponent<AnimatorComponent>().Play(MotionType.Run);
            await ETTask.CompletedTask;
        }
    }
}
