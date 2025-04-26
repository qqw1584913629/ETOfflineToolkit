using System;

namespace MH
{
    public static class UnitHelper
    {
        public static Unit GetUnitByZoneScene(Scene root)
        {
            var playerInfo = root.GetComponent<PlayerInfoComponent>();
            var unit = root.CurrentScene().GetComponent<UnitComponent>().Get(playerInfo.UnitId);
            return unit;
        }
        public static NumericComponent GetUnitNumericByZoneScene(Scene root)
        {
            var playerInfo = root.GetComponent<PlayerInfoComponent>();
            var unit = root.CurrentScene().GetComponent<UnitComponent>().Get(playerInfo.UnitId);
            return unit.GetComponent<NumericComponent>();
        }
    }
}
