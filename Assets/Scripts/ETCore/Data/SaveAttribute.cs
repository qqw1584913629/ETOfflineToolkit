using System;

namespace MH
{
    /// <summary>
    /// 数据存储特性标记
    /// 用于标记需要进行序列化存储的类
    /// </summary>
    [AttributeUsage(AttributeTargets.Class)]
    public class SaveAttribute : Attribute
    {
        
    }
}