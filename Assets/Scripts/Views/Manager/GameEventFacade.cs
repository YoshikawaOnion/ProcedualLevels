using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Views
{
    public interface IPlayerEventAccepter
    {
        IObserver<Collision2D> OnPlayerCollideWithEggSender { get; }
        IObserver<Collision2D> OnPlayerCollideWithTerrainSender { get; }
        IObserver<Collision2D> OnPlayerCollideWithEnemySender { get; }
        IObserver<Models.Enemy> OnPlayerBattleWithEnemySender { get; }
    }

    public interface IGameEventReceiver
    {
        IObservable<Collision2D> OnPlayerCollideWithEggReceiver { get; }
        IObservable<Collision2D> OnPlayerCollideWithTerrainReceiver { get; }
        IObservable<Collision2D> OnPlayerCollideWithEnemyReceiver { get; }
        IObservable<Models.Enemy> OnPlayerBattleWithEnemyReceiver { get; }
    }

    public class GameEventFacade : IPlayerEventAccepter,
        IGameEventReceiver
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
    }
}