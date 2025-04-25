using System;
namespace MH
{
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method)]
    public class EventAttribute : BaseAttribute 
    {
        public SceneType SceneType { get; }

        public EventAttribute(SceneType sceneType)
        {
            this.SceneType = sceneType;
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class BaseAttribute : Attribute
    {

    }
}