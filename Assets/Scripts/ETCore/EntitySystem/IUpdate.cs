using System;

namespace MH
{
    public interface IUpdate
    {
    }
    public interface IUpdateSystem: ISystemType
    {
        void Run(Entity o);
    }
    [EntitySystem]
    public abstract class UpdateSystem<T> : SystemObject, IUpdateSystem where T : Entity, IUpdate
    {
        Type ISystemType.Type()
        {
            return typeof(T);
        }
        Type ISystemType.SystemType()
        {
            return typeof(IUpdateSystem);
        }

        void IUpdateSystem.Run(Entity o)
        {
            this.Update((T)o);
        }
        protected abstract void Update(T self);
    }
}