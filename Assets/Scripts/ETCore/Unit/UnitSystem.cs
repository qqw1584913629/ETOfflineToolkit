using System;
using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class UnitAwakeSystem: AwakeSystem<Unit, int>
    {
        protected override void Awake(Unit self, int a)
        {
            self.ConfigId = a;
        }
    }
    public static class UnitSystem
    {
        public static bool IsAlive(this Unit self)
        {
            if (self == null || self.IsDisposed)
                return false;
            NumericComponent numericComponent = self.GetComponent<NumericComponent>();
            if (numericComponent == null)
                return false;
            return numericComponent.GetAsInt(NumericType.IsAlive) == 1;
        }
        public static void SetAlive(this Unit self, bool state)
        {
            if (self == null || self.IsDisposed)
                return;
            NumericComponent numericComponent = self.GetComponent<NumericComponent>();
            if (numericComponent == null)
                return;
            numericComponent.Set(NumericType.IsAlive, state ? 1 : 0);
        }
    }
}