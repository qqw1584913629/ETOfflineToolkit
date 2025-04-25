using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEditor;
using UnityEngine;


public partial class UICodeSpawner
{
    static public void SpawnSubUICode(GameObject gameObject)
    {

        Path2WidgetCachedDict?.Clear();
        Path2WidgetCachedDict = new Dictionary<string, List<Component>>();
        FindAllWidgets(gameObject.transform, "");
        SpawnCodeForSubUI(gameObject);
        SpawnCodeForSubUIBehaviour(gameObject);
        AssetDatabase.Refresh();
    }

    static void SpawnCodeForSubUI(GameObject objPanel)
    {
        if (null == objPanel)
        {
            return;
        }
        string strDlgName = objPanel.name;

        string strFilePath = Application.dataPath + "/Scripts/UI/HotfixView/UIBehaviour/CommonUI" +
                             "";

        if (!System.IO.Directory.Exists(strFilePath))
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath = Application.dataPath + "/Scripts/UI/HotfixView/UIBehaviour/CommonUI/" + strDlgName + "ViewSystem.cs";

        if (System.IO.File.Exists(strFilePath))
        {
            Debug.LogError("已存在 " + strDlgName + "ViewSystem.cs,将不会再次生成。");
            return;
        }

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        
        strBuilder.AppendLine("\t[EntitySystem]");
        strBuilder.AppendFormat("\tpublic class {0}AwakeSystem : AwakeSystem<{1}, Transform>\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tprotected override void Awake({0} self, Transform transform)\n", strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tself.uiTransform = transform;");
        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("\t}");

        strBuilder.AppendLine("\t[EntitySystem]");
        strBuilder.AppendFormat("\tpublic class {0}DestroySystem : DestroySystem<{1}>\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendFormat("\t\tprotected override void Destroy({0} self)\n", strDlgName);
        strBuilder.AppendLine("\t\t{");
        strBuilder.AppendLine("\t\t\tself.DestroyWidget();");
        strBuilder.AppendLine("\t\t}");
        strBuilder.AppendLine("\t}");
        
        
        strBuilder.AppendFormat("\tpublic static partial class {0}ViewSystem \r\n", strDlgName, strDlgName);
        strBuilder.AppendLine("\t{");
        strBuilder.AppendLine("\n");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("\n");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }

    static void SpawnCodeForSubUIBehaviour(GameObject objPanel)
    {
        if (null == objPanel)
        {
            return;
        }
        string strDlgName = objPanel.name;

        string strFilePath = Application.dataPath + "/Scripts/UI/ModelView/UIBehaviour/CommonUI";

        if (!System.IO.Directory.Exists(strFilePath))
        {
            System.IO.Directory.CreateDirectory(strFilePath);
        }
        strFilePath = Application.dataPath + "/Scripts/UI/ModelView/UIBehaviour/CommonUI/" + strDlgName + ".cs";

        StreamWriter sw = new StreamWriter(strFilePath, false, Encoding.UTF8);

        StringBuilder strBuilder = new StringBuilder();
        strBuilder.AppendLine()
            .AppendLine("using UnityEngine;");
        strBuilder.AppendLine("using UnityEngine.UI;");
        strBuilder.AppendLine("namespace MH");
        strBuilder.AppendLine("{");
        strBuilder.AppendFormat("\tpublic  class {0} : Entity, IAwake<UnityEngine.Transform>, IDestroy, IUILogic \r\n", strDlgName)
            .AppendLine("\t{");


        CreateWidgetBindCode(ref strBuilder, objPanel.transform);
        CreateDestroyWidgetCode(ref strBuilder);
        CreateDeclareCode(ref strBuilder);
        strBuilder.AppendLine("\t\tpublic Transform uiTransform = null;");
        strBuilder.AppendLine("\t}");
        strBuilder.AppendLine("}");

        sw.Write(strBuilder);
        sw.Flush();
        sw.Close();
    }
}
