using UnityEngine;

namespace MH
{
    [DisallowMultipleComponent]
    [AddComponentMenu("")]
    public class PoolObject : MonoBehaviour
    {
        public string poolName;
        // 定义对象是否在对象池中等待或正在使用
        public bool isPooled;
    }
}