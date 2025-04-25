using cfg;
using SimpleJSON;
using YooAsset;

namespace MH
{
    /// <summary>
    /// 配置管理单例类
    /// 负责加载和管理游戏的所有配置数据
    /// 实现了单例模式和唤醒接口
    /// </summary>
    public class ConfigsSingleton : LogicSingleton<ConfigsSingleton>, ISingletonAwake
    {
        /// <summary>
        /// 游戏配置表集合
        /// 包含所有已加载的配置数据
        /// </summary>
        public Tables Tables;

        /// <summary>
        /// 初始化配置管理器
        /// 创建配置表实例并设置加载器
        /// </summary>
        public void Awake()
        {
            Tables = new Tables(Loader);
        }

        /// <summary>
        /// 配置文件加载器
        /// 从资源系统加载JSON格式的配置文件
        /// </summary>
        /// <param name="fileName">要加载的配置文件名</param>
        /// <returns>解析后的JSON节点</returns>
        private JSONNode Loader(string fileName)
        {
            var content = ResourceSingleton.Instance.LoadRawFileSync(fileName);
            return JSON.Parse(content);
        }
    }
}