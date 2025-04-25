using System;
using UnityEngine;

namespace MH
{
    /// <summary>
    /// 单例基类
    /// </summary>
    public abstract class ASingleton : IDisposable
    {
        /// <summary>
        /// 注册
        /// </summary>
        internal abstract void Register();
        public virtual void Dispose()
        {
            
        }
    }
    /// <summary>
    /// 逻辑单例
    /// </summary>
    /// <typeparam name="T">单例类型</typeparam>
    public class LogicSingleton<T> : ASingleton where T : LogicSingleton<T>
    {
        private bool isDisposed;

        private static T instance;
        public static T Instance
        {
            get
            {
                return instance;
            }
            private set
            {
                instance = value;
            }
        }

        internal override void Register()
        {
            Instance = (T)this;
        }

        public bool IsDisposed()
        {
            return this.isDisposed;
        }

        protected virtual void Destroy()
        {

        }

        public override void Dispose()
        {
            if (this.isDisposed)
            {
                return;
            }

            this.isDisposed = true;

            this.Destroy();

            Instance = null;
        }
    }
}