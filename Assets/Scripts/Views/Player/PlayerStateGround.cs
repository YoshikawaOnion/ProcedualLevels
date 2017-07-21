using UnityEngine;
using System.Collections;
using UniRx;
using ProcedualLevels.Views;

public class PlayerStateGround : StateMachine
{
    private PlayerContext context;
    private Rigidbody2D rigidbody;
    private CompositeDisposable disposable;

    public void EvStateEnter(PlayerContext context)
    {
        this.context = context;
        this.disposable = new CompositeDisposable();

        rigidbody = context.Owner.GetComponent<Rigidbody2D>();
        Observable.EveryUpdate()
                  .Subscribe(x => Update())
                  .AddTo(disposable);
    }

    protected override void EvStateExit()
    {
        disposable.Dispose();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space))
        {
            rigidbody.AddForce(new Vector2(0, context.Owner.jumpPower));
            context.Owner.ChangeJumpState(Player.JumpingStateName, context);
        }
    }
}