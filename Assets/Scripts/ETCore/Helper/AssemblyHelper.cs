using System;
using System.Collections.Generic;
using System.Reflection;

namespace MH
{
    public static class AssemblyHelper
    {
        /// <summary>
        /// 获取指定程序集中的所有类型
        /// </summary>
        /// <param name="args">程序集数组</param>
        /// <returns>类型字典，键为类型全名，值为类型</returns>
        public static Dictionary<string, Type> GetAssemblyTypes(params Assembly[] args)
        {
            Dictionary<string, Type> types = new Dictionary<string, Type>();

            foreach (Assembly ass in args)
            {
                foreach (Type type in ass.GetTypes())
                {
                    types[type.FullName] = type;
                }
            }

            return types;
        }
    }
}