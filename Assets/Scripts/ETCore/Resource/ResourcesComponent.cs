using System.Collections.Generic;
using YooAsset;

namespace MH
{
    public class ResourcesComponent: Entity, IAwake, IAwake<string>, IDestroy
    {
        public ResourcePackage package;
        public Dictionary<string, OperationHandleBase> handlers = new();
    }
}