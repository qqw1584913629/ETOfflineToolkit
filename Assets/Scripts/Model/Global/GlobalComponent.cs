using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class GlobalComponentAwakeSystem : AwakeSystem<GlobalComponent>
    {
        protected override void Awake(GlobalComponent self)
        {
            self.NormalRoot = GameObject.Find("/Global/Root/Normal").transform;
            self.PopUpRoot = GameObject.Find("/Global/Root/PopUp").transform;
            self.FixedRoot = GameObject.Find("/Global/Root/Fixed").transform;
            self.OtherRoot = GameObject.Find("/Global/Root/Other").transform;
            self.UICanvas = GameObject.Find("/Global/Root").GetComponent<Canvas>();
            self.PoolRoot = GameObject.Find("/Global/PoolRoot").transform;
            self.UnitRoot = GameObject.Find("/Global/Unit").transform;
        }
    }

    public static class GlobalComponentSystem
    {
        
    }
    public class GlobalComponent : Entity, IAwake
    {
        public Transform NormalRoot { get; set; }
        public Transform PopUpRoot { get; set; }
        public Transform FixedRoot { get; set; }
        public Transform PoolRoot { get; set; }
        public Transform OtherRoot { get; set; }
        public Transform UnitRoot { get; set; }
        public Canvas UICanvas { get; set; }
    }
}