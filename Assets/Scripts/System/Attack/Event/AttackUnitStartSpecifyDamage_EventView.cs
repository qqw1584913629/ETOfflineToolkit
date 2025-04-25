namespace MH
{
    [Event(SceneType.Main)]
    public class AttackUnitStartSpecifyDamage_EventView : AEvent<Scene, AttackUnitStartSpecifyDamage>
    {
        protected override async ETTask Run(Scene scene, AttackUnitStartSpecifyDamage args)
        {
            if (!args.TargetUnit.IsAlive())
                return;
            long hp = args.TargetUnit.GetComponent<NumericComponent>().GetAsInt(NumericType.Hp);
            hp -= args.Damage;
            if (hp <= 0)
            {
                args.TargetUnit.GetComponent<NumericComponent>()[NumericType.Hp] = 0;
                args.TargetUnit.SetAlive(false);
                return;
            }
            args.TargetUnit.GetComponent<NumericComponent>().Set(NumericType.Hp, hp);
            await ETTask.CompletedTask;
        }
    }
}