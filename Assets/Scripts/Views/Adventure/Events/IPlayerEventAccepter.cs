using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    public interface IPlayerEventAccepter
    {
        IObserver<Collision2D> OnPlayerCollideWithEggSender { get; }
        IObserver<Collision2D> OnPlayerCollideWithTerrainSender { get; }
        IObserver<Collision2D> OnPlayerCollideWithEnemySender { get; }
        IObserver<Models.Enemy> OnPlayerBattleWithEnemySender { get; }
        IObserver<Unit> OnPlayerGoalSender { get; }
        IObserver<Unit> OnPlayerDieSender { get; }
        IObserver<Models.Enemy> OnPlayerAttackedByEnemySender { get; }
    }
}