using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using TMPro;
using UnityEngine;
using UnityEngine.Networking;


namespace MH
{
    public class LocalizationSingleton : LogicSingleton<LocalizationSingleton>, ISingletonAwake
    {
        public Dictionary<string, string> StrKeyValuePairs;

        public Language DefaultLanguage;

        public string StreamPath;

        public Language Language;

        public string XmlStreamPath;

        public XmlDocument XmlDocument;
        public void Awake()
        {
            XmlDocument = new XmlDocument();
            StrKeyValuePairs = new Dictionary<string, string>();
            DefaultLanguage = Language.ChineseSimplified;
            StreamPath = XMLStreamingAssetsPath;



            Language = (Language)PlayerPrefs.GetInt("Setting.Language", 0);
            if (Language == Language.Unspecified)
            {
                Language = DefaultLanguage;
                Debug.LogWarning("Language Unspecified! Using Default Language!");
            }
            LoadXmlStream().Coroutine();
            SetLanguage(Language);
        }
        private readonly HashSet<AutoLocalization> autoLocalizationCache = new HashSet<AutoLocalization>();

        private async ETTask LoadXmlStream()
        {
            XmlStreamPath = $"{Application.streamingAssetsPath}/{StreamPath}/{Language}.xml";
            Debug.Log($"XML Stream Path: {XmlStreamPath}");
            Debug.Log("Loading Languages XML Stream....");

            if (Utility.IsWebGL())
            {
                await LoadXmlStreamAsync();
                return;
            }
            else if (Utility.IsAndroid())
            {
                using var request = UnityWebRequest.Get(StreamPath);
                try 
                {
                    await request.SendWebRequest();
                    if (request.result == UnityWebRequest.Result.Success)
                    {
                        XmlDocument.LoadXml(Utility.RemoveXMLBom(request.downloadHandler.text));
                    }
                    else
                    {
                        Debug.LogError($"下载失败: {request.error}");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"请求异常: {ex.Message}");
                }
            }
            else
            {
                if (File.Exists(XmlStreamPath)) XmlDocument.Load(XmlStreamPath);
            }

            CreateDictionary();
        }

        private async ETTask LoadXmlStreamAsync()
        {
            var request = UnityWebRequest.Get(XmlStreamPath);
            await request.SendWebRequest().GetAwaiter();

            if (request.isDone)
            {
                Debug.Log($"Request is done！");
                XmlDocument.LoadXml(Utility.RemoveXMLBom(request.downloadHandler.text));
                CreateDictionary();
                RefreshText();
            }
            else
            {
                Debug.LogError($"Request failure！{request.error}");
            }
        }

        private void CreateDictionary()
        {
            XmlNodeList xmlNodeList = null;

            try
            {
                xmlNodeList = XmlDocument.SelectSingleNode("Dictionaries/Dictionary").ChildNodes;
            }
            catch
            {
                throw new System.Exception($"Non-existent Language:{Language}");
            }

            StrKeyValuePairs = new Dictionary<string, string>();

            foreach (XmlNode item in xmlNodeList)
            {
                string key = item.Attributes["Key"].Value;
                string value = item.Attributes["Value"].Value;
                StrKeyValuePairs.Add(key, value);
            }
            Debug.Log("Load Simple Languages XML Succeed.");
        }
        public void SetLanguage(Language language){
            SetLanguageAsync(language).Coroutine();
        }
        private async ETTask SetLanguageAsync(Language language)
        {
            Language = language;
            PlayerPrefs.SetInt("Setting.Language", (int)language);
            await LoadXmlStream();
            RefreshText();
        }

        public void RegisterAutoLocalization(AutoLocalization autoLocalization)
        {
            if (autoLocalization == null)
                return;
            autoLocalizationCache.Add(autoLocalization);
        }

        public void UnRegisterAutoLocalization(AutoLocalization autoLocalization)
        {
            if (autoLocalization == null)
                return;
            autoLocalizationCache.Remove(autoLocalization);
        }

        private void RefreshText()
        {
            foreach (var obj in autoLocalizationCache)
            {
                if (obj != null && obj.gameObject != null)
                {
                    obj.ShowText();
                }
            }
        }
        public const string XMLStreamingAssetsPath = "AnyLocalization/XML";

        public string GetString(string key)
        {
            if (StrKeyValuePairs.TryGetValue(key, out string value)) return System.Text.RegularExpressions.Regex.Unescape(value);
            Debug.LogWarning("Non-existent Key:" + key);
            return $"[No Key]{key}";
        }
        public string GetString(string key, object arg0)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, arg0);
            }
            catch (Exception exception)
            {
                return Utility.Format("[Error]{0},{1},{2},{3}", key, value, arg0, exception.ToString());
            }
        }

        public string GetString(string key, object arg0, object arg1)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, arg0, arg1);
            }
            catch (Exception exception)
            {
                return Utility.Format("[Error]{0},{1},{2},{3},{4}", key, value, arg0, arg1, exception.ToString());
            }
        }

        public string GetString(string key, object arg0, object arg1, object arg2)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, arg0, arg1, arg2);
            }
            catch (Exception exception)
            {
                return Utility.Format("[Error]{0},{1},{2},{3},{4},{5}", key, value, arg0, arg1, arg2, exception.ToString());
            }
        }

        public string GetString(string key, params object[] args)
        {
            string value = GetString(key);

            try
            {
                return Utility.Format(value, args);
            }
            catch (Exception exception)
            {
                string errorString = Utility.Format("[Error]{0},{1}", key, value);
                if (args != null)
                {
                    foreach (object arg in args)
                    {
                        errorString += "," + arg.ToString();
                    }
                }

                errorString += "," + exception.ToString();
                return errorString;
            }
        }



        public TMP_FontAsset GetFont(Scene root)
        {
            var config = ConfigsSingleton.Instance.Tables.TbLanguageConfig.GetLanguageConfigByLanguageType(Language);
            var resourcesComponent = root.GetComponent<ResourcesComponent>();
            var loadAssetSync = resourcesComponent.LoadAssetSync<TMP_FontAsset>(config.LanguageFonts);
            return loadAssetSync;
        }
        public TMP_FontAsset GetFontByName(Scene root, string name)
        {
            var resourcesComponent = root.GetComponent<ResourcesComponent>();
            var loadAssetSync = resourcesComponent.LoadAssetSync<TMP_FontAsset>(name);
            return loadAssetSync;
        }
    }
}
