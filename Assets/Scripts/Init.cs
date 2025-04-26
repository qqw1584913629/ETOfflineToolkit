using System.Reflection;
using UnityEngine;
using YooAsset;

namespace MH
{
    public class Init : Singleton<Init>
    {
        [Header("YooAsset相关")] [SerializeField]
        private EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        public Scene Root { get; set; }

        /// <summary>
        /// 游戏入口Awake脚本
        /// </summary>
        protected override async void Awake()
        {
            base.Awake();
            //初始化yoo
            await World.Instance.AddSingleton<ResourceSingleton, EPlayMode>(PlayMode).InitializeYooAsset();
            //初始化Luban配表
            World.Instance.AddSingleton<CodeTypes, Assembly[]>(new[] { typeof(Init).Assembly });
            World.Instance.AddSingleton<RedDotSingleton>();
            World.Instance.AddSingleton<ConfigsSingleton>();
            World.Instance.AddSingleton<NumericWatcherComponent>();
            World.Instance.AddSingleton<EventSystem>();
            World.Instance.AddSingleton<EntitySystemSingleton>();
            World.Instance.AddSingleton<TimeInfo>();
            World.Instance.AddSingleton<DatasSingleton>();
            World.Instance.AddSingleton<BuffActionDispatcher>();
            World.Instance.AddSingleton<LocalizationSingleton>();
            World.Instance.AddSingleton<ObjectPool>();

            EventSystem.Instance.PublishAsync(Root, new Main_Init()).Coroutine();
        }
        private void OnApplicationQuit()
        {
            DatasSingleton.Instance?.SaveAllDataByPlayerPrefs();
        }

        private void OnApplicationPause(bool pauseStatus)
        {
            DatasSingleton.Instance?.SaveAllDataByPlayerPrefs();
        }

        private void Update()
        {
            EntitySystemSingleton.Instance?.Update();
        }
        
        private void LateUpdate()
        {
            EntitySystemSingleton.Instance?.LateUpdate();
        }
    }
}