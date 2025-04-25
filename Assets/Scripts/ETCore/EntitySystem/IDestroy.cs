using System;

namespace MH
{
    public interface IDestroy
    {
    }
    public interface IDestroySystem : ISystemType
    {
        void Run(Entity o);
    }
    [EntitySystem]
    public abstract class DestroySystem<T> : SystemObject, IDestroySystem where T : Entity, IDestroy
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }
        Type ISystemType.SystemType()
        {
            return typeof(IDestroySystem);
        }
        void IDestroySystem.Run(Entity o)
        {
            this.Destroy((T)o);
        }
        protected abstract void Destroy(T self);
    }
}