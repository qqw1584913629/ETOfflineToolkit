using UnityEngine;

namespace MH
{
    public class OperaComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public int TriggerMask;
        public Camera MainCamera;
    }
}