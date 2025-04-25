using System.Collections.Generic;

namespace MH
{
    public class MultiDictionary<T, N> : Dictionary<T, List<N>>
    {
        /// <summary>
        /// 尝试获取指定键的列表
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">输出列表</param>
        /// <returns>如果键存在，返回true，否则返回false</returns>
        public bool TryGetList(T t, out List<N> k)
        {
            return this.TryGetValue(t, out k);
        }

        /// <summary>
        /// 添加一个键值对到字典中
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="n">值</param>
        public void Add(T t, N n)
        {
            List<N> kSet;
            this.TryGetValue(t, out kSet);
            if (kSet == null)
            {
                kSet = new List<N>();
                this[t] = kSet;
            }

            kSet.Add(n);
        }

        /// <summary>
        /// 从字典中移除指定键和值
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="n">值</param>
        /// <returns>如果成功移除，返回true，否则返回false</returns>
        public bool Remove(T t, N n)
        {
            this.TryGetValue(t, out List<N> dic);
            if (dic == null || !dic.Remove(n))
            {
                return false;
            }

            if (dic.Count == 0)
            {
                this.Remove(t);
            }

            return true;
        }
    }
}