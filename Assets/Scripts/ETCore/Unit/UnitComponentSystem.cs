using System.Collections.Generic;

namespace MH
{
    [EntitySystem]
    public class UnitComponentAwakeSystem: AwakeSystem<UnitComponent>
    {
        protected override void Awake(UnitComponent self)
        {
            
        }
    }

    public static class UnitComponentSystem
    {
        public static Unit Get(this UnitComponent self, long id)
        {
            Unit unit = self.GetChild<Unit>(id);
            return unit;
        }

        public static List<Unit> GetAll(this UnitComponent self)
        {
            List<Unit> res = new List<Unit>();
            foreach (var value in self.Childrens.Values)
                res.Add(value as Unit);
            return res;
        }

        public static void Remove(this UnitComponent self, long id)
        {
            Unit unit = self.GetChild<Unit>(id);
            unit?.Dispose();
        }
    }
}