using System;

namespace MH
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AUIEventAttribute : BaseAttribute
    {
        public WindowID WindowID
        {
            get;
        }

        public AUIEventAttribute(WindowID windowID)
        {
            this.WindowID = windowID;
        }
    }
}