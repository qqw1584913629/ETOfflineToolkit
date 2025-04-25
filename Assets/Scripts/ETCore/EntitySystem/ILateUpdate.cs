using System;

namespace MH
{
    public interface ILateUpdate
    {
    }
    public interface ILateUpdateSystem : ISystemType
    {
        void Run(Entity o);
    }
    [EntitySystem]
    public abstract class LateUpdateSystem<T> : SystemObject, ILateUpdateSystem where T : Entity, ILateUpdate
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }
        Type ISystemType.SystemType()
        {
            return typeof(ILateUpdateSystem);
        }

        void ILateUpdateSystem.Run(Entity o)
        {
            this.LateUpdate((T)o);
        }
        protected abstract void LateUpdate(T self);
    }
}