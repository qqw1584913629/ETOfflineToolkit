using System;
using System.Collections.Generic;
using System.Reflection;

namespace MH
{
    /// <summary>
    /// 代码类型管理类
    /// 负责管理和缓存程序集中的类型信息
    /// 实现了单例模式和程序集唤醒接口
    /// </summary>
    public class CodeTypes : LogicSingleton<CodeTypes>, ISingletonAwake<Assembly[]>
    {
        /// <summary>
        /// 存储所有类型的字典
        /// Key: 类型的完整名称
        /// Value: 类型对象
        /// </summary>
        private readonly Dictionary<string, Type> allTypes = new();

        /// <summary>
        /// 存储带有特定特性的类型集合
        /// Key: 特性类型
        /// Value: 带有该特性的类型集合
        /// </summary>
        private readonly UnOrderMultiMapSet<Type, Type> types = new();

        /// <summary>
        /// 初始化代码类型管理器
        /// 扫描并缓存程序集中的类型信息
        /// </summary>
        /// <param name="assemblies">要扫描的程序集数组</param>
        public void Awake(Assembly[] assemblies)
        {
            Dictionary<string, Type> addTypes = AssemblyHelper.GetAssemblyTypes(assemblies);
            foreach ((string fullName, Type type) in addTypes)
            {
                this.allTypes[fullName] = type;

                if (type.IsAbstract)
                {
                    continue;
                }

                // 记录所有的有BaseAttribute标记的的类型
                object[] objects = type.GetCustomAttributes(typeof(BaseAttribute), true);

                foreach (object o in objects)
                {
                    this.types.Add(o.GetType(), type);
                }
            }
        }

        /// <summary>
        /// 获取所有已缓存的类型
        /// </summary>
        /// <returns>包含所有类型的字典</returns>
        public Dictionary<string, Type> GetTypes()
        {
            return allTypes;
        }

        /// <summary>
        /// 获取带有指定特性的所有类型
        /// </summary>
        /// <param name="systemAttributeType">要查找的特性类型</param>
        /// <returns>带有指定特性的类型集合</returns>
        public HashSet<Type> GetTypes(Type systemAttributeType)
        {
            if (!this.types.ContainsKey(systemAttributeType))
            {
                return new HashSet<Type>();
            }

            return this.types[systemAttributeType];
        }
    }
}