using System;

namespace MH
{
    [EntitySystem]
    public class UIPathComponentAwakeSystem : AwakeSystem<UIPathComponent>
    {
        protected override void Awake(UIPathComponent self)
        {
            foreach (WindowID windowID in Enum.GetValues(typeof(WindowID)))
            {
                string dlgName = "Dlg" + windowID.ToString().Split('_')[1];
                self.WindowPrefabPath.Add((int)windowID, dlgName);
                self.WindowTypeIdDict.Add(dlgName, (int)windowID);
            }
        }
    }
    [EntitySystem]
    public class UIPathComponentDestroySystem : DestroySystem<UIPathComponent>
    {
        protected override void Destroy(UIPathComponent self)
        {
            self.WindowPrefabPath.Clear();
            self.WindowTypeIdDict.Clear();
        }
    }

    public class UIPathComponentSystem
    {
        
    }
}