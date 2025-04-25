
namespace MH
{
    public static class SceneChangeHelper
    {
        /// <summary>
        /// 场景切换
        /// </summary>
        /// <param name="root">根实体</param>
        /// <param name="sceneName">场景名称</param>
        public static async ETTask SceneChangeTo(Scene root, string sceneName)
        {
            CurrentSceneComponent currentSceneComponent = root.GetComponent<CurrentSceneComponent>();
            currentSceneComponent.Scene?.Dispose(); // 删除之前的CurrentScene，创建新的
            Scene currentScene = CurrentSceneFactory.Create(IdGenerater.GenerateId(), sceneName, currentSceneComponent);
            currentScene.AddComponent<UnitComponent>();
            // 可以订阅这个事件中创建Loading界面
            await EventSystem.Instance.PublishAsync(root, new SceneChangeStart());
            await ETTask.CompletedTask;
        }
    }
}