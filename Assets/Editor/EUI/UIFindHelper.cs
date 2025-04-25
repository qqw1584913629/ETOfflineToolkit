using System.Collections.Generic;
using System;
using System.IO;
using System.Linq;
using System.Text;
using UnityEditor;
using UnityEngine;
public class UIFindHelper
{
    private static Dictionary<string, List<Component>> Path2WidgetCachedDict =null;
    private const string UIWidgetPrefix = "M";
    private const string UIGameObjectPrefix = "MG";
    private const string UIItemPrefix = "Item";
    private const string UIPrefix = "Dlg";

    private static List<string> WidgetInterfaceList = new List<string>()
    {
        "Button",
        "Text",
        "TMPro.TextMeshProUGUI",
        "Input",
        "InputField",
        "TMPro.TMP_InputField",
        "Scrollbar",
        "ToggleGroup",
        "Toggle",
        "Dropdown",
        "Slider",
        "Image",
        "RawImage",
        "UIWarpContent",
        "ET.Client.LoopScrollView",
    };


    public static void SpawnDlgCode(GameObject gameObject)
    {
        string uiName = gameObject.name;
        if (uiName.StartsWith(UIItemPrefix))
        {
            Debug.LogWarning($"-------- 开始生成滚动列表项: {uiName} 相关代码 -------------");
            SpawnLoopItemCode(gameObject);
            Debug.LogWarning($" 开始生成滚动列表项: {uiName} 完毕！！！");
            return;
        }
        if (!uiName.StartsWith(UIPrefix))
        {
            Debug.LogWarning($" 生成代码文件失败，对象不是以Dlg开头");
            return;
        }
        Path2WidgetCachedDict = new Dictionary<string, List<Component>>();
        
        FindAllWidgets(gameObject.transform, "");
        SpawnCodeForDlgSystem(gameObject);
        SpawnCodeForDlgComponent(gameObject);
        SpawnCodeForDlgEventHandle(gameObject);

        AssetDatabase.Refresh();
    }
    static public void SpawnLoopItemCode(GameObject gameObject)
    {
        Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, List<Component>>();
        FindAllWidgets(gameObject.transform, "");
        SpawnCodeForScrollLoopItemBehaviour(gameObject);
        SpawnCodeForScrollLoopItemViewSystem(gameObject);
        AssetDatabase.Refresh();
        
    }
    static void SpawnCodeForScrollLoopItemViewSystem(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name;

        string strFilePath = Application.dataPath + "/Scripts/Item/ItemViewSystem";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/Scripts/Item/ItemViewSystem/Scroll_" + strDlgName + "ViewSystem.cs";
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic static class Scroll_{0}ViewSystem  \r\n", strDlgName)
            .AppendLine("\t{");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    static void SpawnCodeForScrollLoopItemBehaviour(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name;

        string strFilePath = Application.dataPath + "/Scripts/Item/ItemView";

        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath     = Application.dataPath + "/Scripts/Item/ItemView/Scroll_" + strDlgName + ".cs";
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic  class Scroll_{0} : IUIScrollItem \r\n", strDlgName)
            .AppendLine("\t{");
        
        strBuilder.AppendFormat("\t\tpublic Scroll_{0} BindTrans(Transform trans)\r\n",strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tDestroyWidget();");
        strBuilder.AppendLine("\t\t\tthis.uiTransform = trans;");
        strBuilder.AppendLine("\t\t\treturn this;");
        strBuilder.AppendLine("\t\t}\n");
        
        CreateWidgetBindCode(ref strBuilder, gameObject.transform);
        CreateDestroyWidgetCode(ref strBuilder);
        CreateDeclareCode(ref strBuilder);
        
        strBuilder.AppendLine("\t\tpublic Transform uiTransform = null;");
        
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    static void SpawnCodeForDlgSystem(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name ;
        string strDlgSystem =  gameObject.name + "System";


        string strFilePath = Application.dataPath + "/Scripts/UI/" + strDlgName;
        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }

        strFilePath = Application.dataPath + "/Scripts/UI/" + strDlgName + "/" + strDlgSystem + ".cs";
        if(System.IO.File.Exists(strFilePath))
        {
            Debug.LogError("已存在 " + strDlgName + "System.cs,将不会再次生成。");
            return;
        }
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine().AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic static class {0} \r\n", strDlgSystem)
            .AppendLine("\t{");
        strBuilder.AppendLine($"\t\tpublic static void RegisterUIEvent(this {strDlgName}Component self)");
        strBuilder.AppendLine("\t\t{\n\t\t");
        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine($"\t\tpublic static void ShowWindow(this {strDlgName}Component self, object context = null)");
        strBuilder.AppendLine("\t\t{\n\t\t");
        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();


    }

    static void SpawnCodeForDlgEventHandle(GameObject gameObject)
    {
        string strDlgName = gameObject.name;
        string strFilePath = Application.dataPath + "/Scripts/UI/" + strDlgName + "/Event";
        if ( !System.IO.Directory.Exists(strFilePath) )
            System.IO.Directory.CreateDirectory(strFilePath);
	    strFilePath = Application.dataPath + "/Scripts/UI/" + strDlgName + "/Event/" + strDlgName + "EventHandler.cs";
        if(System.IO.File.Exists(strFilePath))
        {
	        Debug.LogError("已存在 " + strDlgName + ".cs,将不会再次生成。");
            return;
        }

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        

        strBuilder.AppendLine("using System;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\t[AUIEvent(WindowID.WindowID_{0})]\n",strDlgName.Substring(3));
        strBuilder.AppendFormat("\tpublic  class {0}EventHandler : IAUIEventHandler", strDlgName);
          strBuilder.AppendLine("\t{");
          
          
          strBuilder.AppendLine("\t\tpublic void OnInitWindowCoreData(BasePanel basePanel)")
	          .AppendLine("\t\t{");

          strBuilder.AppendLine("\t\t\tbasePanel.windowType = UIWindowType.Normal;");
          strBuilder.AppendLine($"\t\t\tbasePanel.uiLogic = Activator.CreateInstance(typeof({strDlgName}Component)) as {strDlgName}Component;");
          strBuilder.AppendLine($"\t\t\tif (basePanel.uiLogic is {strDlgName}Component basePanelUILogic) basePanelUILogic.uiTransform = basePanel.obj.transform;");
          
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
          
          strBuilder.AppendLine("\t\tpublic void OnRegisterUIEvent(BasePanel basePanel)")
            		.AppendLine("\t\t{");
          strBuilder.AppendLine($"\t\t\tif (basePanel.uiLogic is {strDlgName}Component basePanelUILogic) basePanelUILogic.RegisterUIEvent();");
          
          strBuilder.AppendLine("\t\t}")
            .AppendLine();
          
          strBuilder.AppendLine("\t\tpublic void OnShowWindow(BasePanel basePanel, object contextData = null)")
	          .AppendLine("\t\t{");
          strBuilder.AppendLine($"\t\t\tif (basePanel.uiLogic is {strDlgName}Component basePanelUILogic) basePanelUILogic.ShowWindow(contextData);");
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
          
          
          strBuilder.AppendLine("\t\tpublic void OnHideWindow(BasePanel basePanel)")
	          .AppendLine("\t\t{");
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();

            
          strBuilder.AppendLine("\t\tpublic void BeforeUnload(BasePanel basePanel)")
	          .AppendLine("\t\t{");
          
          strBuilder.AppendLine("\t\t}")
	          .AppendLine();
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    static void SpawnCodeForDlgComponent(GameObject gameObject)
    {
        if (null == gameObject)
        {
            return;
        }
        string strDlgName = gameObject.name ;
        string strDlgComponentName =  gameObject.name + "Component";


        string strFilePath = Application.dataPath + "/Scripts/UI/" + strDlgName;
        if ( !System.IO.Directory.Exists(strFilePath) )
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath = Application.dataPath + "/Scripts/UI/" + strDlgName + "/" + strDlgComponentName + ".cs";
        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);
        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine().AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic class {0} : BasePanel, IUILogic\r\n", strDlgComponentName)
            .AppendLine("\t{");
     
        CreateWidgetBindCode(ref strBuilder, gameObject.transform);

        CreateDestroyWidgetCode(ref strBuilder);
	    
        CreateDeclareCode(ref strBuilder);
        strBuilder.AppendFormat("\t\tpublic Transform uiTransform = null;\r\n");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");
        
        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
    public static void CreateDeclareCode(ref StringBuilder strBuilder)
    {
        foreach (KeyValuePair<string,List<Component> > pair in Path2WidgetCachedDict)
        {
            foreach (var info in pair.Value)
            {
                Component widget = info;
                string strClassType = widget.GetType().ToString();

                string widgetName = widget.name + strClassType.Split('.').ToList().Last();
                strBuilder.AppendFormat("\t\tprivate {0} m_{1} = null;\r\n", strClassType, widgetName);
            }
		    
        }
    }
    static string GetWidgetPath(Transform obj, Transform root)
    {
        string path = obj.name;

        while (obj.parent != null && obj.parent != root)
        {
            obj = obj.transform.parent;
            path = obj.name + "/" + path;
        }
        return path;
    }
    public static void CreateDlgWidgetDisposeCode(ref StringBuilder strBuilder,bool isSelf = false)
    {
        string pointStr = isSelf ? "self" : "this";
        foreach (KeyValuePair<string, List<Component>> pair in Path2WidgetCachedDict)
        {
            foreach (var info in pair.Value)
            {
                Component widget = info;
                string strClassType = widget.GetType().ToString();
                string widgetName = widget.name + strClassType.Split('.').ToList().Last();
                strBuilder.AppendFormat("\t\t	{0}.m_{1} = null;\r\n", pointStr,widgetName);
            }
        }
    }
    public static void CreateDestroyWidgetCode( ref StringBuilder strBuilder)
    {
        strBuilder.AppendFormat("\t\tpublic void DestroyWidget()");
        strBuilder.AppendLine("\n\t\t{");
        CreateDlgWidgetDisposeCode(ref strBuilder);
        strBuilder.AppendFormat("\t\t\tthis.uiTransform = null;\r\n");
        strBuilder.AppendLine("\t\t}\n");
    }
    public static void CreateWidgetBindCode(ref StringBuilder strBuilder, Transform transRoot)
    {
        foreach (KeyValuePair<string, List<Component>> pair in Path2WidgetCachedDict)
        {
	        foreach (var info in pair.Value)
	        {
		        Component widget = info;
				string strPath = GetWidgetPath(widget.transform, transRoot);
				string strClassType = widget.GetType().ToString();
				string strInterfaceType = strClassType;
                
				string widgetName = widget.name + strClassType.Split('.').ToList().Last();
				
				
				strBuilder.AppendFormat("		public {0} {1}\r\n", strInterfaceType, widgetName);
				strBuilder.AppendLine("     	{");
				strBuilder.AppendLine("     		get");
				strBuilder.AppendLine("     		{");
				strBuilder.AppendLine("     			if (this.uiTransform == null)");
				strBuilder.AppendLine("     			{");
				strBuilder.AppendLine("     				Debug.LogError(\"uiTransform is null.\");");
				strBuilder.AppendLine("     				return null;");
				strBuilder.AppendLine("     			}");
				strBuilder.AppendFormat("     			if( this.m_{0} == null )\n" , widgetName);
				strBuilder.AppendLine("     			{");
				strBuilder.AppendFormat("		    		this.m_{0} = MUIHelepr.FindDeepChild<{2}>(this.uiTransform.gameObject,\"{1}\");\r\n", widgetName, strPath, strInterfaceType);
				strBuilder.AppendLine("     			}");
				strBuilder.AppendFormat("     			return this.m_{0};\n" , widgetName);
	            strBuilder.AppendLine("     		}");
	            strBuilder.AppendLine("     	}\n");
	        }
        }
    }

    public static void FindAllWidgets(Transform trans, string strPath)
    {
        if (null == trans)
        {
            return;
        }
        for (int nIndex= 0; nIndex < trans.childCount; ++nIndex)
        {
            Transform child = trans.GetChild(nIndex);
            string strTemp = strPath+"/"+child.name;
			
		
            if (child.name.StartsWith(UIGameObjectPrefix))
            {
                List<Component> rectTransfomrComponents = new List<Component>(); 
                rectTransfomrComponents.Add(child.GetComponent<RectTransform>());
                Path2WidgetCachedDict.Add(child.name,rectTransfomrComponents);
            }
            else if (child.name.StartsWith(UIWidgetPrefix))
            {
                foreach (var uiComponent in WidgetInterfaceList)
                {
                    Component component = child.GetComponent(uiComponent);
                    if (null == component)
                    {
                        continue;
                    }
					
                    if ( Path2WidgetCachedDict.ContainsKey(child.name)  )
                    {
                        Path2WidgetCachedDict[child.name].Add(component);
                        continue;
                    }
					
                    List<Component> componentsList = new List<Component>(); 
                    componentsList.Add(component);
                    Path2WidgetCachedDict.Add(child.name, componentsList);
                }
            }
            FindAllWidgets(child, strTemp);
        }
    }
}