using System;
using System.Collections.Generic;

namespace MH
{
    public class MultiMap<T, K> : SortedDictionary<T, List<K>>
    {
        private readonly List<K> Empty = new();
        private readonly int maxPoolCount;
        private readonly Queue<List<K>> pool;

        /// <summary>
        /// 初始化MultiMap
        /// </summary>
        /// <param name="maxPoolCount">最大池大小</param>
        public MultiMap(int maxPoolCount = 0)
        {
            this.maxPoolCount = maxPoolCount;
            this.pool = new Queue<List<K>>(maxPoolCount);
        }

        /// <summary>
        /// 获取一个列表
        /// </summary>
        /// <returns>列表</returns>
        private List<K> FetchList()
        {
            if (this.pool.Count > 0)
            {
                return this.pool.Dequeue();
            }
            return new List<K>(10);
        }

        /// <summary>
        /// 回收一个列表
        /// </summary>
        /// <param name="list">列表</param>
        private void Recycle(List<K> list)
        {
            if (list == null)
            {
                return;
            }
            if (this.pool.Count == this.maxPoolCount)
            {
                return;
            }
            list.Clear();
            this.pool.Enqueue(list);
        }

        /// <summary>
        /// 添加一个键值对到MultiMap中
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        public void Add(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                list = this.FetchList();
                this.Add(t, list);
            }
            list.Add(k);
        }

        /// <summary>
        /// 移除一个键值对
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        /// <returns>如果成功移除，返回true，否则返回false</returns>
        public bool Remove(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            if (!list.Remove(k))
            {
                return false;
            }
            if (list.Count == 0)
            {
                this.Remove(t);
            }
            return true;
        }

        /// <summary>
        /// 移除一个键
        /// </summary>
        /// <param name="t">键</param>
        /// <returns>如果成功移除，返回true，否则返回false</returns>    
        public new bool Remove(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            this.Recycle(list);
            return base.Remove(t);
        }

        /// <summary>
        /// 不返回内部的list,copy一份出来
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public K[] GetAll(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return Array.Empty<K>();
            }
            return list.ToArray();
        }

        /// <summary>
        /// 返回内部的list
        /// </summary>
        /// <param name="t"></param>
        /// <returns></returns>
        public new List<K> this[T t]
        {
            get
            {
                this.TryGetValue(t, out List<K> list);
                return list ?? Empty;
            }
        }
        /// <summary>
        /// 获取一个键的第一个值
        /// </summary>
        /// <param name="t">键</param>
        /// <returns>值</returns>
        public K GetOne(T t)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list != null && list.Count > 0)
            {
                return list[0];
            }
            return default;
        }
        /// <summary>
        /// 检查一个键是否包含一个值
        /// </summary>
        /// <param name="t">键</param>
        /// <param name="k">值</param>
        /// <returns>如果包含，返回true，否则返回false</returns>
        public bool Contains(T t, K k)
        {
            List<K> list;
            this.TryGetValue(t, out list);
            if (list == null)
            {
                return false;
            }
            return list.Contains(k);
        }
    }
}