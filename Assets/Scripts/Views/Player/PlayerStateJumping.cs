using UnityEngine;
using System.Collections;
using UniRx;
using System;
using ProcedualLevels.Views;

public class PlayerStateJumping : StateMachine
{
    private PlayerContext context;
    private CompositeDisposable disposable;

    public void EvStateEnter(PlayerContext context)
    {
        this.context = context;
        this.disposable = new CompositeDisposable();

        context.GameEvents.OnPlayerCollideWithTerrainReceiver
               .Merge(context.GameEvents.OnPlayerCollideWithEggReceiver)
               .Subscribe(x => OnHitWithProbablyGround(x))
               .AddTo(disposable);
    }

    protected override void EvStateExit()
    {
        disposable.Dispose();
        base.EvStateExit();
    }

    private void OnHitWithProbablyGround(Collision2D collision)
    {
        var copy = collision.gameObject.GetComponent<Copy>();
        if (copy != null && !copy.IsOnGround)
        {
            return;
        }
        foreach (var contact in collision.contacts)
        {
            if (contact.normal.x <= 0.15f
               && contact.normal.x >= -0.15f)
            {
                context.Owner.ChangeJumpState(Player.GroundStateName, context);
            }
        }
    }
}