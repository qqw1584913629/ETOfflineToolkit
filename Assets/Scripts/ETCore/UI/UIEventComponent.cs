using System.Collections.Generic;

namespace MH
{
    public class UIEventComponent : Entity, IAwake, IDestroy
    {
        [StaticField]
        public static UIEventComponent Instance { get; set; }
        public readonly Dictionary<WindowID, IAUIEventHandler> UIEventHandlers = new Dictionary<WindowID, IAUIEventHandler>();
        public bool IsClicked { get; set; }
    }
}