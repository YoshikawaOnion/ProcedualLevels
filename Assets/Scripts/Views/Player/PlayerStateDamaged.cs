using UnityEngine;
using System.Collections;
using ProcedualLevels.Views;
using UniRx;
using System;

public class PlayerStateDamaged : StateMachine
{
    PlayerContext context;

    protected void EvStateEnter(PlayerContext context)
    {
        this.context = context;
        Observable.Timer(TimeSpan.FromMilliseconds(800))
                  .Subscribe(x => StandBack());
    }

    private void StandBack()
    {
        context.Owner.ChangeDamageState(Player.HealthyStateName, context);
        context.Owner.ChangeAnimation(Player.IdleAnimationName);
    }
}
