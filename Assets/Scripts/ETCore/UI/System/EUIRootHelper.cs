using UnityEngine;

namespace MH
{
    public static class EUIRootHelper
    {
        public static void Init()
        {

        }

        public static Transform GetTargetRoot(Scene root, UIWindowType type)
        {
            switch (type)
            {
                case UIWindowType.Normal:
                    return root.GetComponent<GlobalComponent>().NormalRoot;
                case UIWindowType.Fixed:
                    return root.GetComponent<GlobalComponent>().FixedRoot;
                case UIWindowType.PopUp:
                    return root.GetComponent<GlobalComponent>().PopUpRoot;// GlobalComponent.Instance.PopUpRoot;
                case UIWindowType.Other:
                    return root.GetComponent<GlobalComponent>().OtherRoot;
                default:
                    Debug.LogError("uiroot type is error: " + type.ToString());
                    return null;
            }
        }
    }
}