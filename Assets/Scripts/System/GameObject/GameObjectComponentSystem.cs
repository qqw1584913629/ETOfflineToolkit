using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class GameObjectComponentAwakeSystem: AwakeSystem<GameObjectComponent>
    {
        protected override void Awake(GameObjectComponent self)
        {
            
        }
    }
    [EntitySystem]
    public class GameObjectComponentDestroySystem: DestroySystem<GameObjectComponent>
    {
        protected override void Destroy(GameObjectComponent self)
        {
            if (!GameObjectPoolHelper.IsContainPool(self.GameObject.name))
            {
                GameObject.Destroy(self.GameObject);
                return;
            }
            GameObjectPoolHelper.ReturnObjectToPool(self.GameObject);
            self.GameObject = null;
        }
    }

    public static class GameObjectComponentSystem
    {
        
    }
}