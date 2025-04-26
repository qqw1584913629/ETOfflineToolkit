using cfg;
using UnityEngine;

namespace MH
{
    [Event(SceneType.Current)]
    public class SceneChangeFinish_EventView : AEvent<Scene, SceneChangeFinish>
    {
        protected override async ETTask Run(Scene scene, SceneChangeFinish a)
        {
            switch (scene.SceneName)
            {
                case "Game":
                {
                    UnitConfig unitConfig = ConfigsSingleton.Instance.Tables.TbUnitConfig.Get(1);
                    await UnitFactory.CreateUnit(scene, unitConfig);
                    break;
                }
            }
            await ETTask.CompletedTask;
        }
    }
}