using System;

namespace MH
{
    public class MoveComponent : Entity, IAwake, IUpdate, IDestroy
    {
        public float Speed;
        public float X;
        public float Y;
        public float Z;

        public bool IsMoving;
    }
}
