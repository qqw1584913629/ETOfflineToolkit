using System.Collections.Generic;

namespace MH
{
    public class BuffInfosComponent: Entity, IAwake, IDestroy
    {
        public List<BuffInfo> BuffInfos = new List<BuffInfo>();
    }
}