using System.Collections.Generic;

namespace MH
{
    public class UIPathComponent : Entity, IAwake, IDestroy
    {

        public Dictionary<int, string> WindowPrefabPath = new Dictionary<int, string>();

        public Dictionary<string, int> WindowTypeIdDict = new Dictionary<string, int>();
    }
}