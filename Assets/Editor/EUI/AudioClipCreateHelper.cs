using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using System.Text.RegularExpressions;
namespace SetAssetBundleTag
{
    public class AudioClipCreateHelper
    {
        [MenuItem("ET/SpawnClipCode")]
        public static void GenerateAudioClipCode() {
 
            // 定义需要打包资源的文件夹根目录
            string strNeedSetLabelRoot = Application.dataPath + "/Bundles/";

            DirectoryInfo[] directoryDIRArray = null;
            DirectoryInfo dirTempInfo = new DirectoryInfo(strNeedSetLabelRoot);
            directoryDIRArray = dirTempInfo.GetDirectories();
            Debug.Log($"文件夹：{directoryDIRArray.Length},添加Code成功");
            foreach (DirectoryInfo currentDir in directoryDIRArray)
            {
                if (!currentDir.Name.Equals("Clips"))
                {
                    Debug.Log($"文件夹：{currentDir.Name},过滤成功");
                    continue;
                }
                JudgeDirOrFileByRecursive(currentDir);
                Debug.Log($"文件夹：{currentDir.Name},添加Code成功");
            }
            AssetDatabase.Refresh();
        }
        private static void JudgeDirOrFileByRecursive(FileSystemInfo fileSystemInfo) {
            // 参数检查
            if (fileSystemInfo.Exists == false)
            {
                Debug.LogError("文件或者目录名称："+fileSystemInfo+" 不存在，请检查");
                return;
            }
 
            // 得到当前目录下一级的文件信息集合
            DirectoryInfo directoryInfoObj = fileSystemInfo as DirectoryInfo;           // 文件信息转为目录信息
            FileSystemInfo[] fileSystemInfoArray = directoryInfoObj.GetFileSystemInfos();
 
            foreach (FileSystemInfo fileInfo in fileSystemInfoArray)
            {
                FileInfo fileInfoObj = fileInfo as FileInfo;
 
                if (fileInfoObj != null)
                {
                    // 修改此文件的 AssetBundle 标签
                    SetFileABLabel(fileInfoObj);
                }
                // 目录类型
                else {
 
                    // 如果是目录，则递归调用
                    JudgeDirOrFileByRecursive(fileInfo);
                }
            }
        }
 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileInfoObj">文件（文件信息）</param>
        /// <param name="scenesName">场景名称</param>
        static void SetFileABLabel(FileInfo fileInfoObj) {
            if (fileInfoObj.Extension == ".prefab" || fileInfoObj.Extension == ".meta" || fileInfoObj.Extension == ".bytes" || fileInfoObj.Extension == ".txt")
            {
                return;
            }
            string tmpWinPath = fileInfoObj.FullName;
            string tmpUnityPath = tmpWinPath.Replace("\\","/");
            string strABName = string.Empty;
            var strs = tmpUnityPath.Split("/");
            strABName = strs[strs.Length-1];
            string tmp1 = strABName.Split(".")[0];//去掉后缀 .prefab  .wav  .unity
            WriteAudiosCode(tmp1);
        }

        public static void WriteAudiosCode(string name)
        {
            string strDlgName = name;
            string strFilePath = Application.dataPath + "/Scripts/Model/Helper/ClipID.cs" ;
            if(!File.Exists(strFilePath))
            {
                Debug.LogError("当前不存在ClipID.cs!!!");
                return;
            }
	    
            string originWindowIdContent = File.ReadAllText(strFilePath);
            if (originWindowIdContent.Contains(strDlgName))
            {
                return;
            }
            var replace = Regex.Replace(strDlgName, @"[^a-zA-Z0-9]", "_");  // 先把非字母替换成下划线
            replace = Regex.Replace(replace, @"_{2,}", "_");             // 把多个连续下划线替换成单个下划线
            int windowIdEndIndex   = GetAudioEndIndex(originWindowIdContent);
            originWindowIdContent  = originWindowIdContent.Insert(windowIdEndIndex, 
                $"\t{replace},\r\n");
            File.WriteAllText(strFilePath, originWindowIdContent);
        
            Debug.Log($"生成ClipID：{replace}完毕");
        }
        public static int GetAudioEndIndex(string content)
        {
            Regex regex = new Regex("ClipID");
            Match match = regex.Match(content);
            Regex regex1 = new Regex("}");
            MatchCollection matchCollection = regex1.Matches(content);
            for (int i = 0; i < matchCollection.Count; i++)
            {
                if (matchCollection[i].Index > match.Index)
                {
                    return matchCollection[i].Index;
                }
            }
            return -1;
        }
    }
}