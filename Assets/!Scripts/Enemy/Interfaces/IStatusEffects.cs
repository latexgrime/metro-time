using _Scripts.Player;
using UnityEngine;

namespace _Scripts.Enemy.Interfaces
{
    public interface IStatusEffect
    {
        float Duration { get; set; }
        void ApplyEffect(PlayerMovement player);
        void RemoveEffect(PlayerMovement player);
    }
}