using System;
using UnityEngine;

namespace MH
{
    [Event(SceneType.Main)]
    public class AfterUnitCreate_EventView : AEvent<Scene, AfterUnitCreate>
    {
        protected override async ETTask Run(Scene scene, AfterUnitCreate a)
        {
            var unit = a.Unit;
            unit.AddComponent<AttributeModifierComponent>();
            var globalComponent = scene.Root.GetComponent<GlobalComponent>();
            GameObject go = GameObjectPoolHelper.GetObjectFromPool(unit.Config.Name);
            if (!go)
            {
                await GameObjectPoolHelper.InitPoolWithPathAsync(unit.Config.Name, unit.Config.Name, 1);
                go = GameObjectPoolHelper.GetObjectFromPool(unit.Config.Name);
            }
            go.transform.SetParent(globalComponent.UnitRoot);    
            var gameObjectComponent = unit.AddComponent<GameObjectComponent>();
            gameObjectComponent.GameObject = go;
            var unitMonoBehaviour = go.GetComponent<UnitMonoBehaviour>() ?? go.AddComponent<UnitMonoBehaviour>();
            unitMonoBehaviour.SetUnitId(unit.Id);


            var animatorComponent = unit.AddComponent<AnimatorComponent>();
            animatorComponent.AddAnimationEventByFrame("Idle", 60, () =>
            {
                Debug.Log("Idle");
            });
            unit.AddComponent<MoveComponent>();
            await ETTask.CompletedTask;
        }
    }
}