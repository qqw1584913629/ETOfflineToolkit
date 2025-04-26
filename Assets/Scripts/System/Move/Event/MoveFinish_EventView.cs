using System;

namespace MH
{
    [Event(SceneType.Current)]
    public class MoveFinish_EventView : AEvent<Scene, MoveFinish>
    {
        protected override async ETTask Run(Scene scene, MoveFinish a)
        {
            var unit = UnitHelper.GetUnitByZoneScene(scene.Root);
            unit.GetComponent<AnimatorComponent>().Play(MotionType.Idle);
            await ETTask.CompletedTask;
        }
    }
}
