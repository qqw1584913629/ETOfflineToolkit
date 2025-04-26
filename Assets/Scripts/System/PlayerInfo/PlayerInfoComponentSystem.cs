using System;

namespace MH
{
    [EntitySystem]
    public class PlayerInfoComponentAwakeSystem : AwakeSystem<PlayerInfoComponent>
    {
        protected override void Awake(PlayerInfoComponent self)
        {
            self.UnitId = 1;
        }
    }
    [EntitySystem]
    public class PlayerInfoComponentDestroySystem : DestroySystem<PlayerInfoComponent>
    {
        protected override void Destroy(PlayerInfoComponent self)
        {
        }
    }

    public static class PlayerInfoComponentSystem
    {
    }
}
