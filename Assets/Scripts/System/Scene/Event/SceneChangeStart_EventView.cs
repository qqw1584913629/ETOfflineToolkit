using UnityEngine.SceneManagement;

namespace MH
{
    [Event(SceneType.Main)]
    public class SceneChangeStart_EventView : AEvent<Scene, SceneChangeStart>
    {
        protected override async ETTask Run(Scene scene, SceneChangeStart a)
        {
            Scene currentScene = scene.CurrentScene();
            ResourcesComponent resourcesComponent = currentScene.GetComponent<ResourcesComponent>();
            // 加载场景资源
            await resourcesComponent.LoadSceneAsync(currentScene.SceneName, LoadSceneMode.Single);
            EventSystem.Instance.Publish(currentScene, new SceneChangeFinish());
        }
    }
}