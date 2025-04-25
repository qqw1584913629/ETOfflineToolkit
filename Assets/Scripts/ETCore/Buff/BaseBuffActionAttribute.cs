using System;

namespace MH
{
    public class BuffActionAttribute : BaseAttribute
    {
        public BuffInfoType buffInfoType;
        public BuffActionAttribute(BuffInfoType buffInfoType)
        {
            this.buffInfoType = buffInfoType;
        }
    }
}
