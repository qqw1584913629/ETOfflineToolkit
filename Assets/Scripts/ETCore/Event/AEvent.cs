using System;

namespace MH
{
    public interface IEvent
    {
        Type Type { get; }
    }
    
    public abstract class AEvent<S, A>: IEvent where A : struct where S : Scene
    {
        public Type Type
        {
            get
            {
                return typeof (A);
            }
        }
        
        protected abstract ETTask Run(S scene, A a);

        public async ETTask Handle(S scene, A a)
        {
            await Run(scene, a);
        }
    }
}