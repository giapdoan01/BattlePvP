using System;

public interface IHealthObserver
{
    event Action<float, float> OnHealthChanged;//current,max 

    float CurrentHealth { get; }
    float MaxHealth { get; }
}
