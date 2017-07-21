using UnityEngine;
using System.Collections;
using ProcedualLevels.Views;
using UniRx;
using System;

public class PlayerStateHealthy : StateMachine
{
    private PlayerContext context;
    private CompositeDisposable disposable;

    protected void EvStateEnter(PlayerContext context)
    {
        this.context = context;
        context.GameEvents.OnPlayerCollideWithEnemyReceiver
               .Subscribe(x => Damage())
               .AddTo(disposable);
    }

    private void Damage()
    {
        context.Owner.ChangeAnimation(Player.DamageAnimationName);
        context.Owner.ChangeDamageState(Player.DamageStateName, context);
        foreach (var item in context.Owner.Charges)
        {
            item.Value = false;
        }
    }

    protected override void EvStateExit()
    {
        disposable.Dispose();
    }
}
