using System;
using Newtonsoft.Json;
using UnityEngine;

namespace MH
{
    public class GameObjectComponent: Entity, IAwake, IDestroy
    {
        public GameObject GameObject;
    }
}