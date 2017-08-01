using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public interface IAdventureView
    {
        void Initialize(AdventureContext context);
        IObservable<Enemy> BattleObservable { get; }
        void Knockback(Battler battlerSubject, Battler battlerAgainst, int power);
        void ShowDeath(Battler subject);
        IObservable<PowerUp> GetPowerUpObservable { get; }
        void PlacePowerUp(int index, PowerUp powerUp);
    }
}