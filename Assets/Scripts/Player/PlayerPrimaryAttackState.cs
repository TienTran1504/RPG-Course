using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrimaryAttackState : PlayerState
{
    private int comboCounter;

    private float lastTimeAttacked;
    private float comboWindow = 2;
    public PlayerPrimaryAttackState(Player _player, PlayerStateMachine _stateMachine, string _animBoolName): base(_player,_stateMachine,_animBoolName) {

    }

    public override void Enter()
    {
        base.Enter();
        xInput = 0; // need this to fix bug on attack direction

        if (comboCounter > 2 || Time.time >= lastTimeAttacked + comboWindow) comboCounter = 0;

        player.anim.SetInteger("ComboCounter", comboCounter);
        // player.anim.speed = 1.2f;


        float attackDir = player.facingDir;
        if (xInput != 0 ){
            attackDir = xInput;
        }

        player.SetVelocity(player.attackMovement[comboCounter].x * attackDir, player.attackMovement[comboCounter].y);

        stateTimer = .1f;
    }

    public override void Exit()
    {
        base.Exit();
        player.StartCoroutine("BusyFor", .15f);
        // player.anim.speed = 1;
        comboCounter++;
        lastTimeAttacked = Time.time;

    }

     public override void Update()
    {
        base.Update();

        if (stateTimer < 0)
            player.SetZeroVelocity();

        if(triggerCalled){
            stateMachine.ChangeState(player.idleState);
        }
    }
}
