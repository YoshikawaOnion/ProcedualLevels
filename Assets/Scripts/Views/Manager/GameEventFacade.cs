using UnityEngine;
using System.Collections;
using UniRx;

namespace ProcedualLevels.Views
{
    public interface IPlayerEventAccepter
    {
        IObserver<Collision2D> OnPlayerCollideWithEggSender { get; }
        IObserver<Collision2D> OnPlayerCollideWithTerrainSender { get; }
    }

    public interface IGameEventReceiver
    {
        IObservable<Collision2D> OnPlayerCollideWithEggReceiver { get; }
        IObservable<Collision2D> OnPlayerCollideWithTerrainReceiver { get; }
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
    }
}