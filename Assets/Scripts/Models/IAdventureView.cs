using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Models
{
    public interface IAdventureView
    {
        void Initialize(AdventureContext context);
        void Knockback(Battler battlerSubject, Battler battlerAgainst, int power);
        void ShowDeath(Battler subject);
        void PlacePowerUp(int index, PowerUp powerUp);

		IObservable<IAdventureView> ResetAsync();

		IObservable<Enemy> OnBattle { get; }
		IObservable<PowerUp> OnGetPowerUp { get; }
		IObservable<Unit> OnGoal { get; }
        IObservable<Unit> OnPlayerDie { get; }
    }
}