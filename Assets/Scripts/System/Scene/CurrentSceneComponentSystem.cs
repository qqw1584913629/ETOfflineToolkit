namespace MH
{
    [EntitySystem]
    public class CurrentSceneComponentAwakeSystem : AwakeSystem<CurrentSceneComponent>
    {
        protected override void Awake(CurrentSceneComponent self)
        {
            
        }
    }

    public static class CurrentSceneComponentSystem
    {
        public static Scene CurrentScene(this Scene root)
        {
            return root.GetComponent<CurrentSceneComponent>()?.Scene;
        }
    }
}