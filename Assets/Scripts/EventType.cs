using Unity.Mathematics;

namespace MH
{
    public struct Main_Init
    {
    }
    public struct SceneChangeStart
    {
    }
    public struct SceneChangeFinish
    {
    }
    public struct AfterUnitCreate
    {
        public Unit Unit;
    }
    public struct AttackUnitStartSpecifyDamage
    {
        public Unit TargetUnit;
        public long Damage;
    }
    public struct AfterCreateCurrentScene
    {
        
    }
}