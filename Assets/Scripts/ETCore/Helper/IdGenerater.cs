using System;
using System.Runtime.InteropServices;
using UnityEditor;

namespace MH
{
    [StructLayout(LayoutKind.Sequential, Pack = 1)]

    public struct IdStruct
    {
        public uint Time; // 30bit
        public uint Value; // 20bit

        public long ToLong()
        {
            ulong result = 0;
            result <<= 30;
            result |= this.Time;
            result <<= 20;
            result |= this.Value;
            return (long)result;
        }

        public IdStruct(uint time, uint value)
        {
            this.Time = time;
            this.Value = value;
        }

        public IdStruct(long id)
        {
            ulong result = (ulong)id;
            this.Value = (uint)(result & IdGenerater.Mask20bit);
            result >>= 20;
            this.Time = (uint)result & IdGenerater.Mask30bit;
            result >>= 30;
        }

        public override string ToString()
        {
            return $"time: {this.Time}, value: {this.Value}";
        }
    }

    public static class IdGenerater
    {
        public const int Mask30bit = 0x3fffffff;
        public const int Mask20bit = 0xfffff;
        private static long epoch2022;
        private static int value;
        private static int instanceIdValue;

        public static uint TimeSince2022()
        {
            long epoch1970Tick = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000;
            epoch2022 = new DateTime(2022, 1, 1, 0, 0, 0, DateTimeKind.Utc).Ticks / 10000 - epoch1970Tick;
            uint a = (uint)(((DateTime.UtcNow.Ticks / 10000 - epoch1970Tick) - epoch2022) / 1000);
            return a;
        }

        public static long GenerateId()
        {
            uint time = TimeSince2022();
            int v = 0;
            if (++value > Mask20bit - 1)
                value = 0;
            v = value;
            IdStruct idStruct = new(time, (uint)v);
            return idStruct.ToLong();
        }
    }
}