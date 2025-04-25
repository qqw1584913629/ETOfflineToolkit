namespace MH
{
    public static class EntitySceneFactory
    {
        /// <summary>
        /// 创建场景
        /// </summary>
        /// <param name="parent">父实体</param>
        /// <param name="id">场景ID</param>
        /// <param name="sceneType">场景类型</param>
        public static Scene CreateScene(Entity parent, long id, SceneType sceneType, string name)
        {
            Scene scene = new Scene(id, sceneType, name);
            parent?.AddChild(scene);
            return scene;
        }
    }
}