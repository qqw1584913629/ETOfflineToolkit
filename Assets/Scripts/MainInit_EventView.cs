
namespace MH
{
    [Event(SceneType.Main)]
    public class MainInit_EventView : AEvent<Scene, Main_Init>
    {
        protected override async ETTask Run(Scene scene, Main_Init a)
        {
            scene.AddComponent<GlobalComponent>();
            scene.AddComponent<CoroutineLockComponent>();
            scene.AddComponent<ResourcesComponent>();
            scene.AddComponent<UIPathComponent>();
            scene.AddComponent<UIEventComponent>();
            scene.AddComponent<CurrentSceneComponent>();
            scene.AddComponent<PlayerInfoComponent>();
            var uiComponent = scene.AddComponent<UIComponent>();

            scene.AddComponent<UnitComponent>();
            scene.AddComponent<AudioComponent>();
            uiComponent.ShowWindow(WindowID.WindowID_Login);
            // await SceneChangeHelper.SceneChangeTo(scene, "Game");
            await ETTask.CompletedTask;
        }
    }
}