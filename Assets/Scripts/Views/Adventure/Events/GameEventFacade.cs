using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// ゲーム内で発生するイベントを通信するクラス。
    /// </summary>
    public class GameEventFacade : IPlayerEventAccepter, IPowerUpItemEventAccepter,
    IGoalEventAccepter, ISpikeEventAccepter, IGameEventReceiver
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

        public IObservable<Unit> OnPlayerDieReceiver { get { return onPlayerDie; } }
        public IObserver<Unit> OnPlayerDieSender { get { return onPlayerDie; } }
        private Subject<Unit> onPlayerDie = new Subject<Unit>();

        public IObservable<Models.Enemy> OnPlayerAttackedByEnemyReceiver { get { return onPlayerAttackedByEnemy; } }
        public IObserver<Models.Enemy> OnPlayerAttackedByEnemySender { get { return onPlayerAttackedByEnemy; } }
        private Subject<Models.Enemy> onPlayerAttackedByEnemy = new Subject<Models.Enemy>();

        public IObservable<Tuple<SpikeController,BattlerController>> OnBattlerTouchedSpikeReceiver { get { return onBattlerTouchedSpike; } }
        public IObserver<Tuple<SpikeController,BattlerController>> OnBattlerTouchedSpikeSender { get { return onBattlerTouchedSpike; } }
        private Subject<Tuple<SpikeController,BattlerController>> onBattlerTouchedSpike = new Subject<Tuple<SpikeController,BattlerController>>();
    }
}