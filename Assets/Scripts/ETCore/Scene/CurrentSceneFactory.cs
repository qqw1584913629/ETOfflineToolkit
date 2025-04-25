namespace MH
{
    public static class CurrentSceneFactory
    {
        /// <summary>
        /// 创建当前场景
        /// </summary>
        /// <param name="id">场景ID</param>
        /// <param name="name">场景名称</param>
        /// <param name="currentSceneComponent">当前场景组件</param>
        public static Scene Create(long id, string name, CurrentSceneComponent currentSceneComponent)
        {
            Scene currentScene = EntitySceneFactory.CreateScene(currentSceneComponent, id, SceneType.Current, name);
            currentSceneComponent.Scene = currentScene;

            EventSystem.Instance.Publish(currentScene, new AfterCreateCurrentScene());
            return currentScene;
        }
    }
}