using System;
using UnityEngine;

namespace MH
{
    [EntitySystem]
    public class MoveComponentAwakeSystem : AwakeSystem<MoveComponent>
    {
        protected override void Awake(MoveComponent self)
        {
            self.Speed = 3f;
        }
    }
    
    [EntitySystem]
    public class MoveComponentUpdateSystem : UpdateSystem<MoveComponent>
    {
        protected override void Update(MoveComponent self)
        {
            if (!self.IsMoving)
                return;

            var gameObject = self.GetParent<Unit>().GetComponent<GameObjectComponent>().GameObject;
            var currentPos = gameObject.transform.position;
            var targetPos = new Vector3(self.X, self.Y, self.Z);

            // 移动
            gameObject.transform.position = Vector3.MoveTowards(currentPos, targetPos, self.Speed * Time.deltaTime);

            // 计算朝向
            Vector3 direction = (targetPos - currentPos);
            if (direction != Vector3.zero) // 确保有方向可以旋转
            {
                // 只在xz平面上旋转（保持y轴不变）
                direction.y = 0;
                Quaternion targetRotation = Quaternion.LookRotation(direction);
                // 平滑旋转
                gameObject.transform.rotation = Quaternion.Slerp(
                    gameObject.transform.rotation, 
                    targetRotation, 
                    Time.deltaTime * 10f); // 10f 是旋转速度，可以根据需要调整
            }

            // 检查是否到达目标点
            if ((currentPos - targetPos).sqrMagnitude < 0.1f)
            {
                self.IsMoving = false;
                EventSystem.Instance.PublishAsync(self.Scene, new MoveFinish()).Coroutine();
            }
        }
    }
    [EntitySystem]
    public class MoveComponentDestroySystem : DestroySystem<MoveComponent>
    {
        protected override void Destroy(MoveComponent self)
        {
        }
    }

    public static class MoveComponentSystem
    {
        public static void MoveTo(this MoveComponent self, float x, float y, float z)
        {
            self.X = x;
            self.Y = y;
            self.Z = z;
            self.IsMoving = true;
            EventSystem.Instance.PublishAsync(self.Scene, new MoveStart()).Coroutine();
        }
    }
}
