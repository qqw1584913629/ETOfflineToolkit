using System;
using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class UIEventComponentAwakeSystem : AwakeSystem<UIEventComponent>
    {
        protected override void Awake(UIEventComponent self)
        {
            UIEventComponent.Instance = self;
            self.UIEventHandlers.Clear();

            var AUIEventAttributeSets = CodeTypes.Instance.GetTypes(typeof(AUIEventAttribute));
            foreach (Type v in AUIEventAttributeSets)
            {
                AUIEventAttribute attr = v.GetCustomAttributes(typeof(AUIEventAttribute), false)[0] as AUIEventAttribute;
                self.UIEventHandlers.Add(attr.WindowID, Activator.CreateInstance(v) as IAUIEventHandler);
            }
        }
    }
    [EntitySystem]
    public class UIEventComponentDestroySystem : DestroySystem<UIEventComponent>
    {
        protected override void Destroy(UIEventComponent self)
        {
            self.UIEventHandlers.Clear();
            self.IsClicked = false;
            UIEventComponent.Instance = null;
        }
    }

    public static class UIEventComponentSystem
    {
        public static IAUIEventHandler GetUIEventHandler(this UIEventComponent self, WindowID windowID)
        {
            if (self.UIEventHandlers.TryGetValue(windowID, out IAUIEventHandler handler))
            {
                return handler;
            }
            Debug.LogError($"windowId : {windowID} is not have any uiEvent");
            return null;
        }

        public static void SetUIClicked(this UIEventComponent self, bool isClicked)
        {
            self.IsClicked = isClicked;
        }
    }
}