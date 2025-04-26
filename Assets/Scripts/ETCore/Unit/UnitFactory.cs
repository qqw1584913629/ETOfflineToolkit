using cfg;

namespace MH
{
    public static class UnitFactory
    {
        public static async ETTask<Unit> CreateUnit(Scene currentScene, UnitConfig config)
        {
            UnitComponent unitComponent = currentScene.GetComponent<UnitComponent>();
            Unit unit = unitComponent.AddChildWithId<Unit, int>(IdGenerater.GenerateId(), config.Id);
            unit.UnitType = UnitType.Player;
            currentScene.Root.GetComponent<PlayerInfoComponent>().UnitId = unit.Id;
            await EventSystem.Instance.PublishAsync(currentScene.Root, new AfterUnitCreate()
            {
                Unit = unit
            });
            return unit;
        }
    }
}