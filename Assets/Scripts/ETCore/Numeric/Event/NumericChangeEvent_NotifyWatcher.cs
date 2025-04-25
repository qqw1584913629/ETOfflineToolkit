
namespace MH.Event
{
    [Event(SceneType.Main)]
    public class NumericChangeEvent_NotifyWatcher : AEvent<Scene, NumbericChange>
    {
        protected override async ETTask Run(Scene scene, NumbericChange args)
        {
            if (args.Unit == null)
                return;
            NumericWatcherComponent.Instance.Run(args.Unit, args);
            await ETTask.CompletedTask;
        }
    }
}