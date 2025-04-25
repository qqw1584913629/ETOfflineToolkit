using System;
using System.Collections.Generic;

namespace MH
{
    public class BuffActionDispatcher : LogicSingleton<BuffActionDispatcher>, ISingletonAwake
    {
        private Dictionary<BuffInfoType, IBuffAction> buffActions = new Dictionary<BuffInfoType, IBuffAction>();

        public void Awake()
        {
            HashSet<Type> types = CodeTypes.Instance.GetTypes(typeof(BuffActionAttribute));
            foreach (Type type in types)
            {
                object[] attrs = type.GetCustomAttributes(typeof(BuffActionAttribute), false);
                foreach (object attr in attrs)
                {
                    BuffActionAttribute buffActionAttribute = (BuffActionAttribute)attr;
                    IBuffAction obj = (IBuffAction)Activator.CreateInstance(type);
                    if (this.buffActions.ContainsKey(buffActionAttribute.buffInfoType))
                        throw new Exception($"BuffActionDispatcher中已经存在BuffInfoType为{buffActionAttribute.buffInfoType}的BuffAction");
                    this.buffActions[buffActionAttribute.buffInfoType] = obj;
                }
            }
        }
        public void Run(BuffInfo buffInfo)
        {
            if (this.buffActions.TryGetValue((BuffInfoType)buffInfo.Config.Id, out IBuffAction buffAction))
            {
                buffAction.Run(buffInfo);
            }
        }
        public void OnStart(BuffInfo buffInfo)
        {
            if (this.buffActions.TryGetValue((BuffInfoType)buffInfo.Config.Id, out IBuffAction buffAction))
            {
                buffAction.OnStart(buffInfo);
            }
        }
        public void OnEnd(BuffInfo buffInfo)
        {
            if (this.buffActions.TryGetValue((BuffInfoType)buffInfo.Config.Id, out IBuffAction buffAction))
            {
                buffAction.OnEnd(buffInfo);
            }
        }
        public void OnLevelChange(BuffInfo buffInfo, int oldLayer, int newLayer)
        {
            if (this.buffActions.TryGetValue((BuffInfoType)buffInfo.Config.Id, out IBuffAction buffAction))
            {
                buffAction.OnLevelChange(buffInfo, oldLayer, newLayer);
            }
        }
    }
}
