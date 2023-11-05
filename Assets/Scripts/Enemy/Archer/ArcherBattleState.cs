using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherBattleState : EnemyState
{
    private Transform player;
    private Enemy_Archer enemy;
    private int moveDir;
    public ArcherBattleState(Enemy _enemyBase, EnemyStateMachine _stateMachine, string _animBoolName, Enemy_Archer _enemy) : base(_enemyBase, _stateMachine, _animBoolName)
    {
        this.enemy = _enemy;
    }

    public override void Enter()
    {
        base.Enter();

        player = PlayerManager.instance.player.transform;

        if (player.GetComponent<PlayerStats>().isDead)
        {
            stateMachine.ChangeState(enemy.moveState);
        }
    }
    public override void Update()
    {
        base.Update();

        if (enemy.IsPlayerDetected())
        {

            stateTimer = enemy.battleTime;
            if (enemy.IsPlayerDetected().distance < enemy.safeDistance){
                if(CanJump()){
                    stateMachine.ChangeState(enemy.jumpState);
                }

            }

            if (enemy.IsPlayerDetected().distance < enemy.attackDistance)
            {
                if (CanAttack()) stateMachine.ChangeState(enemy.attackState);
            }
        }
        else
        {
            if (stateTimer < 0 || Vector2.Distance(player.transform.position, enemy.transform.position) > 10)
            {
                stateMachine.ChangeState(enemy.idleState);
            }
        }
        BattleStateFlipControl();

       

    }
    private void BattleStateFlipControl(){
         if (player.position.x > enemy.transform.position.x && enemy.facingDir ==-1)
        {
            enemy.Flip();
        }
        else if (player.position.x < enemy.transform.position.x && enemy.facingDir == 1){
            enemy.Flip();
        }
    }

    public override void Exit()
    {
        base.Exit();
    }

    private bool CanAttack()
    {
        if (Time.time >= enemy.lastTimeAttacked + enemy.attackCoolDown)
        {
            enemy.attackCoolDown = Random.Range(enemy.minAttackCoolDown, enemy.maxAttackCoolDown);
            enemy.lastTimeAttacked = Time.time;
            return true;
        }
        return false;
    }

    private bool CanJump() {
        if(enemy.GroundBehindCheck() == false || enemy.WallBehindCheck() ==true){
            return false;
        }
        if(Time.time >= enemy.lastTimeJumped + enemy.jumpCooldown){
            enemy.lastTimeJumped = Time.time;
            return true;
        }

        return false;

    }
}
