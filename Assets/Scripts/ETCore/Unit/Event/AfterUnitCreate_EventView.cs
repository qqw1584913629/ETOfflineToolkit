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
                var resourcesComponent = scene.GetComponent<ResourcesComponent>();
                var obj = await resourcesComponent.LoadAssetAsync<GameObject>(unit.Config.Name);
                go = UnityEngine.Object.Instantiate(obj, globalComponent.UnitRoot, true);
            }
            go.transform.SetParent(globalComponent.UnitRoot);    
            var gameObjectComponent = unit.AddComponent<GameObjectComponent>();
            gameObjectComponent.GameObject = go;
            var unitMonoBehaviour = go.GetComponent<UnitMonoBehaviour>() ?? go.AddComponent<UnitMonoBehaviour>();
            unitMonoBehaviour.SetUnitId(unit.Id);
            await ETTask.CompletedTask;
        }
    }
}