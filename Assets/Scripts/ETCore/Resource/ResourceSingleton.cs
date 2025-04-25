using System.Collections.Generic;
using UnityEngine;
using YooAsset;

namespace MH
{
    public class ResourceSingleton : LogicSingleton<ResourceSingleton>, ISingletonAwake<EPlayMode>
    {
        private EPlayMode PlayMode = EPlayMode.EditorSimulateMode;
        public void Awake(EPlayMode a)
        {
            PlayMode = a;
        }

        /// <summary>
        /// 初始化资源系统
        /// </summary>
        public async ETTask InitializeYooAsset()
        {
            // 初始化资源系统
            YooAssets.Initialize();

            // 创建默认的资源包
            var package = YooAssets.CreatePackage("DefaultPackage");

            // 设置该资源包为默认的资源包，可以使用YooAssets相关加载接口加载该资源包内容。
            YooAssets.SetDefaultPackage(package);

            if (PlayMode == EPlayMode.EditorSimulateMode)
            {
                var initParameters = new EditorSimulateModeParameters();
                initParameters.SimulateManifestFilePath = EditorSimulateModeHelper.SimulateBuild("DefaultPackage");
                await package.InitializeAsync(initParameters).Task;
            }
            else if (PlayMode == EPlayMode.OfflinePlayMode)
            {
                var initParameters = new OfflinePlayModeParameters();
                await package.InitializeAsync(initParameters).Task;
            }
            else if (PlayMode == EPlayMode.HostPlayMode)
            {
                // 注意：GameQueryServices.cs 太空战机的脚本类，详细见StreamingAssetsHelper.cs
                string defaultHostServer = "http://127.0.0.1/CDN/Android/v1.0";
                string fallbackHostServer = "http://127.0.0.1/CDN/Android/v1.0";
                var initParameters = new HostPlayModeParameters();
                // todo 如果要使用联机模式则需要配置下面三个东西
                // initParameters.BuildinQueryServices = new GameQueryServices(); 
                // initParameters.DecryptionServices = new FileOffsetDecryption();
                // initParameters.RemoteServices = new RemoteServices(defaultHostServer, fallbackHostServer);
                var initOperation = package.InitializeAsync(initParameters);
                await initOperation.Task;

                if (initOperation.Status == EOperationStatus.Succeed)
                {
                    Debug.Log("资源包初始化成功！");
                }
                else
                {
                    Debug.LogError($"资源包初始化失败：{initOperation.Error}");
                }
            }


            var defaultPackage = YooAssets.GetPackage("DefaultPackage");
            var operation = defaultPackage.UpdatePackageVersionAsync();
            await operation.Task;

            if (operation.Status == EOperationStatus.Succeed)
            {
                //更新成功
                string packageVersion = operation.PackageVersion;
                Debug.Log($"Updated package Version : {packageVersion}");
            }
            else
            {
                //更新失败
                Debug.LogError($"更新失败{operation.Error}");
            }

            // 更新成功后自动保存版本号，作为下次初始化的版本。
            // 也可以通过operation.SavePackageVersion()方法保存。
            bool savePackageVersion = true;
            var operation1 = defaultPackage.UpdatePackageManifestAsync(operation.PackageVersion, savePackageVersion);
            await operation1.Task;

            if (operation1.Status == EOperationStatus.Succeed)
            {
                //更新成功
            }
            else
            {
                Debug.LogError($"更新失败{operation1.Error}");
            }

            await Download();
        }
        /// <summary>
        /// 下载资源
        /// </summary>
        async ETTask Download()
        {
            int downloadingMaxNum = 10;
            int failedTryAgain = 3;
            var package = YooAssets.GetPackage("DefaultPackage");
            var downloader = package.CreateResourceDownloader(downloadingMaxNum, failedTryAgain);

            //没有需要下载的资源
            if (downloader.TotalDownloadCount == 0)
            {
                return;
            }

            //需要下载的文件总数和总大小
            int totalDownloadCount = downloader.TotalDownloadCount;
            long totalDownloadBytes = downloader.TotalDownloadBytes;

            //注册回调方法
            downloader.OnDownloadErrorCallback = OnDownloadErrorFunction;
            downloader.OnDownloadProgressCallback = OnDownloadProgressUpdateFunction;
            downloader.OnDownloadOverCallback = OnDownloadOverFunction;
            downloader.OnStartDownloadFileCallback = OnStartDownloadFileFunction;

            //开启下载
            downloader.BeginDownload();
            await downloader.Task;

            //检测下载结果
            if (downloader.Status == EOperationStatus.Succeed)
            {
                //下载成功
            }
            else
            {
                //下载失败
            }
        }
        /// <summary>
        /// 开始下载文件
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="sizebytes">文件大小</param>
        private void OnStartDownloadFileFunction(string filename, long sizebytes)
        {
        }
        /// <summary>
        /// 下载完成
        /// </summary>
        /// <param name="issucceed">是否成功</param>
        private void OnDownloadOverFunction(bool issucceed)
        {
        }
        /// <summary>
        /// 下载进度更新
        /// </summary>
        /// <param name="totaldownloadcount">总下载数量</param>
        /// <param name="currentdownloadcount">当前下载数量</param>
        /// <param name="totaldownloadbytes">总下载大小</param>
        /// <param name="currentdownloadbytes">当前下载大小</param> 
        private void OnDownloadProgressUpdateFunction(int totaldownloadcount, int currentdownloadcount,
            long totaldownloadbytes, long currentdownloadbytes)
        {
        }
        /// <summary>
        /// 下载错误
        /// </summary>
        /// <param name="filename">文件名</param>
        /// <param name="error">错误信息</param>
        private void OnDownloadErrorFunction(string filename, string error)
        {
        }
        /// <summary>
        /// 销毁资源包
        /// </summary>
        /// <param name="packageName">资源包名称</param>
        public void DestroyPackage(string packageName)
        {
            ResourcePackage package = YooAssets.GetPackage(packageName);
            package.UnloadUnusedAssets();
        }
        /// <summary>
        /// 同步加载资源
        /// </summary>
        /// <typeparam name="T">资源类型</typeparam>
        /// <param name="location">资源路径</param>
        /// <returns>资源</returns>
        public T LoadAssetSync<T>(string location) where T : UnityEngine.Object
        {
            AssetOperationHandle handle = YooAssets.LoadAssetSync<T>(location);
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        /// <summary>
        /// 同步加载原生文件
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        public string LoadRawFileSync(string fileName)
        {
            RawFileOperationHandle handle = YooAssets.LoadRawFileSync(fileName);
            string rawFileText = handle.GetRawFileText();
            handle.Release();
            return rawFileText;
        }
        
        /// <summary>
        /// 主要用来加载dll config aotdll，因为这时候纤程还没创建，无法使用ResourcesLoaderComponent。
        /// 游戏中的资源应该使用ResourcesLoaderComponent来加载
        /// </summary>
        public async ETTask<T> LoadAssetAsync<T>(string location) where T : UnityEngine.Object
        {
            AssetOperationHandle handle = YooAssets.LoadAssetAsync<T>(location);
            await handle.Task;
            T t = (T)handle.AssetObject;
            handle.Release();
            return t;
        }

        /// <summary>
        /// 主要用来加载dll config aotdll，因为这时候纤程还没创建，无法使用ResourcesLoaderComponent。
        /// 游戏中的资源应该使用ResourcesLoaderComponent来加载
        /// </summary>
        public async ETTask<Dictionary<string, T>> LoadAllAssetsAsync<T>(string location) where T : UnityEngine.Object
        {
            AllAssetsOperationHandle allAssetsOperationHandle = YooAssets.LoadAllAssetsAsync<T>(location);
            await allAssetsOperationHandle.Task;
            Dictionary<string, T> dictionary = new Dictionary<string, T>();
            foreach (UnityEngine.Object assetObj in allAssetsOperationHandle.AllAssetObjects)
            {
                T t = assetObj as T;
                dictionary.Add(t.name, t);
            }

            allAssetsOperationHandle.Release();
            return dictionary;
        }
    }
}