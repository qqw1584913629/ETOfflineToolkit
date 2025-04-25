
namespace MH
{
    [Event(SceneType.Current)]
    public class AfterCreateCurrentScene_EventView : AEvent<Scene, AfterCreateCurrentScene>
    {
        protected override async ETTask Run(Scene scene, AfterCreateCurrentScene args)
        {
            scene.AddComponent<UIComponent>();
            scene.AddComponent<ResourcesComponent>();
            scene.AddComponent<CoroutineLockComponent>();
            scene.AddComponent<TimerComponent>();
            scene.AddComponent<OperaComponent>();
            await ETTask.CompletedTask;
        }
    }
}