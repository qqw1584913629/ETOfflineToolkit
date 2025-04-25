namespace MH
{
    public static class BuffFactory
    {
        /// <summary>
        /// 创建buff
        /// </summary>
        /// <param name="caster"></param>
        /// <param name="target"></param>
        /// <param name="configId"></param>
        public static void CreateBuff(Unit caster, Unit target, int configId)
        {
            var buffInfosComponent = target.GetComponent<BuffInfosComponent>();
            var info = buffInfosComponent.AddChild<BuffInfo, int, Unit, Unit>(configId, caster, target);
            buffInfosComponent.AddOrUpdate(info);
        }

        /// <summary>
        /// 移除buff
        /// </summary>
        /// <param name="buffInfo"></param>
        public static void RemoveBuff(BuffInfo buffInfo)
        {
            buffInfo.GetParent<BuffInfosComponent>().Remove(buffInfo.Id);
        }
        /// <summary>
        /// buff层数变化
        /// </summary>
        /// <param name="buffInfo"></param>
        /// <param name="oldLayer"></param>
        /// <param name="newLayer"></param>
        public static void OnBuffLayerValueChange(BuffInfo buffInfo, int oldLayer, int newLayer)
        {
            var attributeModifierComponent = buffInfo.Target.GetComponent<AttributeModifierComponent>();
            if (attributeModifierComponent == null)
                return;
            if (newLayer > oldLayer)
            {
                for (int i = 0; i < buffInfo.Config.BuffInfoConfig.NumericTypes.Count; i++)
                {
                    var key = buffInfo.Config.BuffInfoConfig.NumericTypes[i];
                    var value = buffInfo.Config.BuffInfoConfig.NumericValues[i];
                    attributeModifierComponent.AddModifier(key, buffInfo.Id, value, ModifierType.Add);
                }
            }
            else
            {
                for (int i = 0; i < buffInfo.Config.BuffInfoConfig.NumericTypes.Count; i++)
                {
                    var key = buffInfo.Config.BuffInfoConfig.NumericTypes[i];
                    var value = buffInfo.Config.BuffInfoConfig.NumericValues[i];
                    attributeModifierComponent.AddModifier(key, buffInfo.Id, -value, ModifierType.Add);
                }
            }
        }
        /// <summary>
        /// buff开始
        /// </summary>
        /// <param name="buffInfo"></param>
        /// <param name="modifierType"></param>
        public static void OnBuffStart(BuffInfo buffInfo, ModifierType modifierType)
        {
            var attributeModifierComponent = buffInfo.Target.GetComponent<AttributeModifierComponent>();
            if (attributeModifierComponent == null)
                return;
            for (int i = 0; i < buffInfo.Config.BuffInfoConfig.NumericTypes.Count; i++)
            {
                var key = buffInfo.Config.BuffInfoConfig.NumericTypes[i];
                var value = buffInfo.Config.BuffInfoConfig.NumericValues[i];
                attributeModifierComponent.AddModifier(key, buffInfo.Id, value, modifierType);
            }
        }
    }
}