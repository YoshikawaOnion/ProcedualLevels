using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Views
{
    public class GameEventFacade : IPlayerEventAccepter, IPowerUpItemEventAccepter,
    IGoalEventAccepter, IGameEventReceiver
    {
        public IObservable<Collision2D> OnPlayerCollideWithEggReceiver { get { return onPlayerCollideWithEgg; } }
        public IObserver<Collision2D> OnPlayerCollideWithEggSender { get { return onPlayerCollideWithEgg; } }
        private Subject<Collision2D> onPlayerCollideWithEgg = new Subject<Collision2D>();

        public IObservable<Collision2D> OnPlayerCollideWithTerrainReceiver { get { return onPlayerCollideWithTerrain; } }
        public IObserver<Collision2D> OnPlayerCollideWithTerrainSender { get { return onPlayerCollideWithTerrain; } }
        private Subject<Collision2D> onPlayerCollideWithTerrain = new Subject<Collision2D>();

        public IObservable<Collision2D> OnPlayerCollideWithEnemyReceiver { get { return onPlayerCollideWithEnemy; } }
        public IObserver<Collision2D> OnPlayerCollideWithEnemySender { get { return onPlayerCollideWithEnemy; } }
        private Subject<Collision2D> onPlayerCollideWithEnemy = new Subject<Collision2D>();

        public IObservable<Models.Enemy> OnPlayerBattleWithEnemyReceiver { get { return onPlayerBattleWithEnemy; } }
        public IObserver<Models.Enemy> OnPlayerBattleWithEnemySender { get { return onPlayerBattleWithEnemy; } }
        private Subject<Models.Enemy> onPlayerBattleWithEnemy = new Subject<Models.Enemy>();

        public IObservable<Models.PowerUp> OnPlayerGetPowerUpReceiver { get { return onPlayerGetPowerUp; } }
        public IObserver<Models.PowerUp> OnPlayerGetPowerUpSender { get { return onPlayerGetPowerUp; } }
        private Subject<Models.PowerUp> onPlayerGetPowerUp = new Subject<Models.PowerUp>();

        public IObservable<Unit> OnPlayerGoalReceiver { get { return onPlayerGoal; } }
        public IObserver<Unit> OnPlayerGoalSender { get { return onPlayerGoal; } }
        private Subject<Unit> onPlayerGoal = new Subject<Unit>();
    }
}