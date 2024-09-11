using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using UnityEngine.AI;

public class StateInimigos
{
    public enum STATE
    {
        IDLE, CHASE, ATIRAR
    }

    public enum EVENT
    {
        ENTER, UPDATE, EXIT
    }

    public STATE stateName;
    protected EVENT stage;
    protected GameObject inimigo;
    protected NavMeshAgent agent;
    protected Transform player;
    protected StateInimigos nextState;
    //protected Animator anim;

    float visDistancia = 10.0f;
    float visAngulo = 30.0f;
    
    public StateInimigos (GameObject _inimigo, NavMeshAgent _agent, Transform _player)
    {
        inimigo = _inimigo;
        agent = _agent;
        player = _player;
    }

    public virtual void Enter()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Update()
    {
        stage = EVENT.UPDATE;
    }

    public virtual void Exit()
    {
        stage = EVENT.EXIT;
    }

    public StateInimigos Process()
    {
        if (stage == EVENT.ENTER)
        {
            Enter();
        }
        else if (stage == EVENT.UPDATE)
        {
            Update();
        }
        else
        {
            Exit();
            return nextState;
        }

        return this;
    }

    public bool VejoPlayer()
    {
        Vector3 direcao = player.position - inimigo.transform.position;
        float angulo = Vector3.Angle(direcao, inimigo.transform.forward);
        
        if(direcao.magnitude < visDistancia && angulo < visAngulo)
        {
            return true;
        }

        return false;
    }
}

public class Chase : StateInimigos
{
    public Chase(GameObject _inimigo, NavMeshAgent _agent, Transform _player) : base(_inimigo, _agent, _player)
    {
        stateName = STATE.CHASE;
        agent.speed = 3;
        agent.isStopped = false;
    }

    public override void Enter()
    {
        //mudar animação
        Debug.Log("entrou na chase");
        base.Enter();
    }

    public override void Update()
    {
        agent.SetDestination(player.position);

        if (agent.hasPath)
        {
            if (!VejoPlayer())
            {
                nextState = new Idle(inimigo, agent, player);
                stage = EVENT.EXIT;
            }
        }

        base.Update();
    }

    public override void Exit()
    {
        //reset da animação
        Debug.Log("saiu da chase");
        base.Exit();
    }
}

public class Idle : StateInimigos
{
    public Idle(GameObject _inimigo, NavMeshAgent _agent, Transform _player) : base(_inimigo, _agent, _player)
    {
        stateName = STATE.IDLE;
        agent.isStopped = true;
    }

    public override void Enter()
    {
        //mudar animação
        Debug.Log("entrou em idle");
        base.Enter();
    }

    public override void Update()
    {
        Debug.Log("checkando");
        if (VejoPlayer())
        {
            nextState = new Chase(inimigo, agent, player);
            stage = EVENT.EXIT;
        }
    }

    public override void Exit()
    {
        //reset da animação
        Debug.Log("saiu do idle");
        base.Exit();
    }
}


