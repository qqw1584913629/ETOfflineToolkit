using System.Collections.Generic;

namespace MH
{
    public class UnOrderMultiMapSet<T, K> : Dictionary<T, HashSet<K>>
    {
        /// <summary>
        /// 获取或设置指定键的HashSet
        /// </summary>
        /// <param name="t">键</param>
        /// <returns>HashSet</returns>
        public new HashSet<K> this[T t]
        {
            get
            {
                HashSet<K> set;
                if (!this.TryGetValue(t, out set))
                {
                    set = new HashSet<K>();
                }
                return set;
            }
        }

        /// <summary>
        /// 获取字典
        /// </summary>
        /// <returns>字典</returns>
        public Dictionary<T, HashSet<K>> GetDictionary()
        {
            return this;
        }

        /// <summary>
        /// 添加一个键值对到字典中
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        public void Add(T t, K k)
        {
            HashSet<K> set;
            this.TryGetValue(t, out set);
            if (set == null)
            {
                set = new HashSet<K>();
                base[t] = set;
            }
            set.Add(k);
        }

        /// <summary>
        /// 移除一个键值对
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        /// <returns>如果成功移除，返回true，否则返回false</returns>
        public bool Remove(T t, K k)
        {
            HashSet<K> set;
            this.TryGetValue(t, out set);
            if (set == null)
            {
                return false;
            }
            if (!set.Remove(k))
            {
                return false;
            }
            if (set.Count == 0)
            {
                this.Remove(t);
            }
            return true;
        }

        /// <summary>
        /// 检查一个键是否包含一个值
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        /// <returns>如果包含，返回true，否则返回false</returns>
        public bool Contains(T t, K k)
        {
            HashSet<K> set;
            this.TryGetValue(t, out set);
            if (set == null)
            {
                return false;
            }
            return set.Contains(k);
        }

        /// <summary>
        /// 获取字典中所有键值对的计数
        /// </summary>
        /// <returns>计数</returns>
        public new int Count
        {
            get
            {
                int count = 0;
                foreach (KeyValuePair<T, HashSet<K>> kv in this)
                {
                    count += kv.Value.Count;
                }
                return count;
            }
        }
    }
}