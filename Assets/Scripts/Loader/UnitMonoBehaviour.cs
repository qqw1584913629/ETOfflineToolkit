using UnityEngine;

namespace MH
{
    public class UnitMonoBehaviour: MonoBehaviour
    {
        public long UnitId;

        public void SetUnitId(long id)
        {
            UnitId = id;
        }
    }
}