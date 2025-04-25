using System;

namespace MH
{
    public interface IBuffAction
    {
        /// <summary>
        /// 生命周期开始
        /// </summary>
        /// <param name="buffInfo">所属的BuffInfo</param>
        void OnStart(BuffInfo buffInfo);

        /// <summary>
        /// 轮询
        /// </summary>
        /// <param name="buffInfo">所属的BuffInfo</param>
        void Run(BuffInfo buffInfo);
        
        /// <summary>
        /// 生命周期结束
        /// </summary>
        /// <param name="buffInfo">所属的BuffInfo</param>
        void OnEnd(BuffInfo buffInfo);
        /// <summary>
        /// 层级变化
        /// </summary>
        /// <param name="buffInfo">所属的BuffInfo</param>
        /// <param name="oldLayer">旧层级</param>
        /// <param name="newLayer">新层级</param>
        void OnLevelChange(BuffInfo buffInfo, int oldLayer, int newLayer);
    }
}
