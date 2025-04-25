using System.Collections.Generic;
using UnityEngine;

namespace MH
{
    public static class EUIModelViewHelper
    {
        /// <summary>
        /// 绑定UI元素到指定Transform
        /// </summary>
        /// <typeparam name="T">UI元素类型</typeparam>
        /// <param name="self">UI元素实例</param>
        /// <param name="transform">目标Transform</param>
        /// <returns>绑定后的UI元素实例</returns>
        public static T BindTrans<T>(this T self, Transform transform) where T : Entity, IAwake, IUIScrollItem<T>
        {
            T value = self;
            return value.BindTrans(transform);
        }

        /// <summary>
        /// 添加UI滚动项到指定字典
        /// </summary>
        /// <typeparam name="K">UI逻辑类型</typeparam>
        /// <typeparam name="T">UI滚动项类型</typeparam>
        /// <param name="self">UI逻辑实例</param>

        public static void AddUIScrollItems<K, T>(this K self, ref Dictionary<int, T> dictionary, int count) where K : Entity, IUILogic where T : Entity, IAwake, IUIScrollItem<T>
        {
            if (dictionary == null)
            {
                dictionary = new Dictionary<int, T>();
            }

            if (count <= 0)
            {
                return;
            }

            foreach (var item in dictionary)
            {
                T value = item.Value;
                value?.Dispose();
            }
            dictionary.Clear();
            for (int i = 0; i <= count; i++)
            {
                T itemServer = self.AddChild<T>();
                dictionary.Add(i, itemServer);
            }
        }
    }
}