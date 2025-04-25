using System;
using System.Collections.Generic;

namespace MH
{
    public static class RandomGenerator
    {
        private static Random random;

        private static Random GetRandom()
        {
            return random ??= new Random(Guid.NewGuid().GetHashCode() ^ Environment.TickCount);
        }

        public static ulong RandUInt64()
        {
            int r1 = RandInt32();
            int r2 = RandInt32();
            return ((ulong)r1 << 32) | (uint)r2;
        }

        public static int RandInt32()
        {
            return GetRandom().Next();
        }

        public static uint RandUInt32()
        {
            return (uint)GetRandom().Next();
        }

        public static long RandInt64()
        {
            uint r1 = RandUInt32();
            uint r2 = RandUInt32();
            return (long)(((ulong)r1 << 32) | r2);
        }

        /// <summary>
        /// 获取lower与Upper之间的随机数,包含下限，不包含上限
        /// </summary>
        /// <param name="lower"></param>
        /// <param name="upper"></param>
        /// <returns></returns>
        public static int RandomNumber(int lower, int upper)
        {
            int value = GetRandom().Next(lower, upper);
            return value;
        }
        /// <summary>
        /// 获取一个随机布尔值
        /// </summary>
        /// <returns>随机布尔值</returns>
        public static bool RandomBool()
        {
            return GetRandom().Next(2) == 0;
        }
        /// <summary>
        /// 获取一个随机数组元素
        /// </summary>
        /// <typeparam name="T">数组类型</typeparam>
        /// <param name="array">数组</param>
        /// <returns>随机数组元素</returns> 
        public static T RandomArray<T>(T[] array)
        {
            return array[RandomNumber(0, array.Length)];
        }
        /// <summary>
        /// 获取一个随机列表元素
        /// </summary>
        /// <typeparam name="T">列表类型</typeparam>
        /// <param name="array">列表</param>
        /// <returns>随机列表元素</returns>
        public static T RandomArray<T>(List<T> array)
        {
            return array[RandomNumber(0, array.Count)];
        }

        /// <summary>
        /// 打乱数组
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="arr">要打乱的数组</param>
        public static void BreakRank<T>(List<T> arr)
        {
            if (arr == null || arr.Count < 2)
            {
                return;
            }

            for (int i = 0; i < arr.Count; i++)
            {
                int index = GetRandom().Next(0, arr.Count);
                (arr[index], arr[i]) = (arr[i], arr[index]);
            }
        }
        /// <summary>
        /// 获取一个0到1之间的随机浮点数
        /// </summary>
        /// <returns>随机浮点数</returns>
        public static float RandFloat01()
        {
            int a = RandomNumber(0, 1000000);
            return a / 1000000f;
        }
        /// <summary>
        /// 获取一个指定范围内的随机浮点数
        /// </summary>
        /// <param name="lower">下限</param>
        /// <param name="upper">上限</param>
        /// <returns>随机浮点数</returns>
        public static float RandFloat(float lower, float upper)
        {
            int a = RandomNumber((int)(lower * 1000000), (int)(upper * 1000000));
            return a / 1000000f;
        }
        /// <summary>
        /// 获取一个-1到1之间的随机浮点数
        /// </summary>
        /// <returns>随机浮点数</returns>
        public static float RandFloat11()
        {
            int a = RandomNumber(-1000000, 1000000);
            return a / 1000000f;
        }
    }
}