﻿using System.Collections;
using System.Collections.Generic;
using UniRx;
using UnityEngine;

namespace ProcedualLevels.Views
{
    /// <summary>
    /// ゲーム イベントの受信口となるインターフェース。
    /// </summary>
	public interface IGameEventReceiver
	{
		IObservable<Collision2D> OnPlayerCollideWithEggReceiver { get; }
		IObservable<Collision2D> OnPlayerCollideWithTerrainReceiver { get; }
		IObservable<Collision2D> OnPlayerCollideWithEnemyReceiver { get; }
		IObservable<Models.Enemy> OnPlayerBattleWithEnemyReceiver { get; }
		IObservable<Models.PowerUp> OnPlayerGetPowerUpReceiver { get; }
		IObservable<Unit> OnPlayerGoalReceiver { get; }
        IObservable<Unit> OnPlayerDieReceiver { get; }
        IObservable<Models.Enemy> OnPlayerAttackedByEnemyReceiver { get; }
        IObservable<Tuple<SpikeController, BattlerController>> OnBattlerTouchedSpikeReceiver { get; }
	}
}