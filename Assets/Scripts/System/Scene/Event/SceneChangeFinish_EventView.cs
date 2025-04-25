using UnityEngine;

namespace MH
{
    [Event(SceneType.Current)]
    public class SceneChangeFinish_EventView : AEvent<Scene, SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, SceneChangeFinish a)
        {
            await ETTask.CompletedTask;
        }
    }
}