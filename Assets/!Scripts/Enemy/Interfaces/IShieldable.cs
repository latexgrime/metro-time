namespace _Scripts.Enemy.Interfaces
{
    public interface IShieldable
    {
        float GetCurrentShield();
        float GetMaxShield();
        void TakeShieldDamage(float damage);
        bool IsDeactivated();
        void Reactivate();
    }
}